using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI.Styles;
using Myra.MML;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace GdxSkinImport;

public class Game1 : Game
{
	private const string TextureAtlasFile = "ui_stylesheet.xmat";
	private const string StylesheetFile = "ui_stylesheet.xmms";

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

		try
		{
			var converter = new Converter(GraphicsDevice, _inputFile);
			var stylesheet = converter.Process();

			var absoluteInputPath = Path.GetFullPath(_inputFile);
			var inputDir = Path.GetDirectoryName(absoluteInputPath);
			var outputDir = ".";
			Directory.CreateDirectory(outputDir);

			SaveStylesheet(stylesheet, outputDir, inputDir, converter);

			Console.WriteLine($"Successfully converted skin to: {outputDir}");
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex}");
		}

		Exit();
	}

	private void SaveStylesheet(Stylesheet stylesheet, string outputDir, string sourceDir, Converter converter)
	{
		// Save texture atlas as .xmat
		stylesheet.Atlas.Name = TextureAtlasFile;
		var atlasXml = stylesheet.Atlas.ToXml();
		var atlasPath = Path.Combine(outputDir, TextureAtlasFile);
		File.WriteAllText(atlasPath, atlasXml);
		Console.WriteLine($"Saved atlas: {atlasPath}");

		// Copy texture image
		var imageName = stylesheet.Atlas.Image;
		var sourceImagePath = Path.Combine(sourceDir, imageName);
		if (File.Exists(sourceImagePath))
		{
			var destImagePath = Path.Combine(outputDir, imageName);
			File.Copy(sourceImagePath, destImagePath, true);
			Console.WriteLine($"Copied image: {destImagePath}");
		}

		// Save fonts with updated texture references
		if (stylesheet.Fonts != null)
		{
			var fntFiles = Directory.GetFiles(sourceDir, "*.fnt");
			foreach (var fntFile in fntFiles)
			{
				var fntFileName = Path.GetFileName(fntFile);
				var destFntPath = Path.Combine(outputDir, fntFileName);

				var fntContent = File.ReadAllText(fntFile);

				var regex = new Regex(@"file=""([^\.]+)\.([^""]+)""");
				fntContent = regex.Replace(fntContent, $"file=\"{TextureAtlasFile}:$1\"");

				// Save the font file
				File.WriteAllText(destFntPath, fntContent);
				Console.WriteLine($"Saved font file: {destFntPath}");
			}
		}

		// Save stylesheet as .xmms
		var stylesheetXml = stylesheet.ToXml();
		var stylesheetPath = Path.Combine(outputDir, StylesheetFile);
		File.WriteAllText(stylesheetPath, stylesheetXml);
		Console.WriteLine($"Saved stylesheet: {stylesheetPath}");
	}

	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.CornflowerBlue);
		base.Draw(gameTime);
	}
}
