using System;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using Myra.Platform;

using Vector2 = System.Drawing.PointF;

namespace Myra.Samples.AllWidgets
{
	internal class MGRenderer: IMyraRenderer
	{
		private bool _beginCalled;

		private static RasterizerState _uiRasterizerState;

		private static RasterizerState UIRasterizerState
		{
			get
			{
				if (_uiRasterizerState != null)
				{
					return _uiRasterizerState;
				}

				_uiRasterizerState = new RasterizerState
				{
					ScissorTestEnable = true
				};
				return _uiRasterizerState;
			}
		}

		private readonly GraphicsDevice _device;

		public Rectangle Scissor
		{
			get
			{
				var rect = _device.ScissorRectangle;

				rect.X -= _device.Viewport.X;
				rect.Y -= _device.Viewport.Y;

				return rect.ToSystemDrawing();
			}

			set
			{
				Flush();
				value.X += _device.Viewport.X;
				value.Y += _device.Viewport.Y;
				_device.ScissorRectangle = value.ToXNA();
			}
		}

		private readonly SpriteBatch _batch;

		public MGRenderer(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
			{
				throw new ArgumentNullException(nameof(graphicsDevice));
			}

			_device = graphicsDevice;
			_batch = new SpriteBatch(_device);
		}

		public void Begin()
		{
			_batch.Begin(SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointClamp,
				null,
				UIRasterizerState);

			_beginCalled = true;
		}

		public void End()
		{
			_batch.End();
			_beginCalled = false;
		}

		public void Draw(object texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation,
			Vector2 origin, Vector2 scale, float depth)
		{
			var xnaTexture = (Texture2D)texture;

			_batch.Draw(xnaTexture,
				position.ToXNA(),
				sourceRectangle?.ToXNA(),
				color.ToXNA(),
				rotation,
				origin.ToXNA(),
				scale.ToXNA(),
				SpriteEffects.None,
				depth);
		}

		public void Draw(object texture, Rectangle dest, Rectangle src, Color color)
		{
			var xnaTexture = (Texture2D)texture;

			_batch.Draw(xnaTexture,
				dest.ToXNA(),
				src.ToXNA(),
				color.ToXNA());
		}

		private void Flush()
		{
			if (_beginCalled)
			{
				End();
				Begin();
			}
		}
	}
}
