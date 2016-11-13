using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI;

namespace Myra.Samples
{
	public class ScrollPaneSample: SampleGame
	{
		private Desktop _host;

		protected override void LoadContent()
		{
			base.LoadContent();

			_host = new Desktop();

			var label = new TextBlock
			{
				Text =
					"Lorem ipsum [Green]dolor sit amet, [Red]consectetur adipisicing elit, sed do eiusmod [#AAAAAAAA]tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. [white]Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!",
				VerticalSpacing = 0,
				TextColor = Color.AntiqueWhite,
				Wrap =  true
			};

			var pane = new ScrollPane<TextBlock>
			{
				Widget = label,
			};

			var root = new Grid
			{
				WidthHint = 200,
				HeightHint = 200,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Top
			};

			root.Children.Add(pane);

			_host.Widgets.Add(root);
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
