using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using System;
using System.IO;

namespace Myra.Tools.ToMyraAtlasConverter
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("ToMyraAtlasConverter converts LibGDX texture atlas to Myra texture atlas.");
				Console.WriteLine("Usage: ToMyraAtlasConverter <input.atlas> <output.json>");

				return;
			}

			try
			{
				var inputFile = args[0];
				var outputFile = args[1];

				var inputData = File.ReadAllText(inputFile);
				var atlas = TextureRegionAtlas.FromGDX(inputData, s => new Texture2D(null, 1, 1));

				var json = atlas.ToJson();
				File.WriteAllText(outputFile, json);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
