using GdxSkinImport.MonoGdx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using System.Collections.Generic;
using System.IO;

namespace GdxSkinImport;

/// <summary>
/// This is the main type for your game.
/// </summary>
public class Game1 : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private readonly string _inputFile;

	public Game1(string inputFile)
	{
		_inputFile = inputFile;

		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1200,
			PreferredBackBufferHeight = 800
		};

		Window.AllowUserResizing = true;
		IsMouseVisible = true;
		Content.RootDirectory = inputFile;
	}

	protected override void LoadContent()
	{
		MyraEnvironment.Game = this;

		var converter = new Converter(GraphicsDevice, _inputFile);

		var stylesheet = converter.Process();

		Exit();
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	/// This is called when the game should draw itself.
	/// </summary>
	/// <param name="gameTime">Provides a snapshot of timing values.</param>
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);

		base.Draw(gameTime);
	}
}
