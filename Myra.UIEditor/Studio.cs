using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI;
using Myra.UIEditor.UI;
using Myra.UIEditor.Utils;
using Myra.Utility;
using NLog;
using Color = Microsoft.Xna.Framework.Color;
using Menu = Myra.Graphics2D.UI.Menu;
using MenuItem = Myra.Graphics2D.UI.MenuItem;
using Orientation = Myra.Graphics2D.UI.Orientation;
using Point = Microsoft.Xna.Framework.Point;
using PropertyGrid = Myra.Editor.UI.PropertyGrid;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Myra.UIEditor
{
	public class Studio: Game
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private const string PathFilter = "Myra UIEditor Projects (*.ui)|*.ui";

		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private readonly State _state;
		private Desktop _desktop;
		private SplitPane _topSplitPane;
		private SplitPane _rightSplitPane;
		private Grid _statisticsGrid;
		private TextBlock _gcMemoryLabel;
		private TextBlock _fpsLabel;
		private TextBlock _widgetsCountLabel;
		private TextBlock _drawCallsLabel;
		private Grid _projectHolder;
		private Explorer _explorer;
		private PropertyGrid _propertyGrid;
		private readonly FPSCounter _fpsCounter = new FPSCounter();
		private string _filePath;
		private bool _isDirty;
		private Widget _project;
		private int[] _customColors;

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

		public Widget Project
		{
			get { return _project; }

			set
			{
				if (value == _project)
				{
					return;
				}

				_project = value;
				_explorer.Project = _project;

				_projectHolder.Children.Clear();

				if (_project != null)
				{
					_projectHolder.Children.Add(_project);
				}
			}
		}

		public Studio()
		{
			// Restore state
			_state = State.Load();

			_graphicsDeviceManager = new GraphicsDeviceManager(this);

			if (_state != null)
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = _state.Size.X;
				_graphicsDeviceManager.PreferredBackBufferHeight = _state.Size.Y;
				_customColors = _state.CustomColors;
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
//			BitmapFont.DrawFames = true;
//			Widget.DrawFrames = true;
			Widget.DrawFocused = true;
#endif

			MyraEnvironment.Game = this;

			_desktop = new Desktop();

			var menuBar = new Menu(Orientation.Horizontal);

			var fileMenu = new MenuItem
			{
				Text = "File"
			};

			var newItem = new MenuItem
			{
				Text = "New"
			};
			newItem.Down += NewItemOnClicked;
			fileMenu.AddMenuItem(newItem);

			var openItem = new MenuItem
			{
				Text = "Open.."
			};
			openItem.Down += OpenItemOnClicked;
			fileMenu.AddMenuItem(openItem);

			var saveItem = new MenuItem
			{
				Text = "Save"
			};
			saveItem.Down += SaveItemOnClicked;
			fileMenu.AddMenuItem(saveItem);

			var saveAsItem = new MenuItem
			{
				Text = "Save As..."
			};
			saveAsItem.Down += SaveAsItemOnClicked;

			fileMenu.AddMenuItem(saveAsItem);
			fileMenu.AddSeparator();

			var quitItem = new MenuItem
			{
				Text = "Quit"
			};
			quitItem.Down += QuitItemOnDown;
			fileMenu.AddMenuItem(quitItem);

			menuBar.AddMenuItem(fileMenu);

			var controlsMenu = new MenuItem
			{
				Text = "Controls"
			};

			var addTextBlockItem = new MenuItem
			{
				Text = "Add Text Block"
			};

			controlsMenu.AddMenuItem(addTextBlockItem);
			menuBar.AddMenuItem(controlsMenu);

			var helpMenu = new MenuItem
			{
				Text = "Help"
			};

			var aboutItem = new MenuItem
			{
				Text = "About"
			};
			aboutItem.Down += AboutItemOnClicked;
			helpMenu.AddMenuItem(aboutItem);

			menuBar.AddMenuItem(helpMenu);

			_topSplitPane = new SplitPane(Orientation.Horizontal)
			{
				Id = "topSplitPane",
				HorizontalAlignment = Graphics2D.UI.HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				GridPosition = {Y = 1}
			};

			_projectHolder = new Grid();

			_topSplitPane.Widgets.Add(_projectHolder);

			_rightSplitPane = new SplitPane(Orientation.Vertical);

			_explorer = new Explorer();
			_explorer.Widget.SelectionChanged += WidgetOnSelectionChanged;
			_rightSplitPane.Widgets.Add(_explorer);

			_propertyGrid = new PropertyGrid();
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;
			_propertyGrid.ColorChangeHandler += ColorChangeHandler;

			_rightSplitPane.Widgets.Add(_propertyGrid);

			var root = new Grid();

			root.RowsProportions.Add(new Grid.Proportion
			{
				Type = Grid.ProportionType.Auto
			});

			root.RowsProportions.Add(new Grid.Proportion
			{
				Type = Grid.ProportionType.Part,
				Value = 1.0f
			});

			_topSplitPane.Widgets.Add(_rightSplitPane);

			root.Children.Add(menuBar);
			root.Children.Add(_topSplitPane);

			_desktop.Widgets.Add(root);

			_topSplitPane.SetSplitterPosition(0, _state != null ? _state.TopSplitterPosition : 0.75f);
			_rightSplitPane.SetSplitterPosition(0, _state != null ? _state.RightSplitterPosition : 0.5f);

			_statisticsGrid = new Grid();

			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = BitmapFont.DefaultSmall
			};
			_statisticsGrid.Children.Add(_gcMemoryLabel);

			_fpsLabel = new TextBlock
			{
				Text = "FPS: ",
				Font = BitmapFont.DefaultSmall,
				GridPosition = new Point(0, 1)
			};
			_statisticsGrid.Children.Add(_fpsLabel);

			_widgetsCountLabel = new TextBlock
			{
				Text = "Total Widgets: ",
				Font = BitmapFont.DefaultSmall,
				GridPosition = new Point(0, 2)
			};
			_statisticsGrid.Children.Add(_widgetsCountLabel);

			_drawCallsLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = BitmapFont.DefaultSmall,
				GridPosition = new Point(0, 3)
			};
			_statisticsGrid.Children.Add(_drawCallsLabel);

			_statisticsGrid.HorizontalAlignment = Graphics2D.UI.HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.XHint = 10;
			_statisticsGrid.YHint = -10;
			_desktop.Widgets.Add(_statisticsGrid);
		}

		private void PropertyGridOnPropertyChanged(object sender, EventArgs eventArgs)
		{
			IsDirty = true;
		}

		private Color? ColorChangeHandler(Color? color)
		{
			using (var cp = new ColorDialog())
			{
				cp.FullOpen = true;
				cp.CustomColors = _customColors;

				if (color.HasValue)
				{
					cp.Color = color.Value.ToSystemDrawing();
				}

				var res = cp.ShowDialog();

				_customColors = cp.CustomColors;
				if (res == DialogResult.OK)
				{
					return cp.Color.ToXna();
				}
			}

			return null;
		}

		private void WidgetOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			var treeNode = _explorer.Widget.SelectedRow;

			var selectedObject = treeNode != null ? treeNode.Tag : null;
			_propertyGrid.Object = selectedObject;
		}

		private void QuitItemOnDown(object sender, EventArgs eventArgs)
		{
			var mb = Graphics2D.UI.Window.CreateMessageBox("Quit", "Are you sure?");

			mb.Closed += (o, args) =>
			{
				if (mb.ModalResult == (int)Graphics2D.UI.Window.DefaultModalResult.Ok)
				{
					Exit();
				}
			};

			mb.ShowModal(_desktop);
		}

		private void AboutItemOnClicked(object sender, EventArgs eventArgs)
		{
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
			New();
		}

		private void OpenItemOnClicked(object sender, EventArgs eventArgs)
		{
			string filePath;
			using (var dlg = new OpenFileDialog())
			{
				dlg.Filter = PathFilter;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					filePath = dlg.FileName;
				}
				else
				{
					return;
				}
			}

			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			Load(filePath);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_fpsCounter.Update();
			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false) / 1024);
			_fpsLabel.Text = string.Format("FPS: {0:0.##}", _fpsCounter.FPS);
			_widgetsCountLabel.Text = string.Format("Total Widgets: {0}", _desktop.CalculateTotalWidgets());

			GraphicsDevice.Clear(Color.Black);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render(GraphicsDevice);

			_drawCallsLabel.Text = string.Format("Draw Calls: {0}", GraphicsDevice.Metrics.DrawCount);
		}

		protected override void EndRun()
		{
			base.EndRun();
		
			var state = new State
			{
				Size = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight),
				TopSplitterPosition = _topSplitPane.GetSplitterPosition(0),
				RightSplitterPosition = _rightSplitPane.GetSplitterPosition(0),
				EditedFile = FilePath,
				CustomColors = _customColors
			};

			state.Save();
		}

		private void New()
		{
			Project = new Grid
			{
				Id = "New Form"
			};

			FilePath = string.Empty;
			IsDirty = false;
		}

		private void Save(bool setFileName)
		{
			var filePath = FilePath;
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				using (var dlg = new SaveFileDialog())
				{
					dlg.Filter = PathFilter;
					if (dlg.ShowDialog() == DialogResult.OK)
					{
						filePath = dlg.FileName;
					}
					else
					{
						return;
					}
				}
			}

			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			var data = Serialization.Save(_project);
			File.WriteAllText(filePath, data);

			FilePath = filePath;
			IsDirty = false;
		}

		private void Load(string filePath)
		{
			try
			{
				var data = File.ReadAllText(filePath);
				var project = Serialization.LoadFromData(data);
				Project = project;
				FilePath = filePath;
				IsDirty = false;
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}

		private void UpdateTitle()
		{
			var title = string.IsNullOrEmpty(_filePath) ? "Myra UI Editor" : _filePath;

			if (_isDirty)
			{
				title += " *";
			}

			Window.Title = title;
		}
	}
}