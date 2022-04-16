using FontStashSharp;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Graphics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using Myra.Platform;
using System.Numerics;
using Texture2D = System.Object;
#endif

namespace Myra.Graphics2D
{
	public enum TextureFiltering
	{
		Nearest,
		Linear
	}

	public partial class RenderContext
	{
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
#elif STRIDE
		private static readonly RasterizerStateDescription _uiRasterizerState;

		static RenderContext()
		{
			var rs = new RasterizerStateDescription();
			rs.SetDefault();
			rs.ScissorTestEnable = true;
			_uiRasterizerState = rs;
		}
#endif

#if MONOGAME || FNA || STRIDE
		private readonly SpriteBatch _renderer;
#else
		private readonly IMyraRenderer _renderer;
		private readonly FontStashRenderer _fontStashRenderer;
#endif
		private bool _beginCalled;
		private Rectangle _scissor;
		private TextureFiltering _textureFiltering = TextureFiltering.Nearest;
		public Transform Transform;

		public Rectangle Scissor
		{
			get
			{
				return _scissor;
			}

			set
			{
				_scissor = value;

				if (MyraEnvironment.DisableClipping)
				{
					return;
				}

#if MONOGAME || FNA
				Flush();
				var device = _renderer.GraphicsDevice;
				value.X += device.Viewport.X;
				value.Y += device.Viewport.Y;
				device.ScissorRectangle = value;
#elif STRIDE
				Flush();
				MyraEnvironment.Game.GraphicsContext.CommandList.SetScissorRectangle(value);
#else
				_renderer.Scissor = value;
#endif
			}
		}

		public float Opacity { get; set; }

		public Rectangle View => new Rectangle(AbsoluteView.X - Transform.Offset.X, AbsoluteView.Y - Transform.Offset.Y, AbsoluteView.Width, AbsoluteView.Height);

		public Rectangle AbsoluteView { get; set; }

		public RenderContext()
		{
#if MONOGAME || FNA || STRIDE
			_renderer = new SpriteBatch(MyraEnvironment.Game.GraphicsDevice);
#else
			_renderer = MyraEnvironment.Platform.CreateRenderer();
			_fontStashRenderer = new FontStashRenderer(_renderer);
#endif
			_scissor = GetDeviceScissor();
			Transform.Reset();
		}

		/// <summary>
		/// Applies opacity
		/// </summary>
		/// <param name="opacity"></param>
		public void AddOpacity(float opacity)
		{
			Opacity *= opacity;
		}

		private Rectangle GetDeviceScissor()
		{
#if MONOGAME || FNA
			var device = _renderer.GraphicsDevice;
			var rect = device.ScissorRectangle;

			rect.X -= device.Viewport.X;
			rect.Y -= device.Viewport.Y;

			return rect;
#elif STRIDE
			return MyraEnvironment.Game.GraphicsContext.CommandList.Scissor;
#else
			return _renderer.Scissor;
#endif
		}

		private void SetTextureFiltering(TextureFiltering value)
		{
			if (_textureFiltering == value)
			{
				return;
			}

			_textureFiltering = value;
			Flush();
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
			SetTextureFiltering(TextureFiltering.Nearest);
			color = CrossEngineStuff.MultiplyColor(color, Opacity);

			if (sourceRectangle == null)
			{

#if MONOGAME || FNA || STRIDE
				sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
#else
				var sz = _fontStashRenderer.TextureManager.GetTextureSize(texture);
				sourceRectangle = new Rectangle(0, 0, sz.X, sz.Y);
#endif
			}

			var pos = Transform.Apply(new Vector2(destinationRectangle.X, destinationRectangle.Y));

			var scale = Transform.Scale;
			scale.X *= (float)destinationRectangle.Width / sourceRectangle.Value.Width;
			scale.Y *= (float)destinationRectangle.Height / sourceRectangle.Value.Height;
#if MONOGAME || FNA || STRIDE
			_renderer.Draw(texture, pos, sourceRectangle, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
#else
			_renderer.Draw(texture, pos, sourceRectangle, color, 0.0f, Vector2.Zero, scale, 0.0f);
#endif
		}

#if MONOGAME || FNA || STRIDE
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
		{
			destinationRectangle = Transform.Apply(destinationRectangle);
			SetTextureFiltering(TextureFiltering.Nearest);
			color = CrossEngineStuff.MultiplyColor(color, Opacity);

			_renderer.Draw(texture, destinationRectangle, color);
		}

		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			position = Transform.Apply(position);
			SetTextureFiltering(TextureFiltering.Nearest);
			color = CrossEngineStuff.MultiplyColor(color, Opacity);

			_renderer.Draw(texture, position, color);
		}
		
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
			position = Transform.Apply(position);
			SetTextureFiltering(TextureFiltering.Nearest);
			color = CrossEngineStuff.MultiplyColor(color, Opacity);

			_renderer.Draw(texture, position, sourceRectangle, color);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
		{
			destinationRectangle = Transform.Apply(destinationRectangle);
			SetTextureFiltering(TextureFiltering.Nearest);
			color = CrossEngineStuff.MultiplyColor(color, Opacity);
#if MONOGAME || FNA
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
#else
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, ImageOrientation.AsIs, layerDepth);
#endif
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
			position = Transform.Apply(position);
			SetTextureFiltering(TextureFiltering.Nearest);
			color = CrossEngineStuff.MultiplyColor(color, Opacity);
#if MONOGAME || FNA
			_renderer.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
#else
			_renderer.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, ImageOrientation.AsIs, layerDepth);
#endif
		}
#endif

		private void SetTextTextureFiltering()
		{
			if (!MyraEnvironment.SmoothText)
			{
				SetTextureFiltering(TextureFiltering.Nearest);
			}
			else
			{
				SetTextureFiltering(TextureFiltering.Linear);
			}
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="rotation">A rotation of this text in radians.</param>
		/// <param name="origin">Center of the rotation.</param>
		/// <param name="scale">A scaling of this text.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, Vector2 scale, float rotation, Vector2 origin, float layerDepth = 0.0f)
		{
			position = Transform.Apply(position);
			SetTextTextureFiltering();
			color = CrossEngineStuff.MultiplyColor(color, Opacity);

#if MONOGAME || FNA || STRIDE
			font.DrawText(_renderer, text, position, color, Transform.Scale * scale, rotation, origin, layerDepth);
#else
			font.DrawText(_fontStashRenderer, text, position, color, scale, rotation, origin, layerDepth);
#endif
		}

		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, Vector2 scale, float layerDepth = 0.0f)
		{
			DrawString(font, text, position, color, scale, 0, Vector2.Zero, layerDepth);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, float layerDepth = 0.0f)
		{
			DrawString(font, text, position, color, Vector2.One, 0, Vector2.Zero, layerDepth);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="colors">Colors of glyphs.</param>
		/// <param name="rotation">A rotation of this text in radians.</param>
		/// <param name="origin">Center of the rotation.</param>
		/// <param name="scale">A scaling of this text.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color[] colors, Vector2 scale, float rotation, Vector2 origin, float layerDepth = 0.0f)
		{
			position = Transform.Apply(position);
			SetTextTextureFiltering();

#if MONOGAME || FNA || STRIDE
			font.DrawText(_renderer, text, position, colors, Transform.Scale * scale, rotation, origin, layerDepth);
#else
			font.DrawText(_fontStashRenderer, text, position, colors, scale, rotation, origin, layerDepth);
#endif
		}

		public void Begin()
		{
#if MONOGAME || FNA
			var samplerState = _textureFiltering == TextureFiltering.Nearest ? SamplerState.PointClamp : SamplerState.LinearClamp;

			_renderer.Begin(SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				samplerState,
				null,
				UIRasterizerState,
				null);
#elif STRIDE
			var samplerState = _textureFiltering == TextureFiltering.Nearest ?
				MyraEnvironment.Game.GraphicsDevice.SamplerStates.PointClamp :
				MyraEnvironment.Game.GraphicsDevice.SamplerStates.LinearClamp;
			_renderer.Begin(MyraEnvironment.Game.GraphicsContext,
				SpriteSortMode.Deferred,
				BlendStates.AlphaBlend,
				samplerState,
				null,
				_uiRasterizerState);
#else
			_renderer.Begin(_textureFiltering);
#endif

			_beginCalled = true;
		}

		public void End()
		{
			_renderer.End();
			_beginCalled = false;
		}

		public void Flush()
		{
			if (!_beginCalled)
			{
				return;
			}

			End();
			Begin();
		}
	}
}