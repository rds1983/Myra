using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace Myra.Samples.SplitPaneContainer
{
	public class SplitPaneGame : Game
	{
		private readonly GraphicsDeviceManager graphics;

		private const int Labels = 5;

		private Grid _root;
		private SplitPane _splitPane;
		private Desktop _desktop;

		public SplitPaneGame()
		{
			graphics = new GraphicsDeviceManager(this);

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			_root = new Grid();

			// Top row is buttons
			_root.RowsProportions.Add(new Proportion(ProportionType.Auto));

			var topRow = new HorizontalStackPanel
			{
				Spacing = 8
			};

			var buttonSwitchOrientation = new Button
			{
				Content = new Label
				{
					Text = "Switch Orientation"
				}
			};

			buttonSwitchOrientation.Click += (sender, args) =>
			{
				RebuildSplitPane(_splitPane.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
			};

			topRow.Widgets.Add(buttonSwitchOrientation);

			var buttonReset = new Button
			{
				Content = new Label
				{
					Text = "Reset"
				}
			};
			Grid.SetColumn(buttonReset, 1);

			buttonReset.Click += (sender, args) =>
			{
				_splitPane.Reset();
			};

			topRow.Widgets.Add(buttonReset);

			_root.Widgets.Add(topRow);

			RebuildSplitPane(Orientation.Horizontal);

			_desktop = new Desktop
			{
				Root = _root
			};
		}

		private void RebuildSplitPane(Orientation orientation)
		{
			if (_splitPane != null)
			{
				_root.Widgets.Remove(_splitPane);
				_splitPane = null;
			}

			_splitPane =
				orientation == Orientation.Horizontal ? (SplitPane)new HorizontalSplitPane() : new VerticalSplitPane();

			Grid.SetRow(_splitPane, 1);

			_splitPane.ProportionsChanged += SplitPaneOnProportionsChanged;

			for (var i = 0; i < Labels; ++i)
			{
				var label = new Label
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
				var label = (Label)_splitPane.Widgets[i];

				label.Text = _splitPane.GetProportion(i).ToString(CultureInfo.InvariantCulture);
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