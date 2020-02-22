using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using System.IO;
using XNAssets;
using XNAssets.Utility;

namespace Myra.Samples.AssetManagement
{
	public class AssetManagementGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;
		private MainForm _mainForm;
		
		public AssetManagementGame()
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

			MyraEnvironment.DefaultAssetManager.AssetResolver = new FileAssetResolver(Path.Combine(PathUtils.ExecutingAssemblyDirectory, "Assets"));

			_mainForm = new MainForm();
			_mainForm._mainMenu.HoverIndex = 0;
			_mainForm._menuItemQuit.Selected += (s, a) => Exit();

			Desktop.FocusedKeyboardWidget = _mainForm._mainMenu;

			// Make main menu permanently hold keyboard focus
			Desktop.WidgetLosingKeyboardFocus += (s, a) =>
			{
				a.Cancel = true;
			};
			Desktop.Widgets.Add(_mainForm);
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

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);
			Desktop.Render();
		}
	}
}