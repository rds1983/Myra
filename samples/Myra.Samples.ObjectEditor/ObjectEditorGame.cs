using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Properties;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using System.Linq;
using Myra.Graphics2D;

namespace Myra.Samples.ObjectEditor
{
	public class ObjectEditorGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private readonly Player _player = new Player();
		private Texture2D _playerImage;
		private Label _labelOverGui;
		private Window _windowEditor;
		private Point _lastPosition = new Point(600, 100);
		private SpriteBatch _spriteBatch;
		private RenderContext _renderContext;
		private Desktop _desktop;
		private SpriteFontBase _font;

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

			_font = DefaultAssets.DefaultStylesheet.Fonts.Values.First();

			var root = new Panel();

			var showButton = new ToggleButton
			{
				Content = new Label
				{
					Text = "Show",
				}
			};

			showButton.PressedChanged += ShowButton_PressedChanged;

			root.Widgets.Add(showButton);

			_labelOverGui = new Label
			{
				VerticalAlignment = VerticalAlignment.Bottom
			};
			root.Widgets.Add(_labelOverGui);

			_desktop = new Desktop
			{
				Root = root
			};

			var propertyGrid = new PropertyGrid
			{
				Width = 350
			};


			propertyGrid.CustomWidgetProvider = new System.Func<Record, object, Widget>((r, obj) =>
			{
				RenderAsSliderAttribute att;
				if (r.Type == typeof(int) && (att = r.FindAttribute<RenderAsSliderAttribute>()) != null)
				{
					var value = (int)r.GetValue(obj);
					return new HorizontalProgressBar()
					{
						Minimum = att.Min,
						Maximum = att.Max,
						Value = value,
						HorizontalAlignment = HorizontalAlignment.Stretch,
						Height = 20
					};
				}

				return null;
			});

			propertyGrid.Object = _player;

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
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};
#endif

			_renderContext = new RenderContext();
		}

		private void ShowButton_PressedChanged(object sender, System.EventArgs e)
		{
			var button = (ToggleButton)sender;
			if (button.IsPressed)
			{
				_windowEditor.Show(_desktop, _lastPosition);
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

				var playerRect = new Rectangle(_player.X, _player.Y, _player.Width, _player.Height);
				if (_player.Background != null)
				{
					_renderContext.Begin();
					_player.Background.Draw(_renderContext, playerRect, _player.Color);
					_renderContext.End();
				}

				_spriteBatch.Begin();

				if (!string.IsNullOrEmpty(_player.Name))
				{
					var size = _font.MeasureString(_player.Name);
					_spriteBatch.DrawString(_font, _player.Name, 
						new Vector2(_player.X, _player.Y - size.Y - 5), _player.Color);
				}

				_spriteBatch.DrawString(_font, 
					string.Format("HP: {0}/{1}", _player.HitPoints.Current, _player.HitPoints.Maximum), 
					new Vector2(playerRect.X, playerRect.Bottom + 5), _player.Color);

				_spriteBatch.Draw(_playerImage, playerRect, _player.Color);
				_spriteBatch.End();
			}

			_labelOverGui.Text = "Is mouse over GUI: " + _desktop.IsMouseOverGUI;

			_desktop.Render();
		}
	}
}