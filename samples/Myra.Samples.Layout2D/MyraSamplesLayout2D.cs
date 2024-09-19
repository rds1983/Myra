using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using System;
using System.Linq;

namespace Myra.Samples.Layout2D
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MyraSamplesLayout2D : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Desktop _desktop;

		public MyraSamplesLayout2D()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			MyraEnvironment.Game = this;

			IsMouseVisible = true;
			Window.AllowUserResizing = true;

			_desktop = new Desktop
			{
				HasExternalTextInput = true
			};

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Grid g = new Grid();
			var panel = new Panel();
			panel.Widgets.Add(new TextBox() { Text = "this.X=W.w/3;this.Y=W.h/2", Id = "Expression", Top = 25, Width = 100, Height = 100 });
			var btnA = new Button();
			var btnB = new Button()
			{
				Left = 500,
				Top = 500,
				Content = new Label
				{
					Text = "Button"
				}
			};
			btnB.Click += (object sender, EventArgs e) => { Console.WriteLine(btnB.Layout2d.Expresion); Console.ReadKey(); };

			btnA.Content = new Label
			{
				Text = "Calc"
			};
			btnA.Click += (object sender, EventArgs e) => { btnB.Layout2d.Expresion = (_desktop.FindChild("Expression") as TextBox).Text; _desktop.InvalidateLayout(); _desktop.UpdateLayout(); };
			panel.Widgets.Add(btnA);

			g.Widgets.Add(panel);
			g.Widgets.Add(btnB);

			_desktop.Root = g;
			// TODO: use this.Content to load your game content here
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
			Console.WriteLine((_desktop.Widgets.ToList().Find((Widget w) => { return w.Id == "Expression"; }) as TextBox)?.Text);


			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_desktop.Render();

			base.Draw(gameTime);
		}
	}
}
