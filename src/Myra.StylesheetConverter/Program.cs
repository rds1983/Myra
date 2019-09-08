using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Myra.StylesheetConverter
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Myra.StylesheetConverter converts old json stylesheet to new xml stylesheet.");
				Console.WriteLine("Usage: Myra.StylesheetConverter <input.json> <output.xml>");

				return;
			}

			try
			{
				var inputFile = args[0];
				var outputFile = args[1];

				var loader = new StylesheetLoader(JObject.Parse(File.ReadAllText(inputFile)));
				var doc = loader.Load();
				doc.Save(args[1]);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
