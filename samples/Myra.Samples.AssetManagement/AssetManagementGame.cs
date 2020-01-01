using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Samples.AssetManagement.UI;
using Myra.Assets;
using Myra.Utility;
using System.IO;

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

			AssetManager.Default.AssetResolver = new FileAssetResolver(Path.Combine(PathUtils.ExecutingAssemblyDirectory, "UI"));

			_mainForm = new MainForm();

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

			if (_graphics.PreferredBackBufferWidth != Window.ClientBounds.Width ||
			    _graphics.PreferredBackBufferHeight != Window.ClientBounds.Height)
			{
				_graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				_graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				_graphics.ApplyChanges();
			}

			Desktop.Render();
		}
	}
}