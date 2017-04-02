using System;
using System.Collections.Generic;

namespace Myra.Graphics2D.Text
{
	partial class BitmapFont
	{
		public static BitmapFont LoadFromFnt(string text, Func<string, TextureRegion> textureLoader)
		{
			var data = new Cyotek.Drawing.BitmapFont.BitmapFont();
			data.LoadText(text);

			// Resolve pages
			var pageRegions = new TextureRegion[data.Pages.Length];
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

			var glyphs = new Dictionary<char, Glyph>();

			foreach (var pair in data.Characters)
			{
				var character = pair.Value;

				var bounds = character.Bounds;

				var region = new TextureRegion(pageRegions[pair.Value.TexturePage], bounds);
				var glyph = new Glyph
				{
					Id = character.Char,
					Region = region,
					Offset = character.Offset,
					XAdvance = character.XAdvance
				};

				glyphs[glyph.Id] = glyph;
			}

			// Process kernings
			foreach (var pair in data.Kernings)
			{
				var kerning = pair.Key;

				Glyph glyph;
				if (!glyphs.TryGetValue(kerning.FirstCharacter, out glyph))
				{
					continue;
				}

				glyph.Kerning[kerning.SecondCharacter] = kerning.Amount;
			}

			var result = new BitmapFont(glyphs, pageRegions, data.LineHeight);

			return result;
		}
	}
}