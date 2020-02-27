using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Properties;

namespace Myra.Samples.AllWidgets
{
	public class CustomWidgetsGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private PropertyGrid _propertyGrid;

		public CustomWidgetsGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};
			Window.AllowUserResizing = true;

			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			var arrow = new Arrow
			{
				Width = 200,
				Height = 50,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			var scrollViewer = new ScrollViewer();

			_propertyGrid = new PropertyGrid
			{
				Object = arrow
			};

			scrollViewer.Content = _propertyGrid;

			var topPanel = new HorizontalSplitPane();


			topPanel.Widgets.Add(arrow);
			topPanel.Widgets.Add(scrollViewer);

			topPanel.SetSplitterPosition(0, 0.75f);

			Desktop.Root = topPanel;

			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			Desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				Desktop.OnChar(a.Character);
			};

#if ANDROID
			Desktop.WidgetGotKeyboardFocus += (s, a) =>
			{
				var asTextBox = a.Data as TextBox;
				if (asTextBox == null)
				{
					return;
				}

				Guide.BeginShowKeyboardInput(PlayerIndex.One,
					"Title",
					"Description",
					asTextBox.Text,
					new AsyncCallback(r =>
					{
						var text = Guide.EndShowKeyboardInput(r);
						asTextBox.Text = text;
					}),
					null);
			};
#endif
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			Desktop.Render();
		}
	}
}