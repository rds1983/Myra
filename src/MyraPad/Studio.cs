using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.File;
using Myra.Graphics2D.UI.Properties;
using Myra.Graphics2D.UI.Styles;
using Myra.MiniJSON;
using MyraPad.UI;
using Myra.Utility;
using Myra;

namespace MyraPad
{
	public class Studio : Game
	{
		private static Studio _instance;

		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private readonly State _state;
		private Desktop _desktop;
		private StudioWidget _ui;
		private PropertyGrid _propertyGrid;
		private Grid _statisticsGrid;
		private TextBlock _gcMemoryLabel;
		private TextBlock _fpsLabel;
		private TextBlock _widgetsCountLabel;
		private TextBlock _drawCallsLabel;
		//		private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
		private string _filePath;
		private string _lastFolder;
		private bool _isDirty;
		private Project _project;

		public static Studio Instance
		{
			get
			{
				return _instance;
			}
		}

		public string FilePath
		{
			get { return _filePath; }

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
			get { return _isDirty; }

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
			get { return _project; }

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
			}
		}

		public bool ShowDebugInfo
		{
			get
			{
				return _statisticsGrid.Visible;
			}

			set
			{
				_statisticsGrid.Visible = value;
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
					for(var i = 0; i < Math.Min(ColorPickerDialog.UserColors.Length, _state.UserColors.Length); ++i)
					{
						ColorPickerDialog.UserColors[i] = new Color(_state.UserColors[i]);
					}
				}

				_lastFolder = _state.LastFolder;
			}
			else
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = 1280;
				_graphicsDeviceManager.PreferredBackBufferHeight = 800;
			}
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

			if (_state == null || string.IsNullOrEmpty(_state.EditedFile))
			{
				New(new Grid());
			}
			else
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

			_ui = new StudioWidget();

			_ui._menuFileNew.Selected += NewItemOnClicked;
			_ui._menuFileOpen.Selected += OpenItemOnClicked;
			_ui._menuFileReload.Selected += OnMenuFileReloadSelected;
			_ui._menuFileSave.Selected += SaveItemOnClicked;
			_ui._menuFileSaveAs.Selected += SaveAsItemOnClicked;
			_ui._menuFileExportToCS.Selected += ExportCsItemOnSelected;
			_ui._menuFileLoadStylesheet.Selected += OnMenuFileLoadStylesheet;
			_ui._menuFileReloadStylesheet.Selected += OnMenuFileReloadStylesheet;
			_ui._menuFileResetStylesheet.Selected += OnMenuFileResetStylesheet;
			_ui._menuFileDebugOptions.Selected += DebugOptionsItemOnSelected;
			_ui._menuFileQuit.Selected += QuitItemOnDown;

			_ui._menuHelpAbout.Selected += AboutItemOnClicked;

			_propertyGrid = new PropertyGrid();
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;

			_ui._propertyGridPane.Content = _propertyGrid;

			_ui._topSplitPane.SetSplitterPosition(0, _state != null ? _state.TopSplitterPosition : 0.75f);
			_ui._leftSplitPane.SetSplitterPosition(0, _state != null ? _state.LeftSplitterPosition : 0.5f);

			_desktop.Widgets.Add(_ui);

			_statisticsGrid = new Grid
			{
				Visible = false
			};

			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall
			};
			_statisticsGrid.Widgets.Add(_gcMemoryLabel);

			_fpsLabel = new TextBlock
			{
				Text = "FPS: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 1
			};

			_statisticsGrid.Widgets.Add(_fpsLabel);

			_widgetsCountLabel = new TextBlock
			{
				Text = "Total Widgets: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 2
			};

			_statisticsGrid.Widgets.Add(_widgetsCountLabel);

			_drawCallsLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 3
			};

			_statisticsGrid.Widgets.Add(_drawCallsLabel);

			_statisticsGrid.HorizontalAlignment = HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.Left = 10;
			_statisticsGrid.Top = -10;
			_desktop.Widgets.Add(_statisticsGrid);

			UpdateMenuFile();
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
			var data = File.ReadAllText(path);
			var root = (Dictionary<string, object>)Json.Deserialize(data);

			var folder = Path.GetDirectoryName(path);

			// Load texture atlases
			var textureAtlases = new Dictionary<string, TextureRegionAtlas>();
			Dictionary<string, object> textureAtlasesNode;
			if (root.GetStyle("textureAtlases", out textureAtlasesNode))
			{
				foreach (var pair in textureAtlasesNode)
				{
					var atlasPath = BuildPath(folder, pair.Key.ToString());
					var imagePath = BuildPath(folder, pair.Value.ToString());
					using (var stream = File.OpenRead(imagePath))
					{
						var texture = Texture2D.FromStream(GraphicsDevice, stream);

						var atlasData = File.ReadAllText(atlasPath);
						textureAtlases[pair.Key] = TextureRegionAtlas.FromJson(atlasData, texture);
					}
				}
			}

			// Load fonts
			var fonts = new Dictionary<string, SpriteFont>();
			Dictionary<string, object> fontsNode;
			if (root.GetStyle("fonts", out fontsNode))
			{
				foreach (var pair in fontsNode)
				{
					var fontPath = BuildPath(folder, pair.Value.ToString());

					var fontData = File.ReadAllText(fontPath);
					fonts[pair.Key] = SpriteFontHelper.LoadFromFnt(fontData,
						s =>
						{
							if (s.Contains("#"))
							{
								var parts = s.Split('#');

								return textureAtlases[parts[0]][parts[1]];
							}

							var imagePath = BuildPath(folder, s);
							using (var stream = File.OpenRead(imagePath))
							{
								var texture = Texture2D.FromStream(GraphicsDevice, stream);

								return new TextureRegion(texture);
							}
						});
				}
			}

			return Stylesheet.CreateFromSource(data,
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

		private void SetStylesheet(Stylesheet stylesheet)
		{
			if (Project.Root != null)
			{
				IterateWidget(Project.Root, w => w.ApplyStylesheet(stylesheet));
			}

			Project.Stylesheet = stylesheet;

			if (stylesheet != null && stylesheet.DesktopStyle != null)
			{
				_ui._projectHolder.Background = stylesheet.DesktopStyle.Background;
			}
			else
			{
				_ui._projectHolder.Background = null;
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
				SetStylesheet(stylesheet);
			}
			catch (Exception ex)
			{
				var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
				dialog.ShowModal(_desktop);
			}
		}

		private void OnMenuFileLoadStylesheet(object sender, EventArgs e)
		{
			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.json"
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
				LoadStylesheet(filePath);

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

				IsDirty = true;
			};

			dlg.ShowModal(_desktop);
		}

		private void OnMenuFileReloadStylesheet(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(Project.StylesheetPath))
			{
				return;
			}

			LoadStylesheet(Project.StylesheetPath);
		}

		private void OnMenuFileResetStylesheet(object sender, EventArgs e)
		{
			SetStylesheet(Stylesheet.Current);
			Project.StylesheetPath = null;
			IsDirty = true;
		}

		private void DebugOptionsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new DebugOptionsDialog();

			dlg.AddOption("Show debug info",
						() => { ShowDebugInfo = true; },
						() => { ShowDebugInfo = false; });

			dlg.ShowModal(_desktop);
		}

		private void ExportCsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new ExportOptionsDialog();
			dlg.ShowModal(_desktop);
		}



		private void PropertyGridOnPropertyChanged(object sender, GenericEventArgs<string> eventArgs)
		{
			IsDirty = true;
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

				Container rootWidget = null;

				if (dlg._radioButtonGrid.IsPressed)
				{
					rootWidget = new Grid();
				}
				else
				if (dlg._radioButtonPanel.IsPressed)
				{
					rootWidget = new Panel();
				}
				else
				if (dlg._radioButtonScrollPane.IsPressed)
				{
					rootWidget = new ScrollPane();
				}
				else
				if (dlg._radioButtonHorizontalSplitPane.IsPressed)
				{
					rootWidget = new HorizontalSplitPane();
				}
				else
				if (dlg._radioButtonVerticalSplitPane.IsPressed)
				{
					rootWidget = new VerticalSplitPane();
				}
				else
				if (dlg._radioButtonWindow.IsPressed)
				{
					rootWidget = new Window();
				}
				else
				if (dlg._radioButtonDialog.IsPressed)
				{
					rootWidget = new Dialog();
				}

				New(rootWidget);
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

			//			_fpsCounter.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false) / 1024);
			//			_fpsLabel.Text = string.Format("FPS: {0}", _fpsCounter.FramesPerSecond);
			_widgetsCountLabel.Text = string.Format("Visible Widgets: {0}", _desktop.CalculateTotalWidgets(true));

			GraphicsDevice.Clear(Color.Black);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

#if !FNA
			_drawCallsLabel.Text = string.Format("Draw Calls: {0}", GraphicsDevice.Metrics.DrawCount);
#else
			_drawCallsLabel.Text = "Draw Calls: ?";
#endif

			//			_fpsCounter.Draw(gameTime);
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

		private void New(Container root)
		{
			var project = new Project
			{
				Root = root
			};

			Project = project;

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

			var data = _project.Save();
			File.WriteAllText(filePath, data);

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
					if (FilePath.EndsWith(".ui"))
					{
						FilePath = FilePath.Substring(0, FilePath.Length - 3) + ".xml";
					}

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
				if (FilePath.EndsWith(".ui"))
				{
					FilePath = FilePath.Substring(0, FilePath.Length - 3) + ".xml";
				}

				ProcessSave(FilePath);
			}
		}

		private void Load(string filePath)
		{
			try
			{
				var data = File.ReadAllText(filePath);

				Project project = null;
				if (filePath.EndsWith(".ui"))
				{
					// JSON
					var dialog = Dialog.CreateMessageBox("Error", "'.ui' is no longer supported. Use MyraPad 0.8.2 to convert it to '.xml'.");
					dialog.ShowModal(_desktop);
					return;
				} else
				{
					// XML
					project = Project.LoadFromXml(data);
				}

				Project = project;
				FilePath = filePath;

				_ui._projectHolder.Background = null;
				if (!string.IsNullOrEmpty(Project.StylesheetPath))
				{
					LoadStylesheet(Project.StylesheetPath);
				}

				_ui._textSource.Text = data;

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
		}
	}
}