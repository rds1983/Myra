using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Myra.Samples
{
	public class SplitPaneSample : SampleGame
	{
		private const int Labels = 5;

		private Desktop _host;
		private SplitPane _splitPane;

		protected override void LoadContent()
		{
			base.LoadContent();

			_host = new Desktop();

			var root = new Grid();

			// Top row is buttons
			root.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			var topRow = new Grid();
			topRow.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			topRow.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			var buttonSwitchOrientation = new Button
			{
				Text = "Switch Orientation"
			};

			buttonSwitchOrientation.Down += (sender, args) =>
			{
				var o = _splitPane.Orientation;
//				_splitPane.Orientation = o == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
			};

			topRow.Children.Add(buttonSwitchOrientation);

			var buttonReset = new Button
			{
				Text = "Reset",
				GridPosition =
				{
					X = 1
				}
			};

			buttonReset.Down += (sender, args) =>
			{
				_splitPane.Reset();
			};

			topRow.Children.Add(buttonReset);

			root.Children.Add(topRow);

			_splitPane = new SplitPane(Orientation.Horizontal)
			{
				GridPosition =
				{
					Y = 1
				}
			};

			_splitPane.ProportionsChanged += SplitPaneOnProportionsChanged;

			for (var i = 0; i < Labels; ++i)
			{
				var label = new TextBlock
				{
					Text = "Proportion"
				};

				_splitPane.Widgets.Add(label);
			}

			UpdateProportions();

			root.Children.Add(_splitPane);

			_host.Widgets.Add(root);
		}

		private void SplitPaneOnProportionsChanged(object sender, EventArgs eventArgs)
		{
			UpdateProportions();
		}

		private void UpdateProportions()
		{
			for (var i = 0; i < _splitPane.Widgets.Count; ++i)
			{
				var label = (TextBlock) _splitPane.Widgets[i];

				label.Text = _splitPane.GetProportion(i).ToString(CultureInfo.InvariantCulture);
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GraphicsDevice.Clear(Color.Black);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render(GraphicsDevice);
		}
	}
}
