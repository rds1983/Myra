using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System.Reflection.PortableExecutable;

namespace Myra.Samples.GridContainer
{
	public class GridGame : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private Window _window;
		private HorizontalProgressBar _horizontalProgressBar;
		private VerticalProgressBar _verticalProgressBar;
		private Desktop _desktop;

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

			_desktop = new Desktop();

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
				};
				Grid.SetColumn(header, i);
				Grid.SetRow(header, 0);

				grid.Widgets.Add(header);
			}

			// Combo
			var combo = new ComboView();
			Grid.SetColumn(combo, 1);
			Grid.SetRow(combo, 1);

			combo.Widgets.Add(new Label { Text = "Red", TextColor = Color.Red });
			combo.Widgets.Add(new Label { Text = "Green", TextColor = Color.Green });
			combo.Widgets.Add(new Label { Text = "Blue", TextColor = Color.Blue });

			grid.Widgets.Add(combo);

			// Button
			var button = new Button
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = new Label
				{
					Text = "This is 2 columns button"
				}
			};
			Grid.SetColumn(button, 2);
			Grid.SetRow(button, 1);
			Grid.SetColumnSpan(button, 2);
			Grid.SetRowSpan(button, 1);


			button.Click += (s, a) =>
			{
				var messageBox = Dialog.CreateMessageBox("2C", "2 Columns Button pushed!");
				messageBox.ShowModal(_desktop);
			};

			grid.Widgets.Add(button);

			// Button
			var button2 = new Button
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Content = new Label
				{
					Text = "This is 2 rows button"
				}

			};
			Grid.SetColumn(button2, 2);
			Grid.SetRow(button2, 2);
			Grid.SetColumnSpan(button2, 1);
			Grid.SetRowSpan(button2, 2);


			button2.Click += (s, a) =>
			{
				var messageBox = Dialog.CreateMessageBox("2R", "2 Rows Button pushed!");
				messageBox.ShowModal(_desktop);
			};
			grid.Widgets.Add(button2);

			var text = @"Lorem ipsum /c[green]dolor sit amet, /c[red]consectetur adipisicing elit," +
				@" sed do eiusmod /c[#AAAAAA]tempor incididunt ut labore et dolore magna aliqua. " +
				@"Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip " +
				@"ex ea commodo consequat. /c[white]Duis aute irure dolor in reprehenderit in voluptate " +
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

			var button3 = new Button
			{
				Content = new Label
				{
					Text = "Show Window"
				}
			};
			Grid.SetColumn(button3, 4);
			Grid.SetRow(button3, 3);

			grid.Widgets.Add(button3);

			button3.Click += (sender, args) =>
			{
				_window.ShowModal(_desktop);
			};

			// Horizontal slider
			var hslider = new HorizontalSlider();
			Grid.SetColumn(hslider, 3);
			Grid.SetRow(hslider, 2);
			grid.Widgets.Add(hslider);

			// Horizontal slider value
			var hsliderValue = new Label
			{
				Text = "HSlider Value: 0"
			};
			Grid.SetColumn(hsliderValue, 4);
			Grid.SetRow(hsliderValue, 2);

			hslider.ValueChanged += (sender, args) =>
			{
				hsliderValue.Text = string.Format("HSlider Value: {0:0.00}", hslider.Value);
			};

			grid.Widgets.Add(hsliderValue);

			var textBlock = new Label
			{
				Width = 125,
				Text = "This is textblock which spans for several lines to demonstrate row proportion set to Auto",
				Wrap = true
			};
			Grid.SetColumn(textBlock, 4);
			Grid.SetRow(textBlock, 1);

			grid.Widgets.Add(textBlock);

			var checkBox = new CheckButton
			{
				Content = new Label
				{
					Text = "This is a checkbox"
				}
			};
			Grid.SetColumn(checkBox, 3);
			Grid.SetRow(checkBox, 3);

			grid.Widgets.Add(checkBox);

			// Spin buttons
			var textField = new TextBox
			{
				Width = 100
			};
			Grid.SetColumn(textField, 5);
			Grid.SetRow(textField, 1);

			grid.Widgets.Add(textField);

			var spinButton2 = new SpinButton
			{
				Width = 100,
				Integer = true
			};
			Grid.SetColumn(spinButton2, 5);
			Grid.SetRow(spinButton2, 2);

			grid.Widgets.Add(spinButton2);

			// Progress bars
			_horizontalProgressBar = new HorizontalProgressBar
			{
				Width = 100
			};
			Grid.SetColumn(_horizontalProgressBar, 5);
			Grid.SetRow(_horizontalProgressBar, 3);

			grid.Widgets.Add(_horizontalProgressBar);

			_verticalProgressBar = new VerticalProgressBar
			{
				Height = 100
			};
			Grid.SetColumn(_verticalProgressBar, 6);
			Grid.SetRow(_verticalProgressBar, 1);

			grid.Widgets.Add(_verticalProgressBar);

			// List box
			var list = new ListView();
			Grid.SetColumn(list, 5);
			Grid.SetRow(list, 4);

			list.Widgets.Add(new Label { Text = "Red", TextColor = Color.Red });
			list.Widgets.Add(new Label { Text = "Green", TextColor = Color.Green });
			list.Widgets.Add(new Label { Text = "Blue", TextColor = Color.Blue });
			grid.Widgets.Add(list);

			// Vertical slider
			var vslider = new VerticalSlider();
			Grid.SetColumn(vslider, 2);
			Grid.SetRow(vslider, 4);

			grid.Widgets.Add(vslider);

			// Vertical slider value
			var vsliderValue = new Label
			{
				Text = "VSlider Value: 0"
			};
			Grid.SetColumn(vsliderValue, 4);
			Grid.SetRow(vsliderValue, 4);

			vslider.ValueChanged += (sender, args) =>
			{
				vsliderValue.Text = string.Format("VSlider Value: {0:0.00}", vslider.Value);
			};

			grid.Widgets.Add(vsliderValue);

			var tree = new TreeView();
			Grid.SetColumn(tree, 3);
			Grid.SetRow(tree, 4);

			var node1 = tree.AddSubNode(new Label
			{
				Text = "node1"
			});
			var node2 = node1.AddSubNode(new Label
			{
				Text = "node2"
			});
			var node3 = node2.AddSubNode(new Label
			{
				Text = "node3"
			});
			node3.AddSubNode(new Label
			{
				Text = "node4"
			});
			node3.AddSubNode(new Label
			{
				Text = "node5"
			});
			node2.AddSubNode(new Label
			{
				Text = "node6"
			});

			grid.Widgets.Add(tree);

			var textBlock2 = new Label
			{
				Text = "This is long textblock"
			};
			Grid.SetColumn(textBlock2, 1);
			Grid.SetRow(textBlock2, 4);

			grid.Widgets.Add(textBlock2);

			var hsplitPane = new HorizontalSplitPane();
			Grid.SetColumn(hsplitPane, 1);
			Grid.SetRow(hsplitPane, 5);

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

			var vsplitPane = new VerticalSplitPane();
			Grid.SetColumn(vsplitPane, 6);
			Grid.SetRow(vsplitPane, 4);

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
				};
				Grid.SetColumn(header, 0);
				Grid.SetRow(header, i);

				grid.Widgets.Add(header);
			}

			_desktop.Root = grid;
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

			_desktop.Render();
		}
	}
}