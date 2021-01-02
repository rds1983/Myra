// FillRectangle/DrawRectangle code had been borrowed from the MonoGame.Extended project: https://github.com/craftworkgames/MonoGame.Extended

using FontStashSharp;
using Myra.Platform;
using System;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Vector2 = System.Drawing.PointF;

#endif

namespace Myra.Graphics2D
{
	public class RenderContext
	{
		private readonly IMyraRenderer _renderer;
		private Matrix? _transform;

		public Matrix? Transform
		{
			get
			{
				return _transform;
			}

			set
			{
				if (value == _transform)
				{
					return;
				}

				_transform = value;

				if (_transform != null)
				{
					InverseTransform = Matrix.Invert(_transform.Value);
				}
			}
		}

		internal Matrix InverseTransform { get; set; }

		public Rectangle Scissor
		{
			get
			{
				return _renderer.Scissor;
			}

			set
			{
				if (Transform != null)
				{
					var pos = new Vector2(value.X, value.Y).Transform(Transform.Value);
					var size = new Vector2(value.Width, value.Height).Transform(Transform.Value);

					value = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
				}

				_renderer.Scissor = value;
			}
		}

		public Rectangle View { get; set; }

		public float Opacity { get; set; }

		public RenderContext(IMyraRenderer renderer)
		{
			if (renderer == null)
			{
				throw new ArgumentNullException(nameof(renderer));
			}

			_renderer = renderer;
		}

		public void Draw(object texture, Rectangle dest, Rectangle src, Color color)
		{
			_renderer.Draw(texture, dest, src, CrossEngineStuff.MultiplyColor(color, Opacity));
		}

		public void DrawString(SpriteFontBase font, string text, Point pos, Color color)
		{
			font.DrawText(_renderer, pos.X, pos.Y, text, CrossEngineStuff.MultiplyColor(color, Opacity));
		}

		/// <summary>
		/// Draws a filled rectangle
		/// </summary>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public void FillRectangle(Rectangle rectangle, Color color)
		{
			Draw(DefaultAssets.WhiteTexture, rectangle, Mathematics.RectangleOne, CrossEngineStuff.MultiplyColor(color, Opacity));
		}

		/// <summary>
		/// Draws a rectangle with the thickness provided
		/// </summary>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		/// <param name="thickness">The thickness of the lines</param>
		public void DrawRectangle(Rectangle rectangle, Color color, int thickness = 1)
		{
			var texture = DefaultAssets.WhiteTexture;

			var c = CrossEngineStuff.MultiplyColor(color, Opacity);

			// Top
			Draw(texture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), Mathematics.RectangleOne, c);

			// Bottom
			Draw(texture, new Rectangle(rectangle.X, rectangle.Bottom - thickness, rectangle.Width, thickness), Mathematics.RectangleOne, c);

			// Left
			Draw(texture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), Mathematics.RectangleOne, c);

			// Right
			Draw(texture, new Rectangle(rectangle.Right - thickness, rectangle.Y, thickness, rectangle.Height), Mathematics.RectangleOne, c);
		}

		internal void Begin()
		{
			_renderer.Begin();
		}

		internal void End()
		{
			_renderer.End();
		}

/*		internal void Flush()
		{
			End();
			Begin();
		}*/
	}
}