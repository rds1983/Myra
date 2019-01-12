using System;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using System.IO;
using Myra.Samples.RogueEditor.UI;
using Myra.Graphics2D.UI.File;
using Myra.Samples.RogueEditor.Data;
using Myra.Graphics2D.UI.Properties;

namespace Myra.Samples.RogueEditor
{
	public class Studio : Game
	{
		private const string PathFilter = "*.json";

		private readonly GraphicsDeviceManager _graphics;
		private readonly State _state;
		private Desktop _desktop;
		private StudioWidget _ui;
		private Grid _statisticsGrid;
		private TextBlock _gcMemoryLabel;
		private TextBlock _fpsLabel;
		private TextBlock _widgetsCountLabel;
		private string _filePath;
		private string _lastFolder;
		private bool _isDirty;
		private readonly uint[] _customColors;
		private NewMapDialog _newMapDialog;
		private Explorer _explorer;
		private MapEditor _mapEditor;
		private PropertyGrid _propertyGrid;

		public string FilePath
		{
			get { return _filePath; }

			set
			{
				if (value == _filePath)
				{
					return;
				}

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

				_filePath = value;
				UpdateTitle();
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

		public StudioWidget UI
		{
			get
			{
				return _ui;
			}
		}

		public Module Project
		{
			get
			{
				return _explorer.Project;
			}

			set
			{
				_explorer.Project = value;
			}
		}

		public Explorer Explorer
		{
			get
			{
				return _explorer;
			}
		}

		public static Studio Instance { get; private set; }

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

		public Studio(State state)
		{
			Instance = this;

			_graphics = new GraphicsDeviceManager(this);

			IsMouseVisible = true;
			Window.AllowUserResizing = true;

			// Restore state
			_state = state;

			if (_state != null)
			{
				_graphics.PreferredBackBufferWidth = _state.Size.X;
				_graphics.PreferredBackBufferHeight = _state.Size.Y;
				_customColors = _state.UserColors;
			}
			else
			{
				_graphics.PreferredBackBufferWidth = 1280;
				_graphics.PreferredBackBufferHeight = 800;
			}
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			BuildUI();

			if (_state == null || string.IsNullOrEmpty(_state.EditedFile))
			{
				New();
			}
			else
			{
				Load(_state.EditedFile);
			}

		}

		private void BuildUI()
		{
#if DEBUG
#endif

			_desktop = new Desktop();

			_ui = new StudioWidget();

			_ui._menuFileNew.Selected += NewItemOnClicked;
			_ui._menuFileOpen.Selected += OpenProjectItemOnClicked;
			_ui._menuFileSave.Selected += SaveItemOnClicked;
			_ui._menuFileDebugOptions.Selected += DebugOptionsItemOnSelected;
			_ui._quitMenuItem.Selected += QuitItemOnDown;

			_ui._aboutMenuItem.Selected += AboutItemOnClicked;

			_desktop.Widgets.Add(_ui);

			_ui._topSplitPane.SetSplitterPosition(0, _state != null ? _state.TopSplitterPosition : 0.75f);

			_explorer = new Explorer();
			_explorer.Tree.SelectionChanged += OnExplorerSelectionChanged;
			_ui._explorerContainer.Widgets.Add(_explorer);

			_propertyGrid = new PropertyGrid();
			_propertyGrid.PropertyChanged += OnPropertyChanged;
			_ui._rightSplitPane.Widgets.Add(_propertyGrid);

			_mapEditor = new MapEditor();
			_ui._leftContainer.Widgets.Add(_mapEditor);

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
				GridPositionY = 1
			};
			_statisticsGrid.Widgets.Add(_fpsLabel);

			_widgetsCountLabel = new TextBlock
			{
				Text = "Total Widgets: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 2
			};
			_statisticsGrid.Widgets.Add(_widgetsCountLabel);

			_statisticsGrid.HorizontalAlignment = HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.XHint = 10;
			_statisticsGrid.YHint = -10;
			_desktop.Widgets.Add(_statisticsGrid);
		}

		private void DebugOptionsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new DebugOptionsDialog();

			dlg.AddOption("Show debug info",
						() => { ShowDebugInfo = true; },
						() => { ShowDebugInfo = false; });

			dlg.ShowModal(_desktop);
		}


		private void QuitItemOnDown(object sender, EventArgs eventArgs)
		{
			Exit();
		}

		private void AboutItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dialog = Dialog.CreateMessageBox("About", "Myra.Samples.RogueEditor " + MyraEnvironment.Version);
			dialog.ShowModal(_desktop);
		}

		private void New()
		{
			Module newDocument = Module.New();

			Project = newDocument;

			FilePath = string.Empty;
			IsDirty = false;
		}

		private void NewItemOnClicked(object sender, EventArgs eventArgs)
		{
			New();
		}

		private void SaveItemOnClicked(object sender, EventArgs eventArgs)
		{
			Save(true);
		}

		private void NewMapItemOnClicked(object sender, EventArgs eventArgs)
		{
			_newMapDialog = new NewMapDialog();
			_newMapDialog.ShowModal(_desktop);
			_newMapDialog.ButtonOk.Click += NewOnClicked;
		}

		private void OpenProjectItemOnClicked(object sender, EventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.ChooseFolder)
			{
				Folder = FilePath
			};

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				Load(dlg.Folder);
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
//			_fpsLabel.Text = string.Format("FPS: {0:0.##}", _fpsCounter.FramesPerSecond);
			_widgetsCountLabel.Text = string.Format("Total Widgets: {0}", _desktop.CalculateTotalWidgets(true));

			GraphicsDevice.Clear(Color.Black);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

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
				EditedFile = FilePath,
				UserColors = _customColors,
			};

			state.Save();
		}

		private void NewOnClicked(object sender, EventArgs eventArgs)
		{
			try
			{
				var newMap = new Map();
				newMap.Size = new Point((int)_newMapDialog._spinWidth.Value.Value,
										(int)_newMapDialog._spinHeight.Value.Value);

				TileInfo filler = (TileInfo)_newMapDialog._comboFiller.SelectedItem.Tag;
				for (var x = 0; x < newMap.Size.X; ++x)
				{
					for (var y = 0; y < newMap.Size.Y; ++y)
					{
						newMap.Tiles[x, y] = filler;
					}
				}

				_mapEditor.Map = newMap;

				FilePath = string.Empty;
				IsDirty = false;
			}
			catch (Exception ex)
			{
				var msg = Dialog.CreateMessageBox("Error", ex.Message);
				msg.ShowModal(_desktop);
			}
		}

		private void ProcessSave(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			var data = Project.ToJSON();
			File.WriteAllText(filePath, data);

			FilePath = filePath;
			IsDirty = false;
		}

		private void Save(bool setFileName)
		{
			var filePath = FilePath;
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				var dlg = new FileDialog(FileDialogMode.SaveFile)
				{
					Filter = PathFilter
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
				// Load module
				Module newDocument = Module.FromJSON(File.ReadAllText(filePath));

				Project = newDocument;

				FilePath = filePath;
				IsDirty = false;
			}
			catch (Exception ex)
			{
				ReportError(ex.Message);
			}
		}

		private void ReportError(string message)
		{
			Dialog dlg = Dialog.CreateMessageBox("Error", message);
			dlg.ShowModal(_desktop);
		}

		private void UpdateTitle()
		{
			var title = string.IsNullOrEmpty(_filePath) ? "Myra.Samples.RogueEditor" : _filePath;

			if (_isDirty)
			{
				title += " *";
			}

			Window.Title = title;
		}

		private void OnPropertyChanged(object sender, Utility.GenericEventArgs<string> e)
		{
			IsDirty = true;
		}

		private void OnExplorerSelectionChanged(object sender, EventArgs e)
		{
			_propertyGrid.Object = _explorer.SelectedObject is Project ? null : _explorer.SelectedObject;

			var asMap = _propertyGrid.Object as Map;
			if (asMap != null)
			{
				_mapEditor.Map = asMap;
			}
		}
	}
}