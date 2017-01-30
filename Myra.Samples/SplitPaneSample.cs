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
		private Grid _root;
		private SplitPane _splitPane;

		protected override void LoadContent()
		{
			base.LoadContent();

			_host = new Desktop();

			_root = new Grid();

			// Top row is buttons
			_root.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			var topRow = new Grid();
			topRow.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			topRow.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			var buttonSwitchOrientation = new Button
			{
				Text = "Switch Orientation"
			};

			buttonSwitchOrientation.Down += (sender, args) =>
			{
				RebuildSplitPane(_splitPane.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
			};

			topRow.Widgets.Add(buttonSwitchOrientation);

			var buttonReset = new Button
			{
				Text = "Reset",
				GridPosition = {X = 1}
			};

			buttonReset.Down += (sender, args) =>
			{
				_splitPane.Reset();
			};

			topRow.Widgets.Add(buttonReset);

			_root.Widgets.Add(topRow);

			RebuildSplitPane(Orientation.Horizontal);

			_host.Widgets.Add(_root);
		}

		private void RebuildSplitPane(Orientation orientation)
		{
			if (_splitPane != null)
			{
				_root.Widgets.Remove(_splitPane);
				_splitPane = null;
			}

			_splitPane =
				orientation == Orientation.Horizontal ? (SplitPane) new HorizontalSplitPane() : new VerticalSplitPane();

			_splitPane.GridPosition.Y = 1;

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

			_root.Widgets.Add(_splitPane);
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
			_host.Render();
		}
	}
}