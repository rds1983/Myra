using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Myra.Editor.Plugin;
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
		private const string PathFilter = "Myra UIEditor Projects (*.ui)|*.ui";

		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private static Studio _instance;

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
		private ScrollPane _propertyGridPane; 
		private PropertyGrid _propertyGrid;
		private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
		private string _filePath;
		private bool _isDirty;
		private Project _project;
		private int[] _customColors;

		private MenuItem _addButtonItem,
			_addCheckBoxItem,
			_addImageButtonItem,
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
			_addHorizontalProgressBarItem,
			_addVerticalProgressBarItem,
			_addTextBlockItem,
			_addTextFieldItem,
			_addSpinButtonItem,
//			_addTreeItem,
			_addMenuItemItem,
			_addMenuSeparatorItem,
//			_addTreeNodeItem,
			_deleteItem;

		private Type[] _customWidgetTypes;
		private MenuItem[] _customWidgetMenuItems;
		
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
			_instance = this;

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

			MyraEnvironment.Game = this;

			ProcessPlugin();
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

		private void ProcessPlugin()
		{
			var pluginPath = Configuration.PluginPath;
			if (string.IsNullOrEmpty(pluginPath))
			{
				_logger.Info("Plugin path is not set");
				return;
			}

			if (!Path.IsPathRooted(pluginPath))
			{
				// Add folder path
				var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

				pluginPath = Path.Combine(path, pluginPath);
			}

			_logger.Info("Trying to load plugin: " + pluginPath);

			try
			{
				var pluginAssembly = Assembly.LoadFile(pluginPath);

				// Find implementation of IUIEditorPlugin
				foreach (var c in pluginAssembly.GetExportedTypes())
				{
					if (typeof (IUIEditorPlugin).IsAssignableFrom(c))
					{
						// Found
						// Instantiate
						var plugin = (IUIEditorPlugin) Activator.CreateInstance(c);

						// Call on load
						plugin.OnLoad();
						
						var customWidgets = new WidgetTypesSet();
						plugin.FillCustomWidgetTypes(customWidgets);

						_customWidgetTypes = customWidgets.Types;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}

		private void BuildUI()
		{
#if DEBUG
//			BitmapFont.DrawFames = true;
//			Widget.DrawFrames = true;
			Widget.DrawFocused = true;
#endif

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
			
			var exportCSItem = new MenuItem
			{
				Text = "Export to C#..."
			};
			exportCSItem.Selected += ExportCsItemOnSelected;

			fileMenu.Items.Add(exportCSItem);	
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

			_addImageButtonItem = new MenuItem
			{
				Text = "Add ImageButton"
			};
			_addImageButtonItem.Selected += (s, a) =>
			{
				AddStandardControl<ImageButton>();
			};
			controlsMenu.Items.Add(_addImageButtonItem);

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

			_addHorizontalProgressBarItem = new MenuItem
			{
				Text = "Add Horizontal ProgressBar"
			};
			_addHorizontalProgressBarItem.Selected += (s, a) =>
			{
				AddStandardControl<HorizontalProgressBar>();
			};
			controlsMenu.Items.Add(_addHorizontalProgressBarItem);

			_addVerticalProgressBarItem = new MenuItem
			{
				Text = "Add Vertical ProgressBar"
			};
			_addVerticalProgressBarItem.Selected += (s, a) =>
			{
				AddStandardControl<VerticalProgressBar>();
			};
			controlsMenu.Items.Add(_addVerticalProgressBarItem);

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
				AddStandardControl<ScrollPane>();
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

			_addSpinButtonItem = new MenuItem
			{
				Text = "Add SpinButton"
			};
			_addSpinButtonItem.Selected += (s, a) =>
			{
				AddStandardControl<SpinButton>();
			};
			controlsMenu.Items.Add(_addSpinButtonItem);

/*			_addTreeItem = new MenuItem
			{
				Text = "Add Tree"
			};
			_addTreeItem.Selected += (s, a) =>
			{
				AddStandardControl<Tree>();
			};
			controlsMenu.Items.Add(_addTreeItem);*/
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
			
			if (_customWidgetTypes != null && _customWidgetTypes.Length > 0)
			{
				controlsMenu.Items.Add(new MenuSeparator());

				var customMenuWidgets = new List<MenuItem>();
				foreach (var type in _customWidgetTypes)
				{
					var item = new MenuItem
					{
						Text = "Add " + type.Name
					};
								
					item.Selected += (s, a) =>
					{
						AddStandardControl(type);
					};
					
					controlsMenu.Items.Add(item);
					
					customMenuWidgets.Add(item);
				}

				_customWidgetMenuItems = customMenuWidgets.ToArray();
			}

			controlsMenu.Items.Add(new MenuSeparator());
		
/*			_addTreeNodeItem = new MenuItem
			{
				Text = "Add Tree Node"
			};
			controlsMenu.Items.Add(_addTreeNodeItem);
			controlsMenu.Items.Add(new MenuSeparator());*/

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
			_explorer.Tree.SelectionChanged += WidgetOnSelectionChanged;
			_rightSplitPane.Widgets.Add(_explorer);

			_propertyGridPane = new ScrollPane();

			_propertyGrid = new PropertyGrid();
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;
			_propertyGrid.ColorChangeHandler += ColorChangeHandler;

			_propertyGridPane.Widget = _propertyGrid;

			_rightSplitPane.Widgets.Add(_propertyGridPane);

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

		private void ExportCsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new ExportOptionsDialog();
			dlg.ShowModal(_desktop);
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
					var container = _explorer.SelectedNode.ParentNode.Tag;
					if (container is SplitPane)
					{
						((SplitPane)container).Widgets.Remove(asWidget);
					}
					else if (container is Grid)
					{
						((Grid)container).Widgets.Remove(asWidget);
					} else if (container is ScrollPane)
					{
						((ScrollPane) container).Widget = null;
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
			var node = _explorer.AddObject(_explorer.Tree.SelectedRow, widget);

			_explorer.Tree.SelectedRow = node;
			_explorer.Tree.ExpandPath(node);
		}

		private void AddStandardControl<T>(T widget) where T : Widget
		{
			if (_propertyGrid.Object is Grid)
			{
				((Grid) _propertyGrid.Object).Widgets.Add(widget);
			}
			else if (_propertyGrid.Object is SplitPane)
			{
				((SplitPane) _propertyGrid.Object).Widgets.Add(widget);
			} else if (_propertyGrid.Object is ScrollPane)
			{
				((ScrollPane) _propertyGrid.Object).Widget = widget;
			}

			OnObjectAdded(widget);
		}

		private void AddStandardControl<T>() where T : Widget, new()
		{
			AddStandardControl(typeof(T));
		}
		
		private void AddStandardControl(Type t)
		{
			var control = (Widget)Activator.CreateInstance(t);
			AddStandardControl(control);
		}

		private void PropertyGridOnPropertyChanged(object sender, GenericEventArgs<string> eventArgs)
		{
			if (eventArgs.Data == "Id" || eventArgs.Data == "Text")
			{
				_explorer.OnObjectIdChanged(_explorer.Tree.SelectedRow.Tag);
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
			_propertyGridPane.ResetScroll();

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

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_fpsCounter.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false)/1024);
			_fpsLabel.Text = string.Format("FPS: {0}", _fpsCounter.FramesPerSecond);
			_widgetsCountLabel.Text = string.Format("Visible Widgets: {0}", _desktop.CalculateTotalWidgets(true));

			GraphicsDevice.Clear(Color.Black);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

			_drawCallsLabel.Text = string.Format("Draw Calls: {0}", GraphicsDevice.Metrics.DrawCount);

			_fpsCounter.Draw(gameTime);
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

		    var data = _project.Save();
			File.WriteAllText(filePath, data);

			FilePath = filePath;
			IsDirty = false;
		}

		private void Load(string filePath)
		{
			try
			{
				var data = File.ReadAllText(filePath);
				var project = Project.LoadFromData(data);
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
				else if (selectedObject is ScrollPane && ((ScrollPane) selectedObject).Widget == null)
				{
					enableStandard = true;
				} 
			}

			_addButtonItem.Enabled = enableStandard;
			_addCheckBoxItem.Enabled = enableStandard;
			_addImageButtonItem.Enabled = enableStandard;
			_addHorizontalSliderItem.Enabled = enableStandard;
			_addVerticalSliderItem.Enabled = enableStandard;
			_addHorizontalProgressBarItem.Enabled = enableStandard;
			_addVerticalProgressBarItem.Enabled = enableStandard;
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
			_addSpinButtonItem.Enabled = enableStandard;
//			_addTreeItem.Enabled = enableStandard;

			if (_customWidgetMenuItems != null)
			{
				foreach (var item in _customWidgetMenuItems)
				{
					item.Enabled = enableStandard;
				}
			}

			_addMenuItemItem.Enabled = enableMenuItems;
			_addMenuSeparatorItem.Enabled = enableMenuItems;

//			_addTreeNodeItem.Enabled = enableTreeNode;

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