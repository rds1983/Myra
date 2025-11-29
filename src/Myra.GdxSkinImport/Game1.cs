using GdxSkinImport.MonoGdx;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;

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
		TextureAtlas atlas = null;
		var atlasFile = Path.ChangeExtension(_inputFile, "atlas");
		if (File.Exists(atlasFile))
		{
			atlas = new TextureAtlas(GraphicsDevice, atlasFile);
		}

		Dictionary<string, object> data;
		using (TextReader reader = new StreamReader(_inputFile))
		{
			data = Json.Deserialize(reader) as Dictionary<string, object>;
		}

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
