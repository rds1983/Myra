using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.File;
using Myra.Graphics2D.UI.Properties;
using Myra.Graphics2D.UI.Styles;
using MyraPad.UI;
using Myra.Utility;
using Myra;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Xml.Linq;
using SpriteFontPlus;

namespace MyraPad
{
	public class Studio : Game
	{
		private static Studio _instance;

		private readonly List<WidgetInfo> _projectInfo = new List<WidgetInfo>();
		private readonly ConcurrentQueue<string> _loadQueue = new ConcurrentQueue<string>();
		private readonly ConcurrentQueue<Project> _newProjectsQueue = new ConcurrentQueue<Project>();
		private readonly AutoResetEvent _refreshProjectEvent = new AutoResetEvent(false);

		private readonly ConcurrentDictionary<string, Stylesheet> _stylesheetCache = new ConcurrentDictionary<string, Stylesheet>();

		private bool _suppressProjectRefresh = false;
		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private readonly State _state;
		private Desktop _desktop;
		private StudioWidget _ui;
		private PropertyGrid _propertyGrid;
		private string _filePath;
		private string _lastFolder;
		private bool _isDirty;
		private Project _project;
		private bool _needsCloseTag;
		private string _parentTag;
		private int? _currentTagStart, _currentTagEnd;
		private int _line, _col, _indentLevel;
		private bool _applyAutoIndent = false;
		private bool _applyAutoClose = false;
		private object _newObject;
		private DateTime? _refreshInitiated;

		private VerticalMenu _autoCompleteMenu = null;
		private readonly Options _options = null;

		private const string RowsProportionsName = "RowsProportions";
		private const string ColumnsProportionsName = "ColumnsProportions";
		private const string ProportionsName = "Proportions";
		private const string MenuItemName = "MenuItem";
		private const string ListItemName = "ListItem";

		private static readonly string[] SimpleWidgets = new[]
		{
			"ImageTextButton",
			"TextButton",
			"ImageButton",
			"RadioButton",
			"SpinButton",
			"CheckBox",
			"HorizontalProgressBar",
			"VerticalProgressBar",
			"HorizontalSeparator",
			"VerticalSeparator",
			"HorizontalSlider",
			"VerticalSlider",
			"Image",
			"Label",
			"TextBox",
		};

		private static readonly string[] Containers = new[]
		{
			"Window",
			"Grid",
			"Panel",
			"ScrollViewer",
			"VerticalSplitPane",
			"HorizontalSplitPane",
			"VerticalStackPanel",
			"HorizontalStackPanel"
		};

		private static readonly string[] SpecialContainers = new[]
{
			"HorizontalMenu",
			"VerticalMenu",
			"ComboBox",
			"ListBox",
			"TabControl",
		};

		public static Studio Instance
		{
			get
			{
				return _instance;
			}
		}

		public string FilePath
		{
			get
			{
				return _filePath;
			}

			set
			{
				if (value == _filePath)
				{
					return;
				}

				_filePath = value;

				if (!string.IsNullOrEmpty(_filePath))
				{
					// Store last folder
					try
					{
						_lastFolder = Path.GetDirectoryName(_filePath);
					}
					catch (Exception)
					{
					}
				}

				UpdateTitle();
				UpdateMenuFile();
			}
		}

		public bool IsDirty
		{
			get
			{
				return _isDirty;
			}

			set
			{
				if (value == _isDirty)
				{
					return;
				}

				_isDirty = value;
				UpdateTitle();
			}
		}

		public Project Project
		{
			get
			{
				return _project;
			}

			set
			{
				if (value == _project)
				{
					return;
				}

				_project = value;

				_ui._projectHolder.Widgets.Clear();

				if (_project != null && _project.Root != null)
				{
					_ui._projectHolder.Widgets.Add(_project.Root);
				}

				UpdateMenuFile();
			}
		}

		private string CurrentTag
		{
			get
			{
				if (_currentTagStart == null || _currentTagEnd == null || _currentTagEnd.Value <= _currentTagStart.Value)
				{
					return null;
				}

				return _ui._textSource.Text.Substring(_currentTagStart.Value, _currentTagEnd.Value - _currentTagStart.Value + 1);
			}
		}

		public Studio()
		{
			_instance = this;

			// Restore state
			_state = State.Load();

			_graphicsDeviceManager = new GraphicsDeviceManager(this);

			if (_state != null)
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = _state.Size.X;
				_graphicsDeviceManager.PreferredBackBufferHeight = _state.Size.Y;

				if (_state.UserColors != null)
				{
					for (var i = 0; i < Math.Min(ColorPickerDialog.UserColors.Length, _state.UserColors.Length); ++i)
					{
						ColorPickerDialog.UserColors[i] = new Color(_state.UserColors[i]);
					}
				}

				_lastFolder = _state.LastFolder;
				_options = _state.Options;
			}
			else
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = 1280;
				_graphicsDeviceManager.PreferredBackBufferHeight = 800;
				_options = new Options();
			}

			ThreadPool.QueueUserWorkItem(RefreshProjectProc);
		}

		protected override void Initialize()
		{
			base.Initialize();

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			BuildUI();

			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

			if (_state != null && !string.IsNullOrEmpty(_state.EditedFile))
			{
				Load(_state.EditedFile);
			}
		}

		public void ClosingFunction(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_isDirty)
			{
				OnExiting();
				e.Cancel = true;
			}
		}

		public void OnExiting()
		{
			var mb = Dialog.CreateMessageBox("Quit", "There are unsaved changes. Do you want to exit without saving?");

			mb.Closed += (o, args) =>
			{
				if (mb.Result)
				{
					Exit();
				}
			};

			mb.ShowModal(_desktop);
		}

		private void BuildUI()
		{
			_desktop = new Desktop();

			_desktop.ContextMenuClosed += _desktop_ContextMenuClosed;
			_desktop.KeyDownHandler = key =>
			{
				if (_autoCompleteMenu != null &&
					(key == Keys.Up || key == Keys.Down || key == Keys.Enter))
				{
					_autoCompleteMenu.OnKeyDown(key);
				}
				else
				{
					_desktop.OnKeyDown(key);
				}
			};

			_desktop.KeyDown += (s, a) =>
			{
				if (_desktop.HasModalWindow || _ui._mainMenu.IsOpen)
				{
					return;
				}

				if (_desktop.DownKeys.Contains(Keys.LeftControl) || _desktop.DownKeys.Contains(Keys.RightControl))
				{
					if (_desktop.DownKeys.Contains(Keys.N))
					{
						NewItemOnClicked(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.O))
					{
						OpenItemOnClicked(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.R))
					{
						OnMenuFileReloadSelected(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.S))
					{
						SaveItemOnClicked(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.E))
					{
						ExportCsItemOnSelected(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.T))
					{
						OnMenuFileReloadStylesheet(this, EventArgs.Empty);
					}
					else if (_desktop.DownKeys.Contains(Keys.Q))
					{
						Exit();
					}
					else if (_desktop.DownKeys.Contains(Keys.F))
					{
						_menuEditUpdateSource_Selected(this, EventArgs.Empty);
					}
				}
			};

			_ui = new StudioWidget();

			_ui._menuFileNew.Selected += NewItemOnClicked;
			_ui._menuFileOpen.Selected += OpenItemOnClicked;
			_ui._menuFileReload.Selected += OnMenuFileReloadSelected;
			_ui._menuFileSave.Selected += SaveItemOnClicked;
			_ui._menuFileSaveAs.Selected += SaveAsItemOnClicked;
			_ui._menuFileExportToCS.Selected += ExportCsItemOnSelected;
			_ui._menuFileLoadStylesheet.Selected += OnMenuFileLoadStylesheet;
			_ui._menuFileReloadStylesheet.Selected += OnMenuFileReloadStylesheet;
			_ui._menuFileResetStylesheet.Selected += OnMenuFileResetStylesheetSelected;
			_ui._menuFileDebugOptions.Selected += DebugOptionsItemOnSelected;
			_ui._menuFileQuit.Selected += QuitItemOnDown;

			_ui._menuItemSelectAll.Selected += (s, a) => { _ui._textSource.SelectAll(); };
			_ui._menuEditFormatSource.Selected += _menuEditUpdateSource_Selected;

			_ui._menuHelpAbout.Selected += AboutItemOnClicked;

			_ui._textSource.CursorPositionChanged += _textSource_CursorPositionChanged;
			_ui._textSource.TextChanged += _textSource_TextChanged;
			_ui._textSource.KeyDown += _textSource_KeyDown;
			_ui._textSource.Char += _textSource_Char;

			_ui._textStatus.Text = string.Empty;
			_ui._textLocation.Text = "Line: 0, Column: 0, Indent: 0";

			_propertyGrid = new PropertyGrid
			{
				IgnoreCollections = true
			};
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;
			_propertyGrid.CustomValuesProvider = RecordValuesProvider;
			_propertyGrid.CustomSetter = RecordSetter;

			_ui._propertyGridPane.Content = _propertyGrid;

			_ui._topSplitPane.SetSplitterPosition(0, _state != null ? _state.TopSplitterPosition : 0.75f);
			_ui._leftSplitPane.SetSplitterPosition(0, _state != null ? _state.LeftSplitterPosition : 0.5f);

			_desktop.Widgets.Add(_ui);

			UpdateMenuFile();
		}

		private object[] RecordValuesProvider(Record record)
		{
			if (record.Name != "StyleName")
			{
				// Default processing
				return null;
			}

			var widget = _propertyGrid.Object as Widget;
			if (widget == null)
			{
				return null;
			}

			var styleNames = Project.Stylesheet.GetStylesByWidgetName(widget.GetType().Name);
			if (styleNames == null || styleNames.Length < 2)
			{
				// Dont show this property if there's only one style(Default) or less
				styleNames = new string[0];
			}

			return (from s in styleNames select (object)s).ToArray();
		}

		private bool RecordSetter(Record record, object obj, object value)
		{
			if (record.Name != "StyleName")
			{
				// Default processing
				return false;
			}

			var widget = obj as Widget;
			if (widget == null)
			{
				return false;
			}

			widget.SetStyle(Project.Stylesheet, (string)value);

			return true;
		}

		private void _desktop_ContextMenuClosed(object sender, GenericEventArgs<Widget> e)
		{
			if (e.Data != _autoCompleteMenu)
			{
				return;
			}

			_autoCompleteMenu = null;
		}

		private void UpdateSource()
		{
			var data = Project != null ? Project.Save() : string.Empty;
			if (data == _ui._textSource.Text)
			{
				return;
			}

			_ui._textSource.ReplaceAll(data);
		}

		private void _menuEditUpdateSource_Selected(object sender, EventArgs e)
		{
			try
			{
				var project = Project.LoadFromXml(_ui._textSource.Text, _project.Stylesheet);
				_ui._textSource.Text = _project.Save();
			}
			catch (Exception ex)
			{
				var messageBox = Dialog.CreateMessageBox("Error", ex.Message);
				messageBox.ShowModal(_desktop);
			}
		}

		private void _textSource_Char(object sender, GenericEventArgs<char> e)
		{
			_applyAutoClose = e.Data == '>';
		}

		private void _textSource_KeyDown(object sender, GenericEventArgs<Keys> e)
		{
			_applyAutoIndent = e.Data == Keys.Enter;
		}

		private void ApplyAutoIndent()
		{
			if (!_options.AutoIndent || _options.IndentSpacesSize <= 0 || !_applyAutoIndent)
			{
				return;
			}

			_applyAutoIndent = false;

			var text = _ui._textSource.Text;
			var pos = _ui._textSource.CursorPosition;

			if (string.IsNullOrEmpty(text) || pos == 0 || pos >= text.Length)
			{
				return;
			}

			var il = _indentLevel;
			if (pos < text.Length - 2 && text[pos] == '<' && text[pos + 1] == '/')
			{
				--il;
			}

			if (il <= 0)
			{
				return;
			}

			// Insert indent
			var indent = new string(' ', il * _options.IndentSpacesSize);
			_ui._textSource.Insert(pos, indent);
		}

		private void ApplyAutoClose()
		{
			if (!_options.AutoClose || !_applyAutoClose)
			{
				return;
			}

			_applyAutoClose = false;

			var text = _ui._textSource.Text;
			var pos = _ui._textSource.CursorPosition;

			var currentTag = CurrentTag;
			if (string.IsNullOrEmpty(currentTag) || !_needsCloseTag)
			{
				return;
			}

			var close = "</" + ExtractTag(currentTag) + ">";
			_ui._textSource.Insert(pos, close);
		}

		private void _textSource_TextChanged(object sender, ValueChangedEventArgs<string> e)
		{
			try
			{
				IsDirty = true;

				if (_suppressProjectRefresh)
				{
					return;
				}

				var newLength = string.IsNullOrEmpty(e.NewValue) ? 0 : e.NewValue.Length;
				var oldLength = string.IsNullOrEmpty(e.OldValue) ? 0 : e.OldValue.Length;
				if (Math.Abs(newLength - oldLength) > 1 || _applyAutoClose)
				{
					// Refresh now
					QueueRefreshProject();
				}
				else
				{
					// Refresh after delay
					_refreshInitiated = DateTime.Now;
				}
			}
			catch (Exception)
			{
			}
		}

		private void QueueRefreshProject()
		{
			_refreshInitiated = null;

			_loadQueue.Enqueue(_ui._textSource.Text);
			_refreshProjectEvent.Set();
		}

		private void RefreshProjectProc(object state)
		{
			while (true)
			{
				_refreshProjectEvent.WaitOne();

				string data;

				// We're interested only in the last data
				while (_loadQueue.Count > 1)
				{
					_loadQueue.TryDequeue(out data);
				}

				if (_loadQueue.TryDequeue(out data))
				{
					try
					{
						_ui._textStatus.Text = "Reloading...";

						var xDoc = XDocument.Parse(data);

						var stylesheet = Stylesheet.Current;
						var stylesheetPathAttr = xDoc.Root.Attribute("StylesheetPath");
						if (stylesheetPathAttr != null)
						{
							try
							{
								stylesheet = StylesheetFromFile(stylesheetPathAttr.Value);
							}
							catch (Exception ex)
							{
								var dialog = Dialog.CreateMessageBox("Stylesheet Error", ex.ToString());
								dialog.ShowModal(_desktop);
							}
						}

						var newProject = Project.LoadFromXml(xDoc, stylesheet);
						_newProjectsQueue.Enqueue(newProject);

						_ui._textStatus.Text = string.Empty;
					}
					catch (Exception ex)
					{
						_ui._textStatus.Text = ex.Message;
					}
				}
			}
		}

		private static readonly Regex TagResolver = new Regex("<([A-Za-z0-9\\.]+)");

		private static string ExtractTag(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return null;
			}

			return TagResolver.Match(source).Groups[1].Value;
		}

		private void UpdatePositions()
		{
			var lastStart = _currentTagStart;
			var lastEnd = _currentTagEnd;

			_line = _col = _indentLevel = 0;
			_parentTag = null;
			_currentTagStart = null;
			_currentTagEnd = null;
			_needsCloseTag = false;

			if (string.IsNullOrEmpty(_ui._textSource.Text))
			{
				return;
			}

			var cursorPos = _ui._textSource.CursorPosition;
			var text = _ui._textSource.Text;

			int? tagOpen = null;
			var isOpenTag = true;
			var length = text.Length;

			string currentTag = null;
			Stack<string> parentStack = new Stack<string>();
			for (var i = 0; i < length; ++i)
			{
				if (tagOpen == null)
				{
					if (i >= cursorPos)
					{
						break;
					}

					currentTag = null;
					_currentTagStart = null;
					_currentTagEnd = null;
				}

				if (i < cursorPos)
				{
					++_col;
				}

				var c = text[i];
				if (c == '\n')
				{
					++_line;
					_col = 0;
				}

				if (c == '<')
				{
					if (tagOpen != null && isOpenTag && i >= cursorPos + 1)
					{
						// tag is not closed
						_currentTagStart = tagOpen;
						_currentTagEnd = null;
						break;
					}

					if (i < length - 1 && text[i + 1] != '?')
					{
						tagOpen = i;
						isOpenTag = text[i + 1] != '/';
					}
				}

				if (tagOpen != null && i > tagOpen.Value && c == '>')
				{
					if (isOpenTag)
					{
						var needsCloseTag = text[i - 1] != '/';
						_needsCloseTag = needsCloseTag;

						currentTag = text.Substring(tagOpen.Value, i - tagOpen.Value + 1);
						_currentTagStart = tagOpen;
						_currentTagEnd = i;

						if (needsCloseTag && i <= cursorPos)
						{
							parentStack.Push(currentTag);
						}
					}
					else
					{
						if (parentStack.Count > 0)
						{
							parentStack.Pop();
						}
					}

					tagOpen = null;
				}
			}

			_indentLevel = parentStack.Count;
			if (parentStack.Count > 0)
			{
				_parentTag = parentStack.Pop();
			}

			_ui._textLocation.Text = string.Format("Line: {0}, Col: {1}, Indent: {2}", _line + 1, _col + 1, _indentLevel);

			if (!string.IsNullOrEmpty(_parentTag))
			{
				_parentTag = ExtractTag(_parentTag);

				_ui._textLocation.Text += ", Parent: " + _parentTag;
			}

			if ((lastStart != _currentTagStart || lastEnd != _currentTagEnd))
			{
				_propertyGrid.Object = null;
				if (!string.IsNullOrEmpty(currentTag))
				{
					var xml = currentTag;

					if (_needsCloseTag)
					{
						var tag = ExtractTag(currentTag);
						xml += "</" + tag + ">";
					}

					ThreadPool.QueueUserWorkItem(LoadObjectAsync, xml);
				}
			}

			HandleAutoComplete();
		}

		private void HandleAutoComplete()
		{
			if (_desktop.ContextMenu == _autoCompleteMenu)
			{
				_desktop.HideContextMenu();
			}

			if (_currentTagStart == null || _currentTagEnd != null || string.IsNullOrEmpty(_parentTag))
			{
				return;
			}

			var cursorPos = _ui._textSource.CursorPosition;
			var text = _ui._textSource.Text;

			// Tag isn't closed
			var typed = text.Substring(_currentTagStart.Value, cursorPos - _currentTagStart.Value);
			if (typed.StartsWith("<"))
			{
				typed = typed.Substring(1);

				var all = BuildAutoCompleteVariants();

				// Filter typed
				if (!string.IsNullOrEmpty(typed))
				{
					all = (from a in all where a.StartsWith(typed, StringComparison.OrdinalIgnoreCase) select a).ToList();
				}

				if (all.Count > 0)
				{
					var lastStartPos = _currentTagStart.Value;
					var lastEndPos = cursorPos;
					// Build context menu
					_autoCompleteMenu = new VerticalMenu();
					foreach (var a in all)
					{
						var menuItem = new MenuItem
						{
							Text = a
						};

						menuItem.Selected += (s, args) =>
						{
							var result = "<" + menuItem.Text;
							var skip = result.Length;
							var needsClose = false;

							if (SimpleWidgets.Contains(menuItem.Text) ||
								Project.IsProportionName(menuItem.Text) ||
								menuItem.Text == MenuItemName ||
								menuItem.Text == ListItemName)
							{
								result += "/>";
								skip += 2;
							}
							else
							{
								result += ">";
								++skip;

								if (_options.AutoIndent && _options.IndentSpacesSize > 0)
								{
									// Indent before cursor pos
									result += "\n";
									var indentSize = _options.IndentSpacesSize * (_indentLevel + 1);
									result += new string(' ', indentSize);
									skip += indentSize;

									// Indent before closing tag
									result += "\n";
									indentSize = _options.IndentSpacesSize * _indentLevel;
									result += new string(' ', indentSize);
								}
								result += "</" + menuItem.Text + ">";
								++skip;
								needsClose = true;
							}

							_ui._textSource.Replace(lastStartPos, lastEndPos - lastStartPos, result);
							_ui._textSource.CursorPosition = lastStartPos + skip;
							if (needsClose)
							{
								//								_ui._textSource.OnKeyDown(Keys.Enter);
							}
						};

						_autoCompleteMenu.Items.Add(menuItem);
					}

					var screen = _ui._textSource.CursorScreenPosition;
					screen.Y += _ui._textSource.Font.LineSpacing;

					if (_autoCompleteMenu.Items.Count > 0)
					{
						_autoCompleteMenu.HoverIndex = 0;
					}

					_desktop.ShowContextMenu(_autoCompleteMenu, screen);
					_refreshInitiated = null;
				}
			}
		}

		private List<string> BuildAutoCompleteVariants()
		{
			var result = new List<string>();

			if (string.IsNullOrEmpty(_parentTag))
			{
				return result;
			}

			if (_parentTag == "Project")
			{
				result.AddRange(Containers);
				result.Add("Dialog");
			}
			else if (Containers.Contains(_parentTag) || _parentTag == "Dialog")
			{
				result.AddRange(SimpleWidgets);
				result.AddRange(Containers);
				result.AddRange(SpecialContainers);
			}
			else if (_parentTag.EndsWith(RowsProportionsName) || _parentTag.EndsWith(ColumnsProportionsName) || _parentTag.EndsWith(ProportionsName))
			{
				result.Add(Project.ProportionName);
			}
			else if (_parentTag.EndsWith("Menu"))
			{
				result.Add("MenuItem");
			}
			else if (_parentTag == "ListBox" || _parentTag == "ComboBox")
			{
				result.Add("ListItem");
			}
			else if (_parentTag == "TabControl")
			{
				result.Add("TabItem");
			}

			if (_parentTag == "Grid")
			{
				result.Add(_parentTag + "." + ColumnsProportionsName);
				result.Add(_parentTag + "." + RowsProportionsName);
				result.Add(_parentTag + "." + Project.DefaultColumnProportionName);
				result.Add(_parentTag + "." + Project.DefaultRowProportionName);
			}

			if (_parentTag == "VerticalStackPanel" || _parentTag == "HorizontalStackPanel")
			{
					result.Add(_parentTag + "." + Project.DefaultProportionName);
					result.Add(_parentTag + "." + ProportionsName);
			}

			result = result.OrderBy(s => !s.Contains('.')).ThenBy(s => s).ToList();

			return result;
		}

		private void LoadObjectAsync(object state)
		{
			try
			{
				var xml = (string)state;
				_newObject = Project.LoadObjectFromXml(xml);
			}
			catch (Exception)
			{
			}
		}

		private void UpdateCursor()
		{
			try
			{
				UpdatePositions();
				ApplyAutoIndent();
				ApplyAutoClose();
			}
			catch (Exception)
			{
			}
		}

		private void _textSource_CursorPositionChanged(object sender, EventArgs e)
		{
			UpdateCursor();
		}

		private void OnMenuFileReloadSelected(object sender, EventArgs e)
		{
			Load(FilePath);
		}

		private static string BuildPath(string folder, string fileName)
		{
			if (Path.IsPathRooted(fileName))
			{
				return fileName;
			}

			return Path.Combine(folder, fileName);
		}

		private Stylesheet StylesheetFromFile(string path)
		{
			if (!Path.IsPathRooted(path))
			{
				path = Path.Combine(Path.GetDirectoryName(FilePath), path);
			}

			Stylesheet stylesheet;
			if (_stylesheetCache.TryGetValue(path, out stylesheet))
			{
				return stylesheet;
			}

			var data = File.ReadAllText(path);
			var doc = XDocument.Parse(data);
			var root = doc.Root;

			var textureAtlases = new Dictionary<string, TextureRegionAtlas>();
			var fonts = new Dictionary<string, SpriteFont>();

			var designer = root.Element("Designer");
			if (designer != null)
			{
				var folder = Path.GetDirectoryName(path);

				foreach(var element in designer.Elements())
				{
					if (element.Name == "TextureAtlas")
					{
						var atlasPath = BuildPath(folder, element.Attribute("Atlas").Value);
						var imagePath = BuildPath(folder, element.Attribute("Image").Value);
						using (var stream = File.OpenRead(imagePath))
						{
							var texture = Texture2D.FromStream(GraphicsDevice, stream);
							var atlasData = File.ReadAllText(atlasPath);
							textureAtlases[Path.GetFileName(atlasPath)] = TextureRegionAtlas.FromXml(atlasData, texture);
						}
					}
					else if (element.Name == "Font")
					{
						var id = element.Attribute("Id").Value;
						var fontPath = BuildPath(folder, element.Attribute("File").Value);

						var fontData = File.ReadAllText(fontPath);
						fonts[id] = BMFontLoader.LoadText(fontData,
							s =>
							{
								if (s.Contains("#"))
								{
									var parts = s.Split('#');
									var region = textureAtlases[parts[0]][parts[1]];

									return new TextureWithOffset(region.Texture, region.Bounds.Location);
								}

								var imagePath = BuildPath(folder, s);
								using (var stream = File.OpenRead(imagePath))
								{
									var texture = Texture2D.FromStream(GraphicsDevice, stream);

									return new TextureWithOffset(texture);
								}
							});
					}
				}
			}

			stylesheet = Stylesheet.LoadFromSource(data,
				s =>
				{
					TextureRegion result;
					foreach (var pair in textureAtlases)
					{
						if (pair.Value.Regions.TryGetValue(s, out result))
						{
							return result;
						}
					}

					throw new Exception(string.Format("Could not find texture region '{0}'", s));
				},
				s =>
				{
					SpriteFont result;

					if (fonts.TryGetValue(s, out result))
					{
						return result;
					}

					throw new Exception(string.Format("Could not find font '{0}'", s));
				}
			);

			_stylesheetCache[path] = stylesheet;

			return stylesheet;
		}

		private static void IterateWidget(Widget w, Action<Widget> a)
		{
			a(w);

			var children = w.GetRealChildren();

			if (children != null)
			{
				foreach (var child in children)
				{
					IterateWidget(child, a);
				}
			}
		}

		private void LoadStylesheet(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			try
			{
				if (!Path.IsPathRooted(filePath))
				{
					filePath = Path.Combine(Path.GetDirectoryName(FilePath), filePath);
				}

				var stylesheet = StylesheetFromFile(filePath);

				Project.StylesheetPath = filePath;
				UpdateSource();
			}
			catch (Exception ex)
			{
				var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
				dialog.ShowModal(_desktop);
			}
		}

		private void ResetStylesheetCache()
		{
			_stylesheetCache.Clear();
		}

		private void OnMenuFileLoadStylesheet(object sender, EventArgs e)
		{
			ResetStylesheetCache();
			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.xml"
			};

			try
			{
				if (!string.IsNullOrEmpty(Project.StylesheetPath))
				{
					var stylesheetPath = Project.StylesheetPath;
					if (!Path.IsPathRooted(stylesheetPath))
					{
						// Prepend folder path
						stylesheetPath = Path.Combine(Path.GetDirectoryName(FilePath), stylesheetPath);
					}

					dlg.Folder = Path.GetDirectoryName(stylesheetPath);
				}
				else if (!string.IsNullOrEmpty(FilePath))
				{
					dlg.Folder = Path.GetDirectoryName(FilePath);
				}
			}
			catch (Exception)
			{
			}

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				var filePath = dlg.FilePath;

				// Check whether stylesheet could be loaded
				try
				{
					var stylesheet = StylesheetFromFile(filePath);
				}
				catch(Exception ex)
				{
					var msg = Dialog.CreateMessageBox("Stylesheet Error", ex.Message);
					msg.ShowModal(_desktop);
					return;
				}

				// Try to make stylesheet path relative to project folder
				try
				{
					var fullPathUri = new Uri(filePath, UriKind.Absolute);

					var folderPath = Path.GetDirectoryName(FilePath);
					if (!folderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
					{
						folderPath += Path.DirectorySeparatorChar;
					}
					var folderPathUri = new Uri(folderPath, UriKind.Absolute);

					filePath = folderPathUri.MakeRelativeUri(fullPathUri).ToString();
				}
				catch (Exception)
				{
				}

				Project.StylesheetPath = filePath;
				UpdateSource();
				UpdateMenuFile();
			};

			dlg.ShowModal(_desktop);
		}

		private void OnMenuFileReloadStylesheet(object sender, EventArgs e)
		{
			ResetStylesheetCache();

			if (string.IsNullOrEmpty(Project.StylesheetPath))
			{
				return;
			}

			QueueRefreshProject();
		}

		private void OnMenuFileResetStylesheetSelected(object sender, EventArgs e)
		{
			ResetStylesheetCache();

			Project.StylesheetPath = null;
			UpdateSource();
			UpdateMenuFile();
		}

		private void DebugOptionsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new DebugOptionsDialog();

			dlg.ShowModal(_desktop);
		}

		private void ExportCsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new ExportOptionsDialog();
			dlg.ShowModal(_desktop);

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				try
				{
					Project.ExportOptions.Namespace = dlg._textNamespace.Text;
					Project.ExportOptions.OutputPath = dlg._textOutputPath.Text;
					Project.ExportOptions.Class = dlg._textClassName.Text;

					UpdateSource();

					var export = new ExporterCS(Instance.Project);

					var strings = new List<string>
					{
						"Success. Following files had been written:"
					};
					strings.AddRange(export.Export());

					var msg = Dialog.CreateMessageBox("Export To C#", string.Join("\n", strings));
					msg.ShowModal(_desktop);
				}
				catch (Exception ex)
				{
					var msg = Dialog.CreateMessageBox("Error", ex.Message);
					msg.ShowModal(_desktop);
				}
			};
		}

		private void PropertyGridOnPropertyChanged(object sender, GenericEventArgs<string> eventArgs)
		{
			IsDirty = true;

			var xml = _project.SaveObjectToXml(_propertyGrid.Object, ExtractTag(CurrentTag));

			if (_needsCloseTag)
			{
				xml = xml.Replace("/>", ">");
			}

			if (_currentTagStart != null && _currentTagEnd != null)
			{
				var t = _ui._textSource.Text;

				try
				{
					_suppressProjectRefresh = true;
					_ui._textSource.Replace(_currentTagStart.Value,
						_currentTagEnd.Value - _currentTagStart.Value + 1,
						xml);
					QueueRefreshProject();
				}
				finally
				{
					_suppressProjectRefresh = false;
				}

				_currentTagEnd = _currentTagStart.Value + xml.Length - 1;
			}
		}

		private void QuitItemOnDown(object sender, EventArgs eventArgs)
		{
			var mb = Dialog.CreateMessageBox("Quit", "Are you sure?");

			mb.Closed += (o, args) =>
			{
				if (mb.Result)
				{
					Exit();
				}
			};

			mb.ShowModal(_desktop);
		}

		private void AboutItemOnClicked(object sender, EventArgs eventArgs)
		{
			var messageBox = Dialog.CreateMessageBox("About", "MyraPad " + MyraEnvironment.Version);
			messageBox.ShowModal(_desktop);
		}

		private void SaveAsItemOnClicked(object sender, EventArgs eventArgs)
		{
			Save(true);
		}

		private void SaveItemOnClicked(object sender, EventArgs eventArgs)
		{
			Save(false);
		}

		private void NewItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dlg = new NewProjectWizard();

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				var rootType = "Grid";

				if (dlg._radioButtonHorizontalStackPanel.IsPressed)
				{
					rootType = "HorizontalStackPanel";
				}
				else
				if (dlg._radioButtonVerticalStackPanel.IsPressed)
				{
					rootType = "VerticalStackPanel";
				}
				else
				if (dlg._radioButtonPanel.IsPressed)
				{
					rootType = "Panel";
				}
				else
				if (dlg._radioButtonScrollViewer.IsPressed)
				{
					rootType = "ScrollViewer";
				}
				else
				if (dlg._radioButtonHorizontalSplitPane.IsPressed)
				{
					rootType = "HorizontalSplitPane";
				}
				else
				if (dlg._radioButtonVerticalSplitPane.IsPressed)
				{
					rootType = "VerticalSplitPane";
				}
				else
				if (dlg._radioButtonWindow.IsPressed)
				{
					rootType = "Window";
				}
				else
				if (dlg._radioButtonDialog.IsPressed)
				{
					rootType = "Dialog";
				}

				New(rootType);
			};

			dlg.ShowModal(_desktop);
		}

		private void OpenItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.xml"
			};

			if (!string.IsNullOrEmpty(FilePath))
			{
				dlg.Folder = Path.GetDirectoryName(FilePath);
			}
			else if (!string.IsNullOrEmpty(_lastFolder))
			{
				dlg.Folder = _lastFolder;
			}

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				var filePath = dlg.FilePath;
				if (string.IsNullOrEmpty(filePath))
				{
					return;
				}

				Load(filePath);
			};

			dlg.ShowModal(_desktop);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (_refreshInitiated != null && (DateTime.Now - _refreshInitiated.Value).TotalSeconds >= 0.75f)
			{
				QueueRefreshProject();
			}

			if (_newObject != null)
			{
				_propertyGrid.Object = _newObject;
				_newObject = null;
			}

			Project newProject;
			while (_newProjectsQueue.Count > 1)
			{
				_newProjectsQueue.TryDequeue(out newProject);
			}

			if (_newProjectsQueue.TryDequeue(out newProject))
			{
				Project = newProject;

				if (Project.Stylesheet != null && Project.Stylesheet.DesktopStyle != null)
				{
					_ui._projectHolder.Background = Project.Stylesheet.DesktopStyle.Background;
				}
				else
				{
					_ui._projectHolder.Background = null;
				}
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_desktop.Render();
		}

		protected override void EndRun()
		{
			base.EndRun();

			var state = new State
			{
				Size = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight),
				TopSplitterPosition = _ui._topSplitPane.GetSplitterPosition(0),
				LeftSplitterPosition = _ui._leftSplitPane.GetSplitterPosition(0),
				EditedFile = FilePath,
				LastFolder = _lastFolder,
				UserColors = (from c in ColorPickerDialog.UserColors select c.PackedValue).ToArray()
			};

			state.Save();
		}

		private void New(string rootType)
		{
			var source = Resources.NewProjectTemplate.Replace("$containerType", rootType);

			_ui._textSource.Text = source;

			var newLineCount = 0;
			var pos = 0;
			while (pos < _ui._textSource.Text.Length && newLineCount < 3)
			{
				++pos;

				if (_ui._textSource.Text[pos] == '\n')
				{
					++newLineCount;
				}
			}

			_ui._textSource.CursorPosition = pos;
			_desktop.FocusedKeyboardWidget = _ui._textSource;


			FilePath = string.Empty;
			IsDirty = false;
			_ui._projectHolder.Background = null;
		}

		private void ProcessSave(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			File.WriteAllText(filePath, _ui._textSource.Text);

			FilePath = filePath;
			IsDirty = false;
		}

		private void Save(bool setFileName)
		{
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				var dlg = new FileDialog(FileDialogMode.SaveFile)
				{
					Filter = "*.xml"
				};

				if (!string.IsNullOrEmpty(FilePath))
				{
					dlg.FilePath = FilePath;
				}
				else if (!string.IsNullOrEmpty(_lastFolder))
				{
					dlg.Folder = _lastFolder;
				}

				dlg.ShowModal(_desktop);

				dlg.Closed += (s, a) =>
				{
					if (dlg.Result)
					{
						ProcessSave(dlg.FilePath);
					}
				};
			}
			else
			{
				ProcessSave(FilePath);
			}
		}

		private void Load(string filePath)
		{
			try
			{
				var data = File.ReadAllText(filePath);

				FilePath = filePath;

				_ui._textSource.Text = data;
				_ui._textSource.CursorPosition = 0;
				UpdateCursor();
				_desktop.FocusedKeyboardWidget = _ui._textSource;

				IsDirty = false;
			}
			catch (Exception ex)
			{
				var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
				dialog.ShowModal(_desktop);
			}
		}

		private void UpdateTitle()
		{
			var title = string.IsNullOrEmpty(_filePath) ? "MyraPad" : _filePath;

			if (_isDirty)
			{
				title += " *";
			}

			Window.Title = title;
		}

		private void UpdateMenuFile()
		{
			_ui._menuFileReload.Enabled = !string.IsNullOrEmpty(FilePath);
			_ui._menuFileReloadStylesheet.Enabled = _project != null && !string.IsNullOrEmpty(_project.StylesheetPath);
		}
	}
}