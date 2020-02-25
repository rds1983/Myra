using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Properties;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Samples.ObjectEditor
{
	public class ObjectEditorGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly Player _player = new Player();
		private Texture2D _playerImage;
		private Label _labelOverGui;
		private Window _windowEditor;
		private Point _lastPosition = new Point(800, 100);

		private SpriteBatch _spriteBatch;
		
		public static ObjectEditorGame Instance { get; private set; }

		public ObjectEditorGame()
		{
			Instance = this;

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

			_spriteBatch = new SpriteBatch(GraphicsDevice);

			using (var stream = TitleContainer.OpenStream("image.png"))
			{
				_playerImage = Texture2D.FromStream(GraphicsDevice, stream);
			}

			MyraEnvironment.Game = this;

			var root = new Panel();

			var showButton = new TextButton
			{
				Text = "Show",
				Toggleable = true
			};

			showButton.PressedChanged += ShowButton_PressedChanged;

			root.Widgets.Add(showButton);

			_labelOverGui = new Label
			{
				VerticalAlignment = VerticalAlignment.Bottom
			};
			root.Widgets.Add(_labelOverGui);

			Desktop.Widgets.Add(root);

			var propertyGrid = new PropertyGrid
			{
				Object = _player,
				Width = 350
			};

			_windowEditor = new Window
			{
				Title = "Object Editor",
				Content = propertyGrid
			};

			_windowEditor.Closed += (s, a) =>
			{
				showButton.IsPressed = false;
			};

			// Force window show
			showButton.IsPressed = true;
#if MONOGAME
			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			Desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				Desktop.OnChar(a.Character);
			};
#endif
		}

		private void ShowButton_PressedChanged(object sender, System.EventArgs e)
		{
			var button = (TextButton)sender;
			if (button.IsPressed)
			{
				_windowEditor.Show(_lastPosition);
			}
			else
			{
				_lastPosition = new Point(_windowEditor.Bounds.X, _windowEditor.Bounds.Y);
				_windowEditor.Close();
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			if (_player.Visible)
			{
				_spriteBatch.Begin();

				var font = DefaultAssets.Font;
				if (!string.IsNullOrEmpty(_player.Name))
				{
					var size = font.MeasureString(_player.Name);
					_spriteBatch.DrawString(font, _player.Name, 
						new Vector2(_player.X, _player.Y - size.Y - 5), _player.Color);
				}

				var playerRect = new Rectangle(_player.X, _player.Y, _player.Width, _player.Height);
				if (_player.Background != null)
				{
					_player.Background.Draw(_spriteBatch, playerRect, _player.Color);
				}

				_spriteBatch.DrawString(font, 
					string.Format("HP: {0}/{1}", _player.HitPoints.Current, _player.HitPoints.Maximum), 
					new Vector2(playerRect.X, playerRect.Bottom + 5), _player.Color);

				_spriteBatch.Draw(_playerImage, playerRect, _player.Color);
				_spriteBatch.End();
			}

			_labelOverGui.Text = "Is mouse over GUI: " + Desktop.IsMouseOverGUI;

			Desktop.Render();
		}
	}
}