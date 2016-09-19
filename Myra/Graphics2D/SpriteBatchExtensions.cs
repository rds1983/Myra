using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D
{
	public static class SpriteBatchExtensions
	{
		private static Texture2D _white;
		private static bool _isClipping;
		private static readonly Stack<Rectangle> _scissors = new Stack<Rectangle>();

		public static Texture2D White
		{
			get
			{
				if (_white == null)
				{
					_white = new Texture2D(MyraEnvironment.Game.GraphicsDevice, 1, 1);
					_white.SetData(new[] { Color.White });

					_white.Disposing += (sender, args) =>
					{
						_white = null;
					};
				}

				return _white;
			}
		}


		public static void DrawRect(this SpriteBatch batch, Color color, Rectangle rect)
		{
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Y, rect.Width, 1));
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1));
			FillSolidRect(batch, color, new Rectangle(rect.X, rect.Y, 1, rect.Height));
			FillSolidRect(batch, color, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height));
		}

		public static void FillSolidRect(this SpriteBatch batch, Color color, Rectangle rect)
		{
			batch.Draw(White, rect, color);
		}

		public static void BeginUI(this SpriteBatch batch)
		{
			batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState
			{
				ScissorTestEnable = true
			});
		}

		public static void FlushUI(this SpriteBatch batch)
		{
			batch.End();
			batch.BeginUI();
		}

		public static void PushClip(this SpriteBatch batch, Rectangle clipRect)
		{
			batch.FlushUI();

			if (!_isClipping)
			{

				batch.GraphicsDevice.ScissorRectangle = clipRect;
				_isClipping = true;
			}
			else
			{
				// Save the current rect
				var r = batch.GraphicsDevice.ScissorRectangle;
				_scissors.Push(r);

				// Calculate the new one
				Rectangle r2;
				Rectangle.Intersect(ref r, ref clipRect, out r2);

				batch.GraphicsDevice.ScissorRectangle = r2;
			}
		}

		public static void PopClip(this SpriteBatch batch)
		{
			batch.FlushUI();

			if (_scissors.Count > 0)
			{
				var r = _scissors.Pop();

				batch.GraphicsDevice.ScissorRectangle = r;
			}
			else
			{
				_isClipping = false;
			}
		}
	}
}
