using System;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Graphics;
using Texture2D = Stride.Graphics.Texture;
#endif

namespace Myra.Platform.XNA
{
	internal class XNARenderer: IMyraRenderer
	{
		private bool _beginCalled;

#if MONOGAME || FNA
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
#else
		private static readonly RasterizerStateDescription _uiRasterizerState;

		static XNARenderer()
		{
			var rs = new RasterizerStateDescription();
			rs.SetDefault();
			rs.ScissorTestEnable = true;
			_uiRasterizerState = rs;
		}

#endif

		private readonly GraphicsDevice _device;

		public Rectangle Scissor
		{
			get
			{
#if !STRIDE
				var rect = _device.ScissorRectangle;

				rect.X -= _device.Viewport.X;
				rect.Y -= _device.Viewport.Y;

				return rect;
#else
				return MyraEnvironment.Game.GraphicsContext.CommandList.Scissor;
#endif
			}

			set
			{
				Flush();
#if !STRIDE
				value.X += _device.Viewport.X;
				value.Y += _device.Viewport.Y;
				_device.ScissorRectangle = value;
#else
				MyraEnvironment.Game.GraphicsContext.CommandList.SetScissorRectangle(value);
#endif
			}
		}

		private readonly SpriteBatch _batch;

		public XNARenderer(GraphicsDevice graphicsDevice)
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
#if MONOGAME || FNA
			_batch.Begin(SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointClamp,
				null,
				UIRasterizerState);
#elif STRIDE
			_batch.Begin(MyraEnvironment.Game.GraphicsContext,
				SpriteSortMode.Deferred,
				BlendStates.AlphaBlend,
				MyraEnvironment.Game.GraphicsDevice.SamplerStates.PointClamp,
				DepthStencilStates.Default,
				_uiRasterizerState);
#endif

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

#if MONOGAME || FNA
			_batch.Draw(xnaTexture,
				position,
				sourceRectangle,
				color,
				rotation,
				origin,
				scale,
				SpriteEffects.None,
				depth);
#elif STRIDE
			_batch.Draw(xnaTexture,
				position,
				sourceRectangle,
				color,
				rotation,
				origin,
				scale,
				SpriteEffects.None,
				ImageOrientation.AsIs,
				depth);
#endif
		}

		public void Draw(object texture, Rectangle dest, Rectangle src, Color color)
		{
			var xnaTexture = (Texture2D)texture;

#if MONOGAME || FNA
			_batch.Draw(xnaTexture,
				dest,
				src,
				color);
#elif STRIDE
			_batch.Draw(xnaTexture,
				new RectangleF(dest.X, dest.Y, dest.Width, dest.Height),
				new RectangleF(src.X, src.Y, src.Width, src.Height),
				color,
				0,
				Mathematics.Vector2Zero);
#endif
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
