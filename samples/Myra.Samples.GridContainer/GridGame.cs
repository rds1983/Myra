using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Myra.Samples.GridContainer
{
	public class GridGame : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private Window _window;
		private HorizontalProgressBar _horizontalProgressBar;
		private VerticalProgressBar _verticalProgressBar;

		public GridGame()
		{
			graphics = new GraphicsDeviceManager(this);

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			// Widget.DrawFrames = true;
			var grid = new Grid
			{
				RowSpacing = 3,
				ColumnSpacing = 3,
				//				DrawLines = true
			};

			grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1.0f));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 2.0f));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, 150.0f));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1.0f));
			grid.RowsProportions.Add(new Proportion(ProportionType.Part, 1.0f));
			grid.RowsProportions.Add(new Proportion(ProportionType.Pixels, 200.0f));
			grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			grid.RowsProportions.Add(new Proportion(ProportionType.Fill));

			// Create headers
			for (var i = 1; i < grid.ColumnsProportions.Count; ++i)
			{
				var header = new Label
				{
					Text = grid.ColumnsProportions[i].ToString(),
					GridColumn = i,
					GridRow = 0
				};

				grid.Widgets.Add(header);
			}

			// Combo
			var combo = new ComboBox
			{
				GridColumn = 1,
				GridRow = 1
			};

			combo.Items.Add(new ListItem("Red", Color.Red));
			combo.Items.Add(new ListItem("Green", Color.Green));
			combo.Items.Add(new ListItem("Blue", Color.Blue));
			grid.Widgets.Add(combo);

			// Button
			var button = new ImageTextButton
			{
				GridColumn = 2,
				GridRow = 1,
				GridColumnSpan = 2,
				GridRowSpan = 1,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Text = "This is 2 columns button"
			};

			button.Click += (s, a) =>
			{
				var messageBox = Dialog.CreateMessageBox("2C", "2 Columns Button pushed!");
				messageBox.ShowModal();
			};

			grid.Widgets.Add(button);

			// Button
			var button2 = new TextButton
			{
				GridColumn = 2,
				GridRow = 2,
				GridColumnSpan = 1,
				GridRowSpan = 2,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Text = "This is 2 rows button"
			};

			button2.Click += (s, a) =>
			{
				var messageBox = Dialog.CreateMessageBox("2R", "2 Rows Button pushed!");
				messageBox.ShowModal();
			};
			grid.Widgets.Add(button2);

			var text = @"Lorem ipsum \c[green]dolor sit amet, \c[red]consectetur adipisicing elit," + 
				@" sed do eiusmod \c[#AAAAAA]tempor incididunt ut labore et dolore magna aliqua. " + 
				@"Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip " + 
				@"ex ea commodo consequat. \c[white]Duis aute irure dolor in reprehenderit in voluptate " + 
				"velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non " + 
				"proident, sunt in culpa qui officia deserunt mollit anim id est laborum!";
			var label = new Label
			{
				Text = text,
				VerticalSpacing = 0,
				TextColor = Color.AntiqueWhite,
				Wrap = true
			};

			var pane = new ScrollViewer
			{
				Content = label,
				Width = 200,
				Height = 200
			};

			_window = new Window
			{
				Title = "Text",
				Content = pane
			};

			var button3 = new ImageTextButton
			{
				Text = "Show Window",
				GridColumn = 4,
				GridRow = 3
			};
			grid.Widgets.Add(button3);

			button3.Click += (sender, args) =>
			{
				_window.ShowModal();
			};

			// Horizontal slider
			var hslider = new HorizontalSlider
			{
				GridColumn = 3,
				GridRow = 2
			};
			grid.Widgets.Add(hslider);

			// Horizontal slider value
			var hsliderValue = new Label
			{
				GridColumn = 4,
				GridRow = 2,
				Text = "HSlider Value: 0"
			};

			hslider.ValueChanged += (sender, args) =>
			{
				hsliderValue.Text = string.Format("HSlider Value: {0:0.00}", hslider.Value);
			};

			grid.Widgets.Add(hsliderValue);

			var textBlock = new Label
			{
				Width = 125,
				Text = "This is textblock which spans for several lines to demonstrate row proportion set to Auto",
				GridColumn = 4,
				GridRow = 1,
				Wrap = true
			};
			grid.Widgets.Add(textBlock);

			var checkBox = new CheckBox
			{
				Text = "This is a checkbox",
				GridColumn = 3,
				GridRow = 3,
			};
			grid.Widgets.Add(checkBox);

			// Spin buttons
			var textField = new TextBox
			{
				GridColumn = 5,
				GridRow = 1,
				Width = 100
			};
			grid.Widgets.Add(textField);

			var spinButton2 = new SpinButton
			{
				GridColumn = 5,
				GridRow = 2,
				Width = 100,
				Integer = true
			};
			grid.Widgets.Add(spinButton2);

			// Progress bars
			_horizontalProgressBar = new HorizontalProgressBar
			{
				GridColumn = 5,
				GridRow = 3,
				Width = 100
			};
			grid.Widgets.Add(_horizontalProgressBar);

			_verticalProgressBar = new VerticalProgressBar
			{
				GridColumn = 6,
				GridRow = 1,
				Height = 100
			};
			grid.Widgets.Add(_verticalProgressBar);

			// List box
			var list = new ListBox
			{
				GridColumn = 5,
				GridRow = 4
			};

			list.Items.Add(new ListItem("Red", Color.Red));
			list.Items.Add(new ListItem("Green", Color.Green));
			list.Items.Add(new ListItem("Blue", Color.Blue));
			grid.Widgets.Add(list);

			// Vertical slider
			var vslider = new VerticalSlider
			{
				GridColumn = 2,
				GridRow = 4
			};
			grid.Widgets.Add(vslider);

			// Vertical slider value
			var vsliderValue = new Label
			{
				GridColumn = 4,
				GridRow = 4,
				Text = "VSlider Value: 0"
			};

			vslider.ValueChanged += (sender, args) =>
			{
				vsliderValue.Text = string.Format("VSlider Value: {0:0.00}", vslider.Value);
			};

			grid.Widgets.Add(vsliderValue);

			var tree = new Tree
			{
				HasRoot = false,
				GridColumn = 3,
				GridRow = 4
			};
			var node1 = tree.AddSubNode("node1");
			var node2 = node1.AddSubNode("node2");
			var node3 = node2.AddSubNode("node3");
			node3.AddSubNode("node4");
			node3.AddSubNode("node5");
			node2.AddSubNode("node6");

			grid.Widgets.Add(tree);

			var textBlock2 = new Label
			{
				Text = "This is long textblock",
				GridColumn = 1,
				GridRow = 4
			};
			grid.Widgets.Add(textBlock2);

			var hsplitPane = new HorizontalSplitPane
			{
				GridColumn = 1,
				GridRow = 5
			};
			var hsplitPaneLabel1 = new Label
			{
				Text = "Left"
			};
			hsplitPane.Widgets.Add(hsplitPaneLabel1);
			var hsplitPaneLabel2 = new Label
			{
				Text = "Right"
			};
			hsplitPane.Widgets.Add(hsplitPaneLabel2);
			grid.Widgets.Add(hsplitPane);

			var vsplitPane = new VerticalSplitPane
			{
				GridColumn = 6,
				GridRow = 4
			};
			var vsplitPaneLabel1 = new Label
			{
				Text = "Top"
			};
			vsplitPane.Widgets.Add(vsplitPaneLabel1);
			var vsplitPaneLabel2 = new Label
			{
				Text = "Bottom"
			};
			vsplitPane.Widgets.Add(vsplitPaneLabel2);
			grid.Widgets.Add(vsplitPane);

			for (var i = 1; i < grid.RowsProportions.Count; ++i)
			{
				var header = new Label
				{
					Text = grid.RowsProportions[i].ToString(),
					GridColumn = 0,
					GridRow = i
				};

				grid.Widgets.Add(header);
			}

			Desktop.Root = grid;
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_horizontalProgressBar.Value += 0.5f;
			if (_horizontalProgressBar.Value > _horizontalProgressBar.Maximum)
			{
				_horizontalProgressBar.Value = _horizontalProgressBar.Minimum;
			}

			_verticalProgressBar.Value += 0.5f;
			if (_verticalProgressBar.Value > _verticalProgressBar.Maximum)
			{
				_verticalProgressBar.Value = _verticalProgressBar.Minimum;
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			Desktop.Render();
		}
	}
}