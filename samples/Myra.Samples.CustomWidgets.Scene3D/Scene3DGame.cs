using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Properties;
using DigitalRiseModel.Primitives;

namespace Myra.Samples.CustomWidgets
{
	/// <summary>
	/// Sample game demonstrating a custom 3D rendering widget embedded in a Myra UI.
	/// Displays a rotating capsule mesh rendered via <see cref="Scene3D"/> alongside a
	/// <see cref="PropertyGrid"/> for real-time parameter tweaking.
	/// </summary>
	public class Scene3DGame : Game
	{
		private readonly GraphicsDeviceManager _graphics;

		private PropertyGrid _propertyGrid;
		private Desktop _desktop;

		public Scene3DGame()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1200,
				PreferredBackBufferHeight = 800
			};
			Window.AllowUserResizing = true;

			IsMouseVisible = true;
		}

		/// <summary>
		/// Builds the Myra UI layout: a horizontal split pane with the 3D scene on the left
		/// (taking 75 % of the width) and a scrollable property grid on the right for
		/// editing the scene's render settings at runtime.
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			var scene3d = new Scene3D();
			var scrollViewer = new ScrollViewer
			{
				ShowHorizontalScrollBar = false
			};

			_propertyGrid = new PropertyGrid
			{
				Object = scene3d
			};

			scrollViewer.Content = _propertyGrid;

			var topPanel = new HorizontalSplitPane();


			topPanel.Widgets.Add(scene3d);
			topPanel.Widgets.Add(scrollViewer);

			topPanel.SetSplitterPosition(0, 0.75f);

			_desktop = new Desktop
			{
				Root = topPanel,

				// Inform Myra that external text input is available
				// So it stops translating Keys to chars
				HasExternalTextInput = true
			};

			// Forward keyboard character input from the OS window into the Myra desktop
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

			// Create a capsule mesh and assign it to the 3D scene widget
			var mesh = MeshPrimitives.CreateCapsuleMeshPart(GraphicsDevice, tessellation: 256, uScale: 8, vScale: 8);
			scene3d.Mesh = mesh;
		}

		/// <summary>
		/// Clears the screen and renders the Myra desktop, which in turn triggers
		/// <see cref="Scene3D.InternalRender"/> for the 3D scene widget.
		/// </summary>
		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_desktop.Render();
		}
	}
}