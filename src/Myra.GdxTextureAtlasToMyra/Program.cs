using System;
using System.IO;

namespace Myra.GdxTextureAtlasToMyra
{
	class Program
	{
		static void Convert(string inputFile, string outputFile)
		{
			outputFile = Path.ChangeExtension(outputFile, "xmat");

			var atlas = Gdx.FromGDX(File.ReadAllText(inputFile));

			var xmlData = atlas.ToXml();

			File.WriteAllText(outputFile, xmlData);

			Console.WriteLine("Success.");
		}

		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Usage: Myra.GdxTextureAtlasToMyra <input_file.atlas> <output_file.xmat>");
				return;
			}

			try
			{
				Convert(args[0], args[1]);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
