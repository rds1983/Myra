using System;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Utility;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.Text
{
	public class TextLine
	{
		protected string _text;
		protected readonly SpriteFont _spriteFont;
		protected Point _size;

		public int Count
		{
			get { return _text.Length(); }
		}

		public string Text
		{
			get { return _text; }
		}

		public Point Size
		{
			get
			{
				return _size;
			}
		}

		public int? UnderscoreIndex { get; set; }

		public int LineIndex
		{
			get; internal set;
		}

		public int Top
		{
			get; internal set;
		}

		public int LineStart
		{
			get; internal set;
		}

		public TextLine(SpriteFont font, string text, Point size)
		{
			if (font == null)
			{
				throw new ArgumentNullException("font");
			}

			_spriteFont = font;
			_text = text;
			_size = size;
		}

		public virtual void Draw(SpriteBatch batch, Point pos, Color color, float opacity = 1.0f)
		{
			batch.DrawString(_spriteFont, _text, new Vector2(pos.X, pos.Y), color * opacity);
		}
	}
}
