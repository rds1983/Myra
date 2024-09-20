using AssetManagementBase;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D.UI.File;
using Myra.Graphics2D.UI.Properties;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using Myra;
using Myra.Utility;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using FontStashSharp;
using FontStashSharp.RichText;
using Myra.MML;
using System.Reflection;
using Myra.Attributes;
using System.Collections;
using System.Xml.Linq;
using System.Xml;

namespace MyraPad.UI
{
	public partial class MainForm
	{
		private const string RowsProportionsName = "RowsProportions";
		private const string ColumnsProportionsName = "ColumnsProportions";
		private const string ProportionsName = "Proportions";
		private const string MenuItemName = "MenuItem";
		private const string ListItemName = "ListItem";

		private static readonly string[] SimpleWidgets = new[]
		{
			"ImageTextButton",
			"SpinButton",
			"HorizontalProgressBar",
			"VerticalProgressBar",
			"HorizontalSeparator",
			"VerticalSeparator",
			"HorizontalSlider",
			"VerticalSlider",
			"Image",
			"Label",
			"TextBox",
			"PropertyGrid",
		};

		private static readonly string[] Containers = new[]
		{
			"Button",
			"ToggleButton",
			"CheckButton",
			"RadioButton",
			"Window",
			"Grid",
			"Panel",
			"ScrollViewer",
			"VerticalSplitPane",
			"HorizontalSplitPane",
			"VerticalStackPanel",
			"HorizontalStackPanel",
			"ListView",
			"ComboView"
		};

		private static readonly string[] SpecialContainers = new[]
{
			"HorizontalMenu",
			"VerticalMenu",
			"TabControl",
		};

		private static readonly Regex TagResolver = new Regex("<([A-Za-z0-9\\.]+)");

		private readonly AsyncTasksQueue _queue = new AsyncTasksQueue();
		private readonly ConcurrentQueue<Action> _uiActions = new ConcurrentQueue<Action>();

		private bool _suppressProjectRefresh = false, _suppressExplorerRefresh = false;
		private string _filePath;
		private bool _isDirty;
		private Project _project;
		private bool _needsCloseTag;
		private string _parentTag;
		private int? _currentTagStart, _currentTagEnd;
		private int _line, _col, _indentLevel;
		private bool _applyAutoIndent = false;
		private bool _applyAutoClose = false;
		private DateTime? _refreshInitiated;
		private readonly Dictionary<string, FontSystem> _fontCache = new Dictionary<string, FontSystem>();
		private readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
		private readonly TreeView _treeViewExplorer;

		private VerticalMenu _autoCompleteMenu = null;

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
					var folder = Path.GetDirectoryName(_filePath);
					PropertyGridSettings.BasePath = folder;
					PropertyGridSettings.AssetManager = AssetManager.CreateFileAssetManager(folder);
					LastFolder = folder;
				}
				else
				{
					PropertyGridSettings.BasePath = string.Empty;
					PropertyGridSettings.AssetManager = MyraEnvironment.DefaultAssetManager;
					PropertyGridSettings.AssetManager.Cache.Clear();
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

				_projectHolder.Widgets.Clear();

				if (_project != null && _project.Root != null)
				{
					_projectHolder.Widgets.Add(_project.Root);
				}

				RefreshExplorer();

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

				return _textSource.Text.Substring(_currentTagStart.Value, _currentTagEnd.Value - _currentTagStart.Value + 1);
			}
		}

		private PropertyGrid PropertyGrid => _propertyGrid;

		private PropertyGridSettings PropertyGridSettings
		{
			get
			{
				return PropertyGrid.Settings;
			}
		}

		public AssetManager AssetManager
		{
			get
			{
				return PropertyGridSettings.AssetManager;
			}
		}

		private Type ParentType
		{
			get
			{
				if (string.IsNullOrEmpty(_parentTag))
				{
					return null;
				}

				return Project.GetWidgetTypeByName(_parentTag);
			}
		}

		private string BaseRichTextPath
		{
			get
			{
				var result = string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetDirectoryName(FilePath);
				if (!string.IsNullOrEmpty(Project.DesignerRtfAssetsPath))
				{
					if (string.IsNullOrEmpty(result) || Path.IsPathRooted(Project.DesignerRtfAssetsPath))
					{
						result = Project.DesignerRtfAssetsPath;
					}
					else
					{
						result = Path.Combine(result, Project.DesignerRtfAssetsPath);
					}
				}

				return result;
			}
		}

		public object NewObject { get; set; }
		public Project NewProject { get; set; }

		public string LastFolder { get; set; }
		public Options Options { get; }

		public MainForm(State state)
		{
			BuildUI();

			_menuFileNew.Selected += NewItemOnClicked;
			_menuFileOpen.Selected += OpenItemOnClicked;
			_menuFileReload.Selected += OnMenuFileReloadSelected;
			_menuFileSave.Selected += SaveItemOnClicked;
			_menuFileSaveAs.Selected += SaveAsItemOnClicked;
			_menuFileExportToCS.Selected += ExportCsItemOnSelected;
			_menuFileLoadStylesheet.Selected += OnMenuFileLoadStylesheet;
			_menuFileResetStylesheet.Selected += OnMenuFileResetStylesheetSelected;
			_menuFileDebugOptions.Selected += DebugOptionsItemOnSelected;
			_menuFileQuit.Selected += QuitItemOnDown;

			_menuItemSelectAll.Selected += (s, a) => { _textSource.SelectAll(); };
			_menuEditFormatSource.Selected += _menuEditUpdateSource_Selected;

			_menuHelpAbout.Selected += AboutItemOnClicked;

			_textSource.CursorPositionChanged += _textSource_CursorPositionChanged;
			_textSource.TextChanged += _textSource_TextChanged;
			_textSource.KeyDown += _textSource_KeyDown;
			_textSource.Char += _textSource_Char;
			_textSource.TextDeleted += _textSource_TextDeleted;
			_textStatus.Text = string.Empty;
			_textLocation.Text = "Line: 0, Column: 0, Indent: 0";

			_textBoxFilter.TextChanged += _textBoxFilter_TextChanged;

			PropertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;
			PropertyGrid.CustomValuesProvider = RecordValuesProvider;
			PropertyGrid.CustomSetter = RecordSetter;
			PropertyGrid.Settings.AssetManager = MyraEnvironment.DefaultAssetManager;

			_topSplitPane.SetSplitterPosition(0, state != null ? state.TopSplitterPosition1 : 0.2f);
			_topSplitPane.SetSplitterPosition(1, state != null ? state.TopSplitterPosition2 : 0.6f);
			_leftSplitPane.SetSplitterPosition(0, state != null ? state.CenterSplitterPosition : 0.5f);

			UpdateMenuFile();

			_treeViewExplorer = new TreeView
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
			};

			_treeViewExplorer.SelectionChanged += _treeViewExplorer_SelectionChanged;

			_panelExplorer.Content = _treeViewExplorer;

			RichTextDefaults.FontResolver = p =>
			{
				// Parse font name and size
				var args = p.Split(',');
				var fontName = args[0].Trim();
				var fontSize = int.Parse(args[1].Trim());

				// _fontCache is field of type Dictionary<string, FontSystem>
				// It is used to cache fonts
				FontSystem fontSystem;
				if (!_fontCache.TryGetValue(fontName, out fontSystem))
				{
					// Load and cache the font system
					fontSystem = new FontSystem();
					fontSystem.AddFont(File.ReadAllBytes(Path.Combine(BaseRichTextPath, fontName)));
					_fontCache[fontName] = fontSystem;
				}

				// Return the required font
				return fontSystem.GetFont(fontSize);
			};

			RichTextDefaults.ImageResolver = p =>
			{
				Texture2D texture;

				// _textureCache is field of type Dictionary<string, Texture2D>
				// it is used to cache textures
				if (!_textureCache.TryGetValue(p, out texture))
				{
					using (var stream = File.OpenRead(Path.Combine(BaseRichTextPath, p)))
					{
						texture = Texture2D.FromStream(MyraEnvironment.GraphicsDevice, stream);
					}

					_textureCache[p] = texture;
				}

				return new TextureFragment(texture);
			};

			if (state != null)
			{
				LastFolder = state.LastFolder;
				Options = state.Options;
			}
			else
			{
				Options = new Options();
			}
		}

		protected override void OnPlacedChanged()
		{
			base.OnPlacedChanged();

			if (Desktop == null)
			{
				return;
			}

			Desktop.ContextMenuClosed += Desktop_ContextMenuClosed;
			Desktop.KeyDownHandler = key =>
			{
				if (_autoCompleteMenu != null &&
					(key == Keys.Up || key == Keys.Down || key == Keys.Enter))
				{
					_autoCompleteMenu.OnKeyDown(key);
				}
				else
				{
					Desktop.OnKeyDown(key);
				}
			};

			Desktop.KeyDown += (s, a) =>
			{
				if (Desktop.HasModalWidget || _mainMenu.IsOpen)
				{
					return;
				}

				if (Desktop.IsKeyDown(Keys.LeftControl) || Desktop.IsKeyDown(Keys.RightControl))
				{
					if (Desktop.IsKeyDown(Keys.N))
					{
						NewItemOnClicked(this, EventArgs.Empty);
					}
					else if (Desktop.IsKeyDown(Keys.O))
					{
						OpenItemOnClicked(this, EventArgs.Empty);
					}
					else if (Desktop.IsKeyDown(Keys.R))
					{
						OnMenuFileReloadSelected(this, EventArgs.Empty);
					}
					else if (Desktop.IsKeyDown(Keys.S))
					{
						SaveItemOnClicked(this, EventArgs.Empty);
					}
					else if (Desktop.IsKeyDown(Keys.E))
					{
						ExportCsItemOnSelected(this, EventArgs.Empty);
					}
					else if (Desktop.IsKeyDown(Keys.Q))
					{
						Studio.Instance.Exit();
					}
					else if (Desktop.IsKeyDown(Keys.F))
					{
						_menuEditUpdateSource_Selected(this, EventArgs.Empty);
					}
				}
			};
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
					Studio.Instance.Exit();
				}
			};

			mb.ShowModal(Desktop);
		}

		private void _textSource_TextDeleted(object _, TextDeletedEventArgs e)
		{
			if (e.Value.Contains('\n'))
			{
				return;
			}

			int startIndexOfLine = _textSource.Text.LastIndexOfSafely('\n', _textSource.CursorPosition - 2);
			var endIndexOfLine = _textSource.Text.IndexOfSafely('\n', _textSource.CursorPosition - 2);
			if (endIndexOfLine < 0)
			{
				endIndexOfLine = _textSource.Text.Length;
			}

			if (startIndexOfLine < 0)
			{
				startIndexOfLine = 0;
			}

			var currentLineString = _textSource.Text[startIndexOfLine..endIndexOfLine];
			if (currentLineString is null)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(currentLineString))
			{
				_textSource.Text = _textSource.Text.Remove(startIndexOfLine, currentLineString.Length);
				_textSource.CursorPosition = startIndexOfLine + 1;
			}
		}

		private void _textBoxFilter_TextChanged(object sender, ValueChangedEventArgs<string> e)
		{
			PropertyGrid.Filter = _textBoxFilter.Text;
			_propertyGridPane.ResetScroll();
		}

		private object[] RecordValuesProvider(Record record)
		{
			if (record.Name != "StyleName")
			{
				// Default processing
				return null;
			}

			var widget = PropertyGrid.Object as Widget;
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

		private void Desktop_ContextMenuClosed(object sender, GenericEventArgs<Widget> e)
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
			if (data == _textSource.Text)
			{
				return;
			}

			_textSource.ReplaceAll(data);
		}

		private void _menuEditUpdateSource_Selected(object sender, EventArgs e)
		{
			try
			{
				var project = Project.LoadFromXml(_textSource.Text, AssetManager);
				_textSource.Text = _project.Save();
			}
			catch (Exception ex)
			{
				var messageBox = Dialog.CreateMessageBox("Error", ex.Message);
				messageBox.ShowModal(Desktop);
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

		private static void ProcessResourcesPaths(Widget w, Func<string, bool> resourceProcessor)
		{
			var type = w.GetType();
			foreach (var res in w.Resources)
			{
				var propertyInfo = type.GetProperty(res.Key);
				if (propertyInfo == null)
				{
					continue;
				}

				// Skip brushes for now
				if (propertyInfo.PropertyType == typeof(IBrush))
				{
					continue;
				}

				var result = resourceProcessor(res.Key);
				if (!result)
				{
					break;
				}
			}
		}

		private void UpdateResourcesPaths(string oldPath, string newPath, Action<bool> onFinished)
		{
			try
			{
				// For now only empty old path is allowed
				if (!string.IsNullOrEmpty(oldPath))
				{
					onFinished(false);
					return;
				}

				// Check whether project has external assets
				var hasExternalResources = false;

				Project.Root.ProcessWidgets(w =>
				{
					ProcessResourcesPaths(w, k =>
					{
						// Found
						hasExternalResources = true;
						return false;
					});

					// Continue iteration depending whether hasExternalResources had been set
					return !hasExternalResources;
				});

				if (!hasExternalResources)
				{
					onFinished(false);
					return;
				}

				var dialog = Dialog.CreateMessageBox("Resources Paths Update", "Would you like to update resources paths so it become relative to the project location?");
				dialog.Closed += (s, a) =>
				{
					if (dialog.Result)
					{
						var updated = false;

						var folder = Path.GetDirectoryName(newPath);
						UIUtils.ProcessWidgets(Project.Root, widget =>
						{
							var newResources = new Dictionary<string, string>();

							ProcessResourcesPaths(widget, key =>
							{
								try
								{
									var path = widget.Resources[key];

									if (Path.IsPathRooted(path))
									{
										path = PathUtils.TryToMakePathRelativeTo(path, folder);
										newResources[key] = path;
									}
								}
								catch (Exception)
								{
								}

								// Continue iteration
								return true;
							});

							// Update resources
							foreach (var pair in newResources)
							{
								if (widget.Resources[pair.Key] != pair.Value)
								{
									updated = true;
									widget.Resources[pair.Key] = pair.Value;
								}
							}

							// Continue iteration
							return true;
						});

						if (updated)
						{
							try
							{
								_suppressProjectRefresh = true;
								UpdateSource();
							}
							finally
							{
								_suppressProjectRefresh = false;
							}
						}
					}

					onFinished(true);
				};

				dialog.ShowModal(Desktop);
			}
			catch (Exception)
			{
				onFinished(false);
			}
		}

		private void ApplyAutoIndent()
		{
			if (!Options.AutoIndent || Options.IndentSpacesSize <= 0 || !_applyAutoIndent)
			{
				return;
			}

			_applyAutoIndent = false;

			var text = _textSource.Text;
			var pos = _textSource.CursorPosition;

			if (string.IsNullOrEmpty(text) || pos == 0 || pos >= text.Length)
			{
				return;
			}

			var indentLevel = _indentLevel;
			bool wrapAfterIndent = text.SubstringSafely(pos + 1, 2) == "</";

			if (indentLevel <= 0)
			{
				return;
			}

			// Insert indent
			var indent = new string(' ', indentLevel * Options.IndentSpacesSize);
			if (wrapAfterIndent)
			{
				indent += '\n';
			}
			_textSource.Insert(pos + 1, indent);

			_textSource.CursorPosition = pos + 2;
			//Move the cursor to the position after indent
		}

		private void ApplyAutoClose()
		{
			if (!Options.AutoClose || !_applyAutoClose)
			{
				return;
			}

			_applyAutoClose = false;

			var pos = _textSource.CursorPosition;
			var currentTag = CurrentTag;
			if (string.IsNullOrEmpty(currentTag) || !_needsCloseTag)
			{
				return;
			}

			var closeTag = "</" + ExtractTag(currentTag) + ">";
			_textSource.Insert(pos + 1, closeTag);

			_textSource.CursorPosition = pos;
			//Method Insert(int, string) has moved the cursor position
			//this will restore the cursor to the position after '<Tag>' and before '<Tag/>'
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

				UpdateCursor();

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

			_queue.QueueLoadProject(_textSource.Text);
		}

		private void QueueUIAction(Action action)
		{
			_uiActions.Enqueue(action);
		}

		public void QueueClearExplorer()
		{
			QueueUIAction(() => _treeViewExplorer.RemoveAllSubNodes());
		}

		public void QueueSetStatusText(string text)
		{
			QueueUIAction(() => _textStatus.Text = text);
		}

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

			if (string.IsNullOrEmpty(_textSource.Text))
			{
				return;
			}

			var cursorPos = _textSource.CursorPosition;
			var text = _textSource.Text;

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

			_textLocation.Text = string.Format("Line: {0}, Col: {1}, Indent: {2}", _line + 1, _col + 1, _indentLevel);

			if (!string.IsNullOrEmpty(_parentTag))
			{
				_parentTag = ExtractTag(_parentTag);

				_textLocation.Text += ", Parent: " + _parentTag;
			}

			if ((lastStart != _currentTagStart || lastEnd != _currentTagEnd))
			{
				PropertyGrid.Object = null;
				_propertyGridPane.ResetScroll();
				if (!string.IsNullOrEmpty(currentTag))
				{
					var xml = currentTag;

					if (_needsCloseTag)
					{
						var tag = ExtractTag(currentTag);
						xml += "</" + tag + ">";
					}

					_queue.QueueLoadObject(xml);
				}
			}

			HandleAutoComplete();
		}

		private void HandleAutoComplete()
		{
			if (Desktop.ContextMenu == _autoCompleteMenu)
			{
				Desktop.HideContextMenu();
			}

			if (_currentTagStart == null || _currentTagEnd != null || string.IsNullOrEmpty(_parentTag))
			{
				return;
			}

			var cursorPos = _textSource.CursorPosition;
			var text = _textSource.Text;

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

								if (Options.AutoIndent && Options.IndentSpacesSize > 0)
								{
									// Indent before cursor pos
									result += "\n";
									var indentSize = Options.IndentSpacesSize * (_indentLevel + 1);
									result += new string(' ', indentSize);
									skip += indentSize;

									// Indent before closing tag
									result += "\n";
									indentSize = Options.IndentSpacesSize * _indentLevel;
									result += new string(' ', indentSize);
								}
								result += "</" + menuItem.Text + ">";
								++skip;
								needsClose = true;
							}

							_textSource.Replace(lastStartPos, lastEndPos - lastStartPos, result);
							_textSource.CursorPosition = lastStartPos + skip;
							if (needsClose)
							{
								//								_textSource.OnKeyDown(Keys.Enter);
							}
						};

						_autoCompleteMenu.Items.Add(menuItem);
					}

					var screen = _textSource.ToGlobal(_textSource.CursorCoords);
					screen.Y += _textSource.Font.LineHeight;

					if (_autoCompleteMenu.Items.Count > 0)
					{
						_autoCompleteMenu.HoverIndex = 0;
					}

					Desktop.ShowContextMenu(_autoCompleteMenu, screen);
					// Keep focus at text field
					Desktop.FocusedKeyboardWidget = _textSource;

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
			AssetManager.Cache.Clear();
			_fontCache.Clear();
			_textureCache.Clear();
			Load(FilePath);
		}

		private void OnMenuFileLoadStylesheet(object sender, EventArgs e)
		{
			AssetManager.Cache.Clear();

			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.xmms|*.xml"
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
					var stylesheet = AssetManager.LoadStylesheet(filePath);
				}
				catch (Exception ex)
				{
					var msg = Dialog.CreateMessageBox("Stylesheet Error", ex.Message);
					msg.ShowModal(Desktop);
					return;
				}

				// Try to make stylesheet path relative to project folder
				filePath = PathUtils.TryToMakePathRelativeTo(filePath, Path.GetDirectoryName(FilePath));

				Project.StylesheetPath = filePath;
				UpdateSource();
				UpdateMenuFile();
			};

			dlg.ShowModal(Desktop);
		}

		private void OnMenuFileResetStylesheetSelected(object sender, EventArgs e)
		{
			AssetManager.Cache.Clear();
			Project.StylesheetPath = null;
			UpdateSource();
			UpdateMenuFile();
		}

		private void DebugOptionsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var debugOptions = new DebugOptionsWindow();
			debugOptions.ShowModal(Desktop);
		}

		private void ExportCsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new ExportOptionsDialog();
			dlg.ShowModal(Desktop);

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

					using (var export = new ExporterCS(Project))
					{
						var strings = new List<string>
						{
							"Success. Following files had been written:"
						};
						strings.AddRange(export.Export());

						var msg = Dialog.CreateMessageBox("Export To C#", string.Join("\n", strings));
						msg.ShowModal(Desktop);
					}
				}
				catch (Exception ex)
				{
					var msg = Dialog.CreateMessageBox("Error", ex.Message);
					msg.ShowModal(Desktop);
				}
			};
		}

		private void PropertyGridOnPropertyChanged(object sender, GenericEventArgs<string> eventArgs)
		{
			IsDirty = true;

			var xml = _project.SaveObjectToXml(PropertyGrid.Object, ExtractTag(CurrentTag), ParentType);

			if (_needsCloseTag)
			{
				xml = xml.Replace("/>", ">");
			}

			if (_currentTagStart != null && _currentTagEnd != null)
			{
				try
				{
					_suppressProjectRefresh = true;
					_textSource.Replace(_currentTagStart.Value,
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
					Studio.Instance.Exit();
				}
			};

			mb.ShowModal(Desktop);
		}

		private void AboutItemOnClicked(object sender, EventArgs eventArgs)
		{
			var messageBox = Dialog.CreateMessageBox("About", "MyraPad " + MyraEnvironment.Version);
			messageBox.ShowModal(Desktop);
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

			dlg.ShowModal(Desktop);
		}

		private void OpenItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.xmmp|*.xml"
			};

			if (!string.IsNullOrEmpty(FilePath))
			{
				dlg.Folder = Path.GetDirectoryName(FilePath);
			}
			else if (!string.IsNullOrEmpty(LastFolder))
			{
				dlg.Folder = LastFolder;
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

			dlg.ShowModal(Desktop);
		}

		public void Update(GameTime gameTime)
		{
			try
			{
				if (_refreshInitiated != null && (DateTime.Now - _refreshInitiated.Value).TotalSeconds >= 0.75f)
				{
					QueueRefreshProject();
				}

				while (!_uiActions.IsEmpty)
				{
					Action action;
					_uiActions.TryDequeue(out action);
					action();
				}

				if (NewObject != null)
				{
					PropertyGrid.ParentType = ParentType;
					PropertyGrid.Object = NewObject;

					// Set the selected item in tree
					try
					{
						_suppressExplorerRefresh = true;

						// Determine selected item
						object selectedItem = null;
						foreach (var pair in Project.ObjectsNodes)
						{
							var lineInfo = (IXmlLineInfo)pair.Item2;
							if (lineInfo.LineNumber - 1 > _line ||
								(lineInfo.LineNumber - 1 == _line && lineInfo.LinePosition - 1 > _col))
							{
								break;
							}

							selectedItem = pair.Item1;
						}

						if (selectedItem != null)
						{
							var node = _treeViewExplorer.FindNode(n => n.Tag == selectedItem);
							_treeViewExplorer.SelectedNode = node;
						}
					}
					finally
					{
						_suppressExplorerRefresh = false;
					}

					_propertyGridPane.ResetScroll();
					NewObject = null;
				}

				if (NewProject != null)
				{
					Project = NewProject;

					if (Project.Stylesheet != null && Project.Stylesheet.DesktopStyle != null)
					{
						_projectHolder.Background = Project.Stylesheet.DesktopStyle.Background;
					}
					else
					{
						_projectHolder.Background = null;
					}
				}
			}
			catch (Exception ex)
			{
				_textStatus.Text = ex.Message;
			}
		}

		private void New(string rootType)
		{
			var source = MyraPad.Resources.NewProjectTemplate.Replace("$containerType", rootType);

			_textSource.Text = source;

			var newLineCount = 0;
			var pos = 0;
			while (pos < _textSource.Text.Length && newLineCount < 3)
			{
				++pos;

				if (_textSource.Text[pos] == '\n')
				{
					++newLineCount;
				}
			}

			_textSource.CursorPosition = pos;
			Desktop.FocusedKeyboardWidget = _textSource;

			FilePath = string.Empty;
			IsDirty = false;
			_projectHolder.Background = null;
		}

		private void ProcessSave(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			UpdateResourcesPaths(FilePath, filePath, updated =>
			{
				File.WriteAllText(filePath, _textSource.Text);

				FilePath = filePath;
				IsDirty = false;

				if (updated)
				{
					QueueRefreshProject();
				}
			});
		}

		private void Save(bool setFileName)
		{
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				var dlg = new FileDialog(FileDialogMode.SaveFile)
				{
					Filter = "*.xmmp"
				};

				if (!string.IsNullOrEmpty(FilePath))
				{
					dlg.FilePath = FilePath;
				}
				else if (!string.IsNullOrEmpty(LastFolder))
				{
					dlg.Folder = LastFolder;
				}

				dlg.ShowModal(Desktop);

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

		public void Load(string filePath)
		{
			try
			{
				var data = File.ReadAllText(filePath);

				FilePath = filePath;

				try
				{
					_suppressProjectRefresh = true;
					_textSource.Text = data;
					_textSource.CursorPosition = 0;
				}
				finally
				{
					_suppressProjectRefresh = false;
				}

				QueueRefreshProject();
				UpdateCursor();
				Desktop.FocusedKeyboardWidget = _textSource;

				IsDirty = false;
			}
			catch (Exception ex)
			{
				var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
				dialog.ShowModal(Desktop);
			}
		}

		private void UpdateTitle()
		{
			var title = string.IsNullOrEmpty(_filePath) ? "MyraPad" : _filePath;

			if (_isDirty)
			{
				title += " *";
			}

			Studio.Instance.Window.Title = title;
		}

		private void UpdateMenuFile()
		{
			_menuFileReload.Enabled = !string.IsNullOrEmpty(FilePath);
		}

		public void Quit()
		{
			_queue.Quit();
		}

		private void BuildExplorerTreeRecursive(ITreeViewNode node, IItemWithId root)
		{
			if (root == null)
			{
				return;
			}

			var id = root.GetType().Name;
			if (!string.IsNullOrEmpty(root.Id))
			{
				id += $" (#{root.Id})";
			}

			var newNode = node.AddSubNode(new Label
			{
				Text = id
			});

			newNode.Tag = root;
			newNode.IsExpanded = true;

			var props = root.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var contentProperty = (from p in props where p.FindAttribute<ContentAttribute>() != null select p).FirstOrDefault();
			if (contentProperty == null)
			{
				return;
			}

			var content = contentProperty.GetValue(root);

			var asList = content as IList;
			if (asList != null)
			{
				// List
				foreach (IItemWithId child in asList)
				{
					BuildExplorerTreeRecursive(newNode, child);
				}

			}
			else
			{
				// Simple
				BuildExplorerTreeRecursive(newNode, (IItemWithId)content);
			}
		}

		private void RefreshExplorer()
		{
			// Save selected index
			int? selectedIndex = null;
			if (_treeViewExplorer.SelectedNode != null)
			{
				selectedIndex = _treeViewExplorer.AllNodes.IndexOf(_treeViewExplorer.SelectedNode);
			}

			_treeViewExplorer.RemoveAllSubNodes();

			if (Project == null || Project.Root == null)
			{
				return;
			}

			BuildExplorerTreeRecursive(_treeViewExplorer, Project.Root);

			// Restore selected index
			if (selectedIndex != null && selectedIndex.Value < _treeViewExplorer.AllNodes.Count)
			{
				try
				{
					_suppressExplorerRefresh = true;

					_treeViewExplorer.SelectedNode = _treeViewExplorer.AllNodes[selectedIndex.Value];
				}
				finally
				{
					_suppressExplorerRefresh = false;
				}
			}
		}

		private void _treeViewExplorer_SelectionChanged(object sender, EventArgs e)
		{
			if (_suppressExplorerRefresh || _treeViewExplorer.SelectedNode == null || Project.ObjectsNodes == null)
			{
				return;
			}

			// Try to find the corresponding node
			Tuple<object, XElement> find = null;
			foreach (var pair in Project.ObjectsNodes)
			{
				if (pair.Item1 == _treeViewExplorer.SelectedNode.Tag)
				{
					find = pair;
					break;
				}
			}

			if (find == null)
			{
				return;
			}

			var lineInfo = (IXmlLineInfo)find.Item2;

			// Set the position
			var currentLineNumber = 0;
			var currentLinePosition = 0;
			for (var pos = 0; pos < _textSource.Text.Length; ++pos)
			{
				if (currentLineNumber > lineInfo.LineNumber - 1 ||
					(currentLineNumber == lineInfo.LineNumber - 1 && currentLinePosition >= lineInfo.LinePosition - 1))
				{
					// Found
					_textSource.CursorPosition = pos;
					Desktop.FocusedKeyboardWidget = _textSource;
					break;
				}

				var c = _textSource.Text[pos];
				switch (c)
				{
					case '\n':
						// New line
						++currentLineNumber;
						currentLinePosition = 0;
						break;

					case '\r':
						// Do nothing
						break;

					default:
						// New row
						++currentLinePosition;
						break;

				}
			}
		}

	}
}