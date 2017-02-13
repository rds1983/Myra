using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.UIEditor.UI;
using Myra.UIEditor.Utils;
using Myra.Utility;
using NLog;
using Button = Myra.Graphics2D.UI.Button;
using CheckBox = Myra.Graphics2D.UI.CheckBox;
using Color = Microsoft.Xna.Framework.Color;
using ComboBox = Myra.Graphics2D.UI.ComboBox;
using HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment;
using ListBox = Myra.Graphics2D.UI.ListBox;
using Menu = Myra.Graphics2D.UI.Menu;
using MenuItem = Myra.Graphics2D.UI.MenuItem;
using Point = Microsoft.Xna.Framework.Point;
using PropertyGrid = Myra.Editor.UI.PropertyGrid;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Myra.UIEditor
{
	public class Studio : Game
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
		private Project _project;
		private int[] _customColors;

		private MenuItem _addButtonItem,
			_addCheckBoxItem,
			_addHorizontalSliderItem,
			_addVerticalSliderItem,
			_addComboBoxItem,
			_addListBoxItem,
			_addGridItem,
			_addImageItem,
			_addHorizontalMenuItem,
			_addVerticalMenuItem,
			_addScrollPaneItem,
			_addHorizontalSplitPaneItem,
			_addVerticalSplitPaneItem,
			_addTextBlockItem,
			_addTextFieldItem,
			_addTreeItem,
			_addMenuItemItem,
			_addMenuSeparatorItem,
			_addTreeNodeItem,
			_deleteItem;

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
				_explorer.Project = _project;

				_projectHolder.Widgets.Clear();

				if (_project != null)
				{
					_projectHolder.Widgets.Add(_project.Root);
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

			var menuBar = new HorizontalMenu();

			var fileMenu = new MenuItem
			{
				Text = "File"
			};

			var newItem = new MenuItem
			{
				Text = "New"
			};
			newItem.Selected += NewItemOnClicked;
			fileMenu.Items.Add(newItem);

			var openItem = new MenuItem
			{
				Text = "Open.."
			};
			openItem.Selected += OpenItemOnClicked;
			fileMenu.Items.Add(openItem);

			var saveItem = new MenuItem
			{
				Text = "Save"
			};
			saveItem.Selected += SaveItemOnClicked;
			fileMenu.Items.Add(saveItem);

			var saveAsItem = new MenuItem
			{
				Text = "Save As..."
			};
			saveAsItem.Selected += SaveAsItemOnClicked;

			fileMenu.Items.Add(saveAsItem);
			fileMenu.Items.Add(new MenuSeparator());

			var quitItem = new MenuItem
			{
				Text = "Quit"
			};
			quitItem.Selected += QuitItemOnDown;
			fileMenu.Items.Add(quitItem);

			menuBar.Items.Add(fileMenu);

			var controlsMenu = new MenuItem
			{
				Text = "Controls"
			};

			_addButtonItem = new MenuItem
			{
				Text = "Add Button"
			};

			_addButtonItem.Selected += (s, a) =>
			{
				AddStandardControl<Button>();
			};
			controlsMenu.Items.Add(_addButtonItem);

			_addCheckBoxItem = new MenuItem
			{
				Text = "Add CheckBox"
			};
			_addCheckBoxItem.Selected += (s, a) =>
			{
				AddStandardControl<CheckBox>();
			};
			controlsMenu.Items.Add(_addCheckBoxItem);

			_addHorizontalSliderItem = new MenuItem
			{
				Text = "Add Horizontal Slider"
			};
			_addHorizontalSliderItem.Selected += (s, a) =>
			{
				AddStandardControl<HorizontalSlider>();
			};
			controlsMenu.Items.Add(_addHorizontalSliderItem);

			_addVerticalSliderItem = new MenuItem
			{
				Text = "Add Vertical Slider"
			};
			_addVerticalSliderItem.Selected += (s, a) =>
			{
				AddStandardControl<VerticalSlider>();
			};
			controlsMenu.Items.Add(_addVerticalSliderItem);

			_addComboBoxItem = new MenuItem
			{
				Text = "Add ComboBox"
			};
			_addComboBoxItem.Selected += (s, a) =>
			{
				AddStandardControl<ComboBox>();
			};
			controlsMenu.Items.Add(_addComboBoxItem);

			_addListBoxItem = new MenuItem
			{
				Text = "Add ListBox"
			};
			_addListBoxItem.Selected += (s, a) =>
			{
				AddStandardControl<ListBox>();
			};
			controlsMenu.Items.Add(_addListBoxItem);

			_addGridItem = new MenuItem
			{
				Text = "Add Grid"
			};
			_addGridItem.Selected += (s, a) =>
			{
				AddStandardControl<Grid>();
			};
			controlsMenu.Items.Add(_addGridItem);

			_addImageItem = new MenuItem
			{
				Text = "Add Image"
			};
			_addImageItem.Selected += (s, a) =>
			{
				AddStandardControl<Image>();
			};
			controlsMenu.Items.Add(_addImageItem);

			_addHorizontalMenuItem = new MenuItem
			{
				Text = "Add Horizontal Menu"
			};
			_addHorizontalMenuItem.Selected += (s, a) =>
			{
				AddStandardControl(new HorizontalMenu());
			};
			controlsMenu.Items.Add(_addHorizontalMenuItem);

			_addVerticalMenuItem = new MenuItem
			{
				Text = "Add Vertical Menu"
			};
			_addVerticalMenuItem.Selected += (s, a) =>
			{
				AddStandardControl(new VerticalMenu());
			};
			controlsMenu.Items.Add(_addVerticalMenuItem);

			_addScrollPaneItem = new MenuItem
			{
				Text = "Add ScrollPane"
			};
			_addScrollPaneItem.Selected += (s, a) =>
			{
				AddStandardControl<ScrollPane<Widget>>();
			};
			controlsMenu.Items.Add(_addScrollPaneItem);

			_addHorizontalSplitPaneItem = new MenuItem
			{
				Text = "Add Horizontal SplitPane"
			};
			_addHorizontalSplitPaneItem.Selected += (s, a) =>
			{
				AddStandardControl(new HorizontalSplitPane());
			};
			controlsMenu.Items.Add(_addHorizontalSplitPaneItem);

			_addVerticalSplitPaneItem = new MenuItem
			{
				Text = "Add Vertical SplitPane"
			};
			_addVerticalSplitPaneItem.Selected += (s, a) =>
			{
				AddStandardControl(new VerticalSplitPane());
			};
			controlsMenu.Items.Add(_addVerticalSplitPaneItem);

			_addTextBlockItem = new MenuItem
			{
				Text = "Add TextBlock"
			};
			_addTextBlockItem.Selected += (s, a) =>
			{
				AddStandardControl<TextBlock>();
			};
			controlsMenu.Items.Add(_addTextBlockItem);

			_addTextFieldItem = new MenuItem
			{
				Text = "Add TextField"
			};
			_addTextFieldItem.Selected += (s, a) =>
			{
				AddStandardControl<TextField>();
			};
			controlsMenu.Items.Add(_addTextFieldItem);

			_addTreeItem = new MenuItem
			{
				Text = "Add Tree"
			};
			_addTreeItem.Selected += (s, a) =>
			{
				AddStandardControl<Tree>();
			};
			controlsMenu.Items.Add(_addTreeItem);
			controlsMenu.Items.Add(new MenuSeparator());

			_addMenuItemItem = new MenuItem
			{
				Text = "Add Menu Item"
			};
			_addMenuItemItem.Selected += (s, a) =>
			{
				AddMenuItem(new MenuItem());
			};
			controlsMenu.Items.Add(_addMenuItemItem);

			_addMenuSeparatorItem = new MenuItem
			{
				Text = "Add Menu Separator"
			};
			_addMenuSeparatorItem.Selected += (s, a) =>
			{
				AddMenuItem(new MenuSeparator());
			};
			controlsMenu.Items.Add(_addMenuSeparatorItem);
			controlsMenu.Items.Add(new MenuSeparator());

			_addTreeNodeItem = new MenuItem
			{
				Text = "Add Tree Node"
			};
			controlsMenu.Items.Add(_addTreeNodeItem);
			controlsMenu.Items.Add(new MenuSeparator());

			_deleteItem = new MenuItem
			{
				Text = "Delete"
			};
			_deleteItem.Selected += DeleteItemOnSelected;

			controlsMenu.Items.Add(_deleteItem);

			menuBar.Items.Add(controlsMenu);

			var helpMenu = new MenuItem
			{
				Text = "Help"
			};

			var aboutItem = new MenuItem
			{
				Text = "About"
			};
			aboutItem.Selected += AboutItemOnClicked;
			helpMenu.Items.Add(aboutItem);
			menuBar.Items.Add(helpMenu);

			_topSplitPane = new HorizontalSplitPane
			{
				Id = "topSplitPane",
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				GridPositionY = 1
			};

			_projectHolder = new Grid();

			_topSplitPane.Widgets.Add(_projectHolder);

			_rightSplitPane = new VerticalSplitPane();

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

			root.Widgets.Add(menuBar);
			root.Widgets.Add(_topSplitPane);

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

			_drawCallsLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridPositionY = 3
			};

			_statisticsGrid.Widgets.Add(_drawCallsLabel);

			_statisticsGrid.HorizontalAlignment = HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.XHint = 10;
			_statisticsGrid.YHint = -10;
			_desktop.Widgets.Add(_statisticsGrid);

			UpdateEnabled();
		}

		private void DeleteItemOnSelected(object sender, EventArgs eventArgs)
		{
			// Remove from model
			var asMenuItem = _propertyGrid.Object as IMenuItem;
			if (asMenuItem != null)
			{
				asMenuItem.Menu.Items.Remove(asMenuItem);
			}
			else
			{
				var asWidget = _propertyGrid.Object as Widget;
				if (asWidget != null)
				{
					var asSplitPane = _explorer.SelectedNode.ParentNode.Tag as SplitPane;
					if (asSplitPane != null)
					{
						asSplitPane.Widgets.Remove(asWidget);
					}
					else
					{
						((Grid) asWidget.Parent).Widgets.Remove(asWidget);
					}
				}
			}

			// Remove from tree
			_explorer.SelectedNode.ParentNode.RemoveSubNode(_explorer.SelectedNode);

			IsDirty = true;
		}

		private void AddMenuItem(IMenuItem iMenuItem)
		{
			var menuItem = iMenuItem as MenuItem;
			if (menuItem != null)
			{
				menuItem.Changed += (sender, args) =>
				{
					_explorer.OnObjectIdChanged(iMenuItem);
				};
			}

			if (_propertyGrid.Object is Menu)
			{
				((Menu) _propertyGrid.Object).Items.Add(iMenuItem);
			}
			else if (_propertyGrid.Object is MenuItem)
			{
				((MenuItem) _propertyGrid.Object).Items.Add(iMenuItem);
			}

			OnObjectAdded(iMenuItem);
		}

		private void OnObjectAdded(object widget)
		{
			var node = _explorer.AddObject(_explorer.Widget.SelectedRow, widget);

			_explorer.Widget.SelectedRow = node;
			_explorer.Widget.ExpandPath(node);
		}

		private void AddStandardControl<T>(T widget) where T : Widget
		{
			IList<Widget> container;
			var asGrid = _propertyGrid.Object as Grid;
			if (asGrid != null)
			{
				container = asGrid.Widgets;
			}
			else
			{
				container = ((SplitPane) _propertyGrid.Object).Widgets;
			}

			container.Add(widget);

			OnObjectAdded(widget);
		}

		private void AddStandardControl<T>() where T : Widget, new()
		{
			var control = new T();
			AddStandardControl(control);
		}

		private void PropertyGridOnPropertyChanged(object sender, GenericEventArgs<string> eventArgs)
		{
			if (eventArgs.Data == "Id" || eventArgs.Data == "Text")
			{
				_explorer.OnObjectIdChanged(_explorer.Widget.SelectedRow.Tag);
			}

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
			_propertyGrid.Object = _explorer.SelectedObject;

			UpdateEnabled();
		}

		private void QuitItemOnDown(object sender, EventArgs eventArgs)
		{
			var mb = Dialog.CreateMessageBox("Quit", "Are you sure?");

			mb.Closed += (o, args) =>
			{
				if (mb.ModalResult == (int) Graphics2D.UI.Window.DefaultModalResult.Ok)
				{
					Exit();
				}
			};

			mb.ShowModal(_desktop);
		}

		private void AboutItemOnClicked(object sender, EventArgs eventArgs)
		{
			var messageBox = Dialog.CreateMessageBox("About", "Myra UI Editor " + MyraEnvironment.Version);
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
			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false)/1024);
			_fpsLabel.Text = string.Format("FPS: {0:0.##}", _fpsCounter.FPS);
			_widgetsCountLabel.Text = string.Format("Visible Widgets: {0}", _desktop.CalculateTotalWidgets(true));

			GraphicsDevice.Clear(Color.Black);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

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
			var project = new Project
			{
				Root =
				{
					Id = "Root"
				}
			};

			Project = project;

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

		private void UpdateEnabled()
		{
			var enableStandard = false;
			var enableMenuItems = false;
			var enableTreeNode = false;

			var selectedObject = _propertyGrid.Object;
			if (selectedObject != null)
			{
				if (selectedObject is Menu || 
					selectedObject is MenuItem)
				{
					enableMenuItems = true;
				}
				else if (selectedObject is Tree)
				{
					enableTreeNode = true;
				}
				else if (selectedObject is Grid || selectedObject is SplitPane)
				{
					enableStandard = true;
				}
			}

			_addButtonItem.Enabled = enableStandard;
			_addCheckBoxItem.Enabled = enableStandard;
			_addHorizontalSliderItem.Enabled = enableStandard;
			_addVerticalSliderItem.Enabled = enableStandard;
			_addComboBoxItem.Enabled = enableStandard;
			_addListBoxItem.Enabled = enableStandard;
			_addGridItem.Enabled = enableStandard;
			_addImageItem.Enabled = enableStandard;
			_addHorizontalMenuItem.Enabled = enableStandard;
			_addVerticalMenuItem.Enabled = enableStandard;
			_addScrollPaneItem.Enabled = enableStandard;
			_addHorizontalSplitPaneItem.Enabled = enableStandard;
			_addVerticalSplitPaneItem.Enabled = enableStandard;
			_addTextBlockItem.Enabled = enableStandard;
			_addTextFieldItem.Enabled = enableStandard;
			_addTreeItem.Enabled = enableStandard;

			_addMenuItemItem.Enabled = enableMenuItems;
			_addMenuSeparatorItem.Enabled = enableMenuItems;

			_addTreeNodeItem.Enabled = enableTreeNode;

			if (selectedObject is IMenuItem ||
				selectedObject is Widget)
			{
				_deleteItem.Enabled = true;
				_deleteItem.Text = "Delete " + _explorer.SelectedNode.Text;
			}
			else
			{
				_deleteItem.Enabled = false;
			}
		}
	}
}