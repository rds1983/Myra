using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.Text
{
	public static class SpriteFontHelper
	{
		public static SpriteFont LoadFromFnt(string text, Func<string, TextureRegion> textureRegionLoader)
		{
			var data = new BitmapFont();
			data.LoadText(text);

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

			var constructorInfo = typeof(SpriteFont).GetConstructors().First();
			var result = (SpriteFont) constructorInfo.Invoke(new object[]
			{
				textureRegion.Texture, glyphBounds, cropping,
				chars, data.LineHeight, 0, kerning, ' '
			});

			return result;
		}
	}
}