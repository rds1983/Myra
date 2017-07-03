using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.IO;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.Text
{
	public static class BitmapFontHelper
	{
		/// <summary>
		/// Ensures that the specified text contains valid description for BMFont
		/// </summary>
		/// <param name="input"></param>
		public static void Validate(string input)
		{
			var data = new Cyotek.Drawing.BitmapFont.BitmapFont();
			data.LoadText(input);
		}
		
		public static BitmapFont LoadFromFnt(string name, string text, Func<string, TextureRegion2D> textureLoader)
		{
			var data = new Cyotek.Drawing.BitmapFont.BitmapFont();
			data.LoadText(text);

			// Resolve pages
			var pageRegions = new TextureRegion2D[data.Pages.Length];
			for (var i = 0; i < data.Pages.Length; ++i)
			{
				var fn = data.Pages[i].FileName;
				var region = textureLoader(fn);
				if (region == null)
				{
					throw new Exception(string.Format("Unable to resolve texture {0}", fn));
				}

				pageRegions[i] = region;
			}

			var glyphs = new List<BitmapFontRegion>();
			foreach (var pair in data.Characters)
			{
				var character = pair.Value;

				var bounds = character.Bounds;

				var pageRegion = pageRegions[pair.Value.TexturePage];
				bounds.Offset(pageRegion.X, pageRegion.Y);

				var region = new TextureRegion2D(pageRegion.Texture, bounds);
				var glyph = new BitmapFontRegion(region, character.Char, character.Offset.X, character.Offset.Y, character.XAdvance);
				glyphs.Add(glyph);
			}

/*			var characterMap = glyphs.ToDictionary(a => a.Character);

			// Process kernings
			foreach (var pair in data.Kernings)
			{
				var kerning = pair.Key;

				BitmapFontRegion glyph;
				if (!characterMap.TryGetValue(kerning.FirstCharacter, out glyph))
				{
					continue;
				}

				glyph.Kernings[kerning.SecondCharacter] = kerning.Amount;
			}*/

			var result = new BitmapFont(name, glyphs, data.LineHeight);

			return result;
		}
	}
}