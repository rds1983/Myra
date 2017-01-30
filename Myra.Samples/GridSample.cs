using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Button = Myra.Graphics2D.UI.Button;
using CheckBox = Myra.Graphics2D.UI.CheckBox;
using ComboBox = Myra.Graphics2D.UI.ComboBox;
using HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment;

namespace Myra.Samples
{
	public class GridSample: Game
	{
		private readonly GraphicsDeviceManager graphics;

		private Desktop _host;

		public GridSample()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			// Widget.DrawFrames = true;
			_host = new Desktop();

			var grid = new Grid
			{
				RowSpacing = 3,
				ColumnSpacing = 3,
				DrawLines = true
			};

			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part, 1.0f));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part, 2.0f));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 150.0f));
			grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part, 1.0f));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Part, 1.0f));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Pixels, 200.0f));
			grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			// Create headers
			for (var i = 1; i < grid.ColumnsProportions.Count; ++i)
			{
				var header = new TextBlock
				{
					Text = grid.ColumnsProportions[i].ToString(),
					GridPosition =
					{
						X = i,
						Y = 0
					}
				};

				grid.Widgets.Add(header);
			}

			// Combo
			var combo = new ComboBox
			{
				GridPosition =
				{
					X = 1,
					Y = 1
				}
			};

			combo.Items.Add(new ListItem("Red", Color.Red));
			combo.Items.Add(new ListItem("Green", Color.Green));
			combo.Items.Add(new ListItem("Blue", Color.Blue));
			grid.Widgets.Add(combo);

			// Button
			var button = new Button
			{
				GridPosition =
				{
					X = 2,
					Y = 1
				},
				GridSpan =
				{
					X = 2,
					Y = 1
				},
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Text = "This is 2 columns button"
			};

			button.Down += (s, a) =>
			{
				var messageBox = Dialog.CreateMessageBox("2C", "2 Columns Button pushed!");
				messageBox.ShowModal(_host);
			};

			grid.Widgets.Add(button);

			// Button
			var button2 = new Button
			{
				GridPosition =
				{
					X = 2,
					Y = 2
				},
				GridSpan =
				{
					X = 1,
					Y = 2
				},
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Text = "This is 2 rows button"
			};

			button2.Down += (s, a) =>
			{
				var messageBox = Dialog.CreateMessageBox("2R", "2 Rows Button pushed!");
				messageBox.ShowModal(_host);
			};

			grid.Widgets.Add(button2);

			var textBlock = new TextBlock
			{
				WidthHint = 125,
				Text = "This is textblock which spans for several lines to demonstrate row proportion set to Auto",
				GridPosition =
				{
					X = 4,
					Y = 1
				}
			};
			grid.Widgets.Add(textBlock);

			var checkBox = new CheckBox
			{
				Text = "This is a checkbox",
				GridPosition =
				{
					X = 3,
					Y = 3
				}
			};
			grid.Widgets.Add(checkBox);

			var textBlock2 = new TextBlock
			{
				Text = "This is long textblock",
				GridPosition =
				{
					X = 1,
					Y = 4
				}
			};
			grid.Widgets.Add(textBlock2);

			for (var i = 1; i < grid.RowsProportions.Count; ++i)
			{
				var header = new TextBlock
				{
					Text = grid.RowsProportions[i].ToString(),
					GridPosition =
					{
						X = 0,
						Y = i
					}
				};

				grid.Widgets.Add(header);
			}

			_host.Widgets.Add(grid);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GraphicsDevice.Clear(Color.Black);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}
	}
}
