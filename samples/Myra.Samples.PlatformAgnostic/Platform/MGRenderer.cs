using System;
using System.Drawing;
using System.Numerics;
using FontStashSharp.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Platform;

namespace Myra.Samples.AllWidgets
{
	internal class MGRenderer: IMyraRenderer
	{
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

		private readonly MGPlatform _platform;
		private bool _beginCalled;
		private readonly SpriteBatch _batch;
		private Matrix3x2? _transform;
		private TextureFiltering _textureFiltering;

		private GraphicsDevice GraphicsDevice => _platform.GraphicsDevice;

		public Rectangle Scissor
		{
			get
			{
				var rect = GraphicsDevice.ScissorRectangle;

				rect.X -= GraphicsDevice.Viewport.X;
				rect.Y -= GraphicsDevice.Viewport.Y;

				return rect.ToSystemDrawing();
			}

			set
			{
				Flush();
				value.X += GraphicsDevice.Viewport.X;
				value.Y += GraphicsDevice.Viewport.Y;
				GraphicsDevice.ScissorRectangle = value.ToXNA();
			}
		}

		public ITexture2DManager TextureManager => _platform;

		public MGRenderer(MGPlatform platform)
		{
			if (platform == null)
			{
				throw new ArgumentNullException(nameof(platform));
			}

			_platform = platform;
			_batch = new SpriteBatch(GraphicsDevice);
		}

		public void Begin(Matrix3x2? transform, TextureFiltering textureFiltering)
		{
			var samplerState = textureFiltering == TextureFiltering.Nearest ? SamplerState.PointClamp : SamplerState.LinearClamp;
			_batch.Begin(SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				samplerState,
				null,
				UIRasterizerState,
				null,
				transform != null ? transform.Value.ToXNA() : (Microsoft.Xna.Framework.Matrix?)null);

			_beginCalled = true;
			_transform = transform;
			_textureFiltering = textureFiltering;
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

		public void Draw(object texture, Rectangle dest, Rectangle? src, Color color)
		{
			var xnaTexture = (Texture2D)texture;

			_batch.Draw(xnaTexture,
				dest.ToXNA(),
				src != null ? src.Value.ToXNA() : (Microsoft.Xna.Framework.Rectangle?)null,
				color.ToXNA());
		}

		private void Flush()
		{
			if (_beginCalled)
			{
				End();
				Begin(_transform, _textureFiltering);
			}
		}
	}
}
