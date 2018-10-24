using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Myra.Editor.Plugin;
using Myra.Editor.UI.File;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.UIEditor.UI;
using Myra.Utility;
using Button = Myra.Graphics2D.UI.Button;
using CheckBox = Myra.Graphics2D.UI.CheckBox;
using Color = Microsoft.Xna.Framework.Color;
using ComboBox = Myra.Graphics2D.UI.ComboBox;
using FileDialog = Myra.Editor.UI.File.FileDialog;
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
		private static Studio _instance;

		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private readonly State _state;
		private Desktop _desktop;
		private StudioWidget _ui;
		private MenuItem _deleteItem;
		private Explorer _explorer;
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
		private Type[] _customWidgetTypes;
		private MenuItem[] _customWidgetMenuItems;
		private bool _isWindow = false;

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

		private void ProcessPlugin()
		{
			var pluginPath = Configuration.PluginPath;
			if (string.IsNullOrEmpty(pluginPath))
			{
				return;
			}

			if (!Path.IsPathRooted(pluginPath))
			{
				// Add folder path
				var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

				pluginPath = Path.Combine(path, pluginPath);
			}

			try
			{
				var pluginAssembly = Assembly.LoadFile(pluginPath);

				// Find implementation of IUIEditorPlugin
				foreach (var c in pluginAssembly.GetExportedTypes())
				{
					if (typeof(IUIEditorPlugin).IsAssignableFrom(c))
					{
						// Found
						// Instantiate
						var plugin = (IUIEditorPlugin)Activator.CreateInstance(c);

						// Call on load
						plugin.OnLoad();

						var customWidgets = new WidgetTypesSet();
						plugin.FillCustomWidgetTypes(customWidgets);

						_customWidgetTypes = customWidgets.Types;
						break;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void BuildUI()
		{
			_desktop = new Desktop();

			_ui = new StudioWidget();

			_ui._menuFileNew.Selected += NewItemOnClicked;
			_ui._menuFileOpen.Selected += OpenItemOnClicked;
			_ui._menuFileSave.Selected += SaveItemOnClicked;
			_ui._menuFileSaveAs.Selected += SaveAsItemOnClicked;
			_ui._menuFileExportToCS.Selected += ExportCsItemOnSelected;
			_ui._menuFileDebugOptions.Selected += DebugOptionsItemOnSelected;
			_ui._menuFileQuit.Selected += QuitItemOnDown;

			_ui._menuControlsAddButton.Selected += (s, a) =>
			{
				AddStandardControl<Button>();
			};
			_ui._menuControlsAddCheckBox.Selected += (s, a) =>
			{
				AddStandardControl<CheckBox>();
			};
			_ui._menuControlsAddImageButton.Selected += (s, a) =>
			{
				AddStandardControl<ImageButton>();
			};
			_ui._menuControlsAddTextButton.Selected += (s, a) =>
			{
				AddStandardControl<TextButton>();
			};
			_ui._menuControlsAddHorizontalSlider.Selected += (s, a) =>
			{
				AddStandardControl<HorizontalSlider>();
			};
			_ui._menuControlsAddVerticalSlider.Selected += (s, a) =>
			{
				AddStandardControl<VerticalSlider>();
			};
			_ui._menuControlsAddHorizontalProgressBar.Selected += (s, a) =>
			{
				AddStandardControl<HorizontalProgressBar>();
			};
			_ui._menuControlsAddVerticalProgressBar.Selected += (s, a) =>
			{
				AddStandardControl<VerticalProgressBar>();
			};
			_ui._menuControlsAddHorizontalSeparator.Selected += (s, a) =>
			{
				AddStandardControl<HorizontalSeparator>();
			};
			_ui._menuControlsAddVerticalSeparator.Selected += (s, a) =>
			{
				AddStandardControl<VerticalSeparator>();
			};
			_ui._menuControlsAddComboBox.Selected += (s, a) =>
			{
				AddStandardControl<ComboBox>();
			};
			_ui._menuControlsAddListBox.Selected += (s, a) =>
			{
				AddStandardControl<ListBox>();
			};
			_ui._menuControlsAddPanel.Selected += (s, a) =>
			{
				AddStandardControl<Myra.Graphics2D.UI.Panel>();
			};
			_ui._menuControlsAddGrid.Selected += (s, a) =>
			{
				AddStandardControl<Grid>();
			};
			_ui._menuControlsAddImage.Selected += (s, a) =>
			{
				AddStandardControl<Image>();
			};
			_ui._menuControlsAddHorizontalMenu.Selected += (s, a) =>
			{
				AddStandardControl(new HorizontalMenu());
			};
			_ui._menuControlsAddVerticalMenu.Selected += (s, a) =>
			{
				AddStandardControl(new VerticalMenu());
			};
			_ui._menuControlsAddScrollPane.Selected += (s, a) =>
			{
				AddStandardControl<ScrollPane>();
			};
			_ui._menuControlsAddHorizontalSplitPane.Selected += (s, a) =>
			{
				AddStandardControl(new HorizontalSplitPane());
			};
			_ui._menuControlsAddVerticalSplitPane.Selected += (s, a) =>
			{
				AddStandardControl(new VerticalSplitPane());
			};
			_ui._menuControlsAddTextBlock.Selected += (s, a) =>
			{
				AddStandardControl<TextBlock>();
			};
			_ui._menuControlsAddTextField.Selected += (s, a) =>
			{
				AddStandardControl<TextField>();
			};
			_ui._menuControlsAddSpinButton.Selected += (s, a) =>
			{
				AddStandardControl<SpinButton>();
			};
			_ui._menuControlsAddMenuItem.Selected += (s, a) =>
			{
				AddMenuItem(new MenuItem());
			};
			_ui._menuControlsAddDialog.Selected += (s, a) =>
			{
				AddStandardControl<Dialog>();
			}; _ui._menuControlsAddWindow.Selected += (s, a) =>
			{
				AddStandardControl<Window>();
			};
			_ui._menuControlsAddMenuSeparator.Selected += (s, a) =>
			{
				AddMenuItem(new MenuSeparator());
			};

			if (_customWidgetTypes != null && _customWidgetTypes.Length > 0)
			{
				_ui._menuControls.Items.Add(new MenuSeparator());

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

					_ui._menuControls.Items.Add(item);

					customMenuWidgets.Add(item);
				}

				_customWidgetMenuItems = customMenuWidgets.ToArray();
			}

			_ui._menuControls.Items.Add(new MenuSeparator());

			_deleteItem = new MenuItem
			{
				Text = "Delete"
			};
			_deleteItem.Selected += DeleteItemOnSelected;

			_ui._menuControls.Items.Add(_deleteItem);

			_ui._menuHelpAbout.Selected += AboutItemOnClicked;

			_explorer = new Explorer();
			_explorer.Tree.SelectionChanged += WidgetOnSelectionChanged;
			_ui._explorerHolder.Widgets.Add(_explorer);

			_propertyGrid = new PropertyGrid();
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;

			_ui._propertyGridPane.Widget = _propertyGrid;

			_ui._topSplitPane.SetSplitterPosition(0, _state != null ? _state.TopSplitterPosition : 0.75f);
			_ui._rightSplitPane.SetSplitterPosition(0, _state != null ? _state.RightSplitterPosition : 0.5f);

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
					if (container is Window)
					{
						((Window)container).Content = null;
					}
					else if (container is SplitPane)
					{
						((SplitPane)container).Widgets.Remove(asWidget);
					}
					else if (container is MultipleItemsContainer)
					{
						((MultipleItemsContainer)container).Widgets.Remove(asWidget);
					}
					else if (container is ScrollPane)
					{
						((ScrollPane)container).Widget = null;
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
				((Menu)_propertyGrid.Object).Items.Add(iMenuItem);
			}
			else if (_propertyGrid.Object is MenuItem)
			{
				((MenuItem)_propertyGrid.Object).Items.Add(iMenuItem);
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
			var root = _explorer.SelectedObject;

			if (root is Window)
			{
				((Window)root).Content = widget;
			}
			else if (root is MultipleItemsContainer)
			{
				((MultipleItemsContainer)root).Widgets.Add(widget);
			}
			else if (root is SplitPane)
			{
				((SplitPane)root).Widgets.Add(widget);
			}
			else if (root is ScrollPane)
			{
				((ScrollPane)root).Widget = widget;
			}
			else if (root is Project)
			{
				((Project)root).Root = widget;

				if (widget != null)
				{
					_ui._projectHolder.Widgets.Add(widget);
				}
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

		private void WidgetOnSelectionChanged(object sender, EventArgs eventArgs)
		{
			_propertyGrid.Object = _explorer.SelectedObject is Project ? null : _explorer.SelectedObject;

			UpdateEnabled();
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
			var dlg = new FileDialog(FileDialogMode.OpenFile)
			{
				Filter = "*.ui"
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

			if (_isWindow)
			{
				((Window)Project.Root).CenterInBounds(_ui._projectHolder.Bounds);

				_isWindow = false;
			}

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
				RightSplitterPosition = _ui._rightSplitPane.GetSplitterPosition(0),
				EditedFile = FilePath,
				LastFolder = _lastFolder,
				UserColors = (from c in ColorPickerDialog.UserColors select c.PackedValue).ToArray()
			};

			state.Save();
		}

		private void New()
		{
			var project = new Project();

			Project = project;

			FilePath = string.Empty;
			IsDirty = false;
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
					Filter = "*.ui"
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
				var project = Project.LoadFromData(data);
				Project = project;
				FilePath = filePath;
				IsDirty = false;

				if (Project != null && Project.Root is Window)
				{
					_isWindow = true;
				}
			}
			catch (Exception ex)
			{
				var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
				dialog.ShowModal(_desktop);
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
			var enableContainers = false;
			var enableMenuItems = false;
			var enableTreeNode = false;
			var enableWindows = false;

			var selectedObject = _explorer.SelectedObject;
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
				else if (selectedObject is MultipleItemsContainer || selectedObject is SplitPane)
				{
					enableStandard = true;
				}
				else if (selectedObject is ScrollPane && ((ScrollPane)selectedObject).Widget == null)
				{
					enableStandard = true;
				}
				else if (selectedObject is Project && ((Project)selectedObject).Root == null)
				{
					enableContainers = true;
					enableWindows = true;
				}
			}

			_ui._menuControlsAddButton.Enabled = enableStandard;
			_ui._menuControlsAddCheckBox.Enabled = enableStandard;
			_ui._menuControlsAddImageButton.Enabled = enableStandard;
			_ui._menuControlsAddTextButton.Enabled = enableStandard;
			_ui._menuControlsAddHorizontalSlider.Enabled = enableStandard;
			_ui._menuControlsAddVerticalSlider.Enabled = enableStandard;
			_ui._menuControlsAddHorizontalProgressBar.Enabled = enableStandard;
			_ui._menuControlsAddVerticalProgressBar.Enabled = enableStandard;
			_ui._menuControlsAddHorizontalSeparator.Enabled = enableStandard;
			_ui._menuControlsAddVerticalSeparator.Enabled = enableStandard;
			_ui._menuControlsAddComboBox.Enabled = enableStandard;
			_ui._menuControlsAddListBox.Enabled = enableStandard;
			_ui._menuControlsAddImage.Enabled = enableStandard;
			_ui._menuControlsAddHorizontalMenu.Enabled = enableStandard;
			_ui._menuControlsAddVerticalMenu.Enabled = enableStandard;
			_ui._menuControlsAddTextBlock.Enabled = enableStandard;
			_ui._menuControlsAddTextField.Enabled = enableStandard;
			_ui._menuControlsAddSpinButton.Enabled = enableStandard;

			// Containers
			_ui._menuControlsAddPanel.Enabled = enableStandard || enableContainers;
			_ui._menuControlsAddGrid.Enabled = enableStandard || enableContainers;
			_ui._menuControlsAddHorizontalSplitPane.Enabled = enableStandard || enableContainers;
			_ui._menuControlsAddVerticalSplitPane.Enabled = enableStandard || enableContainers;
			_ui._menuControlsAddScrollPane.Enabled = enableStandard || enableContainers;

			_ui._menuControlsAddDialog.Enabled = enableWindows;
			_ui._menuControlsAddWindow.Enabled = enableWindows;

			if (_customWidgetMenuItems != null)
			{
				foreach (var item in _customWidgetMenuItems)
				{
					item.Enabled = enableStandard;
				}
			}

			_ui._menuControlsAddMenuItem.Enabled = enableMenuItems;
			_ui._menuControlsAddMenuSeparator.Enabled = enableMenuItems;

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