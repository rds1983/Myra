using System;
using System.Collections.Generic;
using System.Linq;
using Cyotek.Drawing.BitmapFont;
using Myra.Graphics2D.TextureAtlases;
using System.Reflection;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
using Xenko.Graphics.Font;
#endif

namespace Myra.Graphics2D.Text
{
	public static class SpriteFontHelper
	{
		public static SpriteFont LoadFromFnt(string text, Func<string, TextureRegion> textureRegionLoader)
		{
			var data = new BitmapFont();
			data.LoadText(text);

#if !XENKO
			var glyphBounds = new List<Rectangle>();
			var cropping = new List<Rectangle>();
			var chars = new List<char>();
			var kerning = new List<Vector3>();

			var textureRegion = textureRegionLoader(data.Pages[0].FileName);

			var characters = data.Characters.Values.OrderBy(c => c.Char);
			foreach (var character in characters)
			{
				var bounds = character.Bounds;

				bounds.Offset(textureRegion.Bounds.Location);

				glyphBounds.Add(bounds);
				cropping.Add(new Rectangle(character.Offset.X, character.Offset.Y, bounds.Width, bounds.Height));

				chars.Add(character.Char);

				kerning.Add(new Vector3(0, character.Bounds.Width, character.XAdvance - character.Bounds.Width));
			}

			var constructorInfo = typeof(SpriteFont).GetTypeInfo().DeclaredConstructors.First();
			var result = (SpriteFont) constructorInfo.Invoke(new object[]
			{
				textureRegion.Texture, glyphBounds, cropping,
				chars, data.LineHeight, 0, kerning, ' '
			});

			return result;
#else
			var textureRegion = textureRegionLoader(data.Pages[0].FileName);

			var glyphs = new List<Glyph>();
			foreach (var pair in data.Characters)
			{
				var character = pair.Value;

				var bounds = character.Bounds;
				bounds.X += textureRegion.Bounds.X;
				bounds.Y += textureRegion.Bounds.Y;
				var glyph = new Glyph
				{
					Character = character.Char,
					BitmapIndex = 0,
					Offset = new Vector2(character.Offset.X, character.Offset.Y),
					Subrect = bounds,
					XAdvance = character.XAdvance
				};

				glyphs.Add(glyph);
			}

			var images = new List<Image>
			{
				DefaultAssets.UIImage
			};

			return DefaultAssets.FontSystem.NewStatic(data.LineHeight, glyphs, images, 0, data.LineHeight);
#endif
		}
	}
}