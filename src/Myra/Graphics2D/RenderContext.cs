using System;
using FontStashSharp;
using Myra.Utility;
using FontStashSharp.RichText;
using FontStashSharp.Interfaces;
using info.lundin.math;

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
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D
{
	public enum TextureFiltering
	{
		Nearest,
		Linear
	}

	public partial class RenderContext : IDisposable
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
		private readonly FontStashRenderer2 _fontStashRenderer2;
		private VertexPositionColorTexture _topLeft = new VertexPositionColorTexture(), 
			_topRight = new VertexPositionColorTexture(),
			_bottomLeft = new VertexPositionColorTexture(),
			_bottomRight = new VertexPositionColorTexture();
#endif
		private bool _beginCalled;
		private Rectangle _scissor;
		private TextureFiltering _textureFiltering = TextureFiltering.Nearest;
		public Transform Transform;

		internal Rectangle DeviceScissor
		{
			get
			{
#if MONOGAME || FNA
				var device = _renderer.GraphicsDevice;
				return device.ScissorRectangle;
#elif STRIDE
				return MyraEnvironment.Game.GraphicsContext.CommandList.Scissor;
#else
				return _renderer.Scissor;
#endif
			}

			set
			{
#if MONOGAME || FNA
				var device = _renderer.GraphicsDevice;
				device.ScissorRectangle = value;
#elif STRIDE
				Flush();
				MyraEnvironment.Game.GraphicsContext.CommandList.SetScissorRectangle(value);
#else
				_renderer.Scissor = value;
#endif
			}
		}


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
#elif STRIDE
				Flush();
#else
#endif

				DeviceScissor = value;
			}
		}

		public float Opacity { get; set; }

		public RenderContext()
		{
#if MONOGAME || FNA || STRIDE
			_renderer = new SpriteBatch(MyraEnvironment.Game.GraphicsDevice);
#else
			_renderer = MyraEnvironment.Platform.Renderer;

			if (_renderer.RendererType == RendererType.Sprite)
			{
				_fontStashRenderer = new FontStashRenderer(_renderer);
				_fontStashRenderer2 = null;
			}
			else
			{
				_fontStashRenderer = null;
				_fontStashRenderer2 = new FontStashRenderer2(_renderer);
			}
#endif
		}

		/// <summary>
		/// Applies opacity
		/// </summary>
		/// <param name="opacity"></param>
		public void AddOpacity(float opacity)
		{
			Opacity *= opacity;
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

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="destinationRectangle"></param>
		/// <param name="sourceRectangle"></param>
		/// <param name="color"></param>
		/// <param name="rotation"></param>
		/// <param name="depth"></param>
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, float depth = 0.0f)
		{
			Vector2 sz;
			if (sourceRectangle != null)
			{
				sz = new Vector2(sourceRectangle.Value.Width, sourceRectangle.Value.Height);
			}
			else
			{
#if MONOGAME || FNA || STRIDE
				sz = new Vector2(texture.Width, texture.Height);
#else
				Point p;
				if (_fontStashRenderer != null)
				{
					p = _fontStashRenderer.TextureManager.GetTextureSize(texture);
				} else
				{
					p = _fontStashRenderer2.TextureManager.GetTextureSize(texture);
				}

				sz = new Vector2(p.X, p.Y);
#endif
			}

			var pos = new Vector2(destinationRectangle.X, destinationRectangle.Y);
			var scale = new Vector2(destinationRectangle.Width / sz.X, destinationRectangle.Height / sz.Y);
			Draw(texture, pos, sourceRectangle, color, rotation, scale, depth);
		}

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="destinationRectangle"></param>
		/// <param name="sourceRectangle"></param>
		/// <param name="color"></param>
		/// <param name="rotation"></param>
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation) => Draw(texture, destinationRectangle, sourceRectangle, color, rotation, 0.0f);

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="destinationRectangle"></param>
		/// <param name="sourceRectangle"></param>
		/// <param name="color"></param>
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) => Draw(texture, destinationRectangle, sourceRectangle, color, 0);

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="destinationRectangle"></param>
		/// <param name="color"></param>
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color) => Draw(texture, destinationRectangle, null, color, 0);

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="position"></param>
		/// <param name="sourceRectangle"></param>
		/// <param name="color"></param>
		/// <param name="rotation"></param>
		/// <param name="depth"></param>
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 scale, float depth = 0.0f)
		{
			SetTextureFiltering(TextureFiltering.Nearest);
			color = CrossEngineStuff.MultiplyColor(color, Opacity);
			scale *= Transform.Scale;
			rotation += Transform.Rotation;

#if MONOGAME || FNA
			position = Transform.Apply(position);

			_renderer.Draw(texture, position, sourceRectangle, color, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
#elif STRIDE
			position = Transform.Apply(position);

			_renderer.Draw(texture, position, sourceRectangle, color, rotation, Vector2.Zero, scale, SpriteEffects.None, ImageOrientation.AsIs, depth);
#else
			if (_fontStashRenderer != null)
			{
				position = Transform.Apply(position);
				_renderer.DrawSprite(texture, position, sourceRectangle, color, rotation, scale, depth);
			}
			else
			{
				Rectangle r;
				if (sourceRectangle != null)
				{
					r = sourceRectangle.Value;
				} else
				{
					var textureSize = _fontStashRenderer2.TextureManager.GetTextureSize(texture);
					r = new Rectangle(0, 0, textureSize.X, textureSize.Y);
				}

				var size = new Vector2(scale.X * r.Width, scale.Y * r.Height);
				_renderer.DrawQuad(texture, color, position, ref Transform.Matrix, depth, size, r,
					ref _topLeft, ref _topRight, ref _bottomLeft, ref _bottomRight);
			}
#endif
		}

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="pos"></param>
		/// <param name="color"></param>
		/// <param name="scale"></param>
		/// <param name="rotation"></param>
		public void Draw(Texture2D texture, Vector2 pos, Color color, Vector2 scale, float rotation = 0.0f) =>
			Draw(texture, pos, null, color, rotation, scale);

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="position"></param>
		/// <param name="sourceRectangle"></param>
		/// <param name="color"></param>
		/// <param name="rotation"></param>
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation) =>
			Draw(texture, position, sourceRectangle, color, rotation, Vector2.One);

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="position"></param>
		/// <param name="sourceRectangle"></param>
		/// <param name="color"></param>
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) =>
			Draw(texture, position, sourceRectangle, color, 0, Vector2.One);

		/// <summary>
		/// Draws a texture
		/// </summary>
		/// <param name="texture"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public void Draw(Texture2D texture, Vector2 position, Color color) =>
			Draw(texture, position, null, color, 0, Vector2.One);

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
		/// <param name="scale">A scaling of this text.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, Vector2 scale, float rotation, float layerDepth = 0.0f)
		{
			SetTextTextureFiltering();
			color = CrossEngineStuff.MultiplyColor(color, Opacity);
			position = Transform.Apply(position);

			scale *= Transform.Scale;
			rotation += Transform.Rotation;

#if MONOGAME || FNA || STRIDE
			font.DrawText(_renderer, text, position, color, rotation, Vector2.Zero, scale, layerDepth);
#else
			if (_fontStashRenderer != null)
			{
				font.DrawText(_fontStashRenderer, text, position, color, rotation, Vector2.Zero, scale);
			}
			else
			{
				font.DrawText(_fontStashRenderer2, text, position, color, rotation, Vector2.Zero, scale);
			}
#endif
		}

		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, Vector2 scale, float layerDepth = 0.0f) =>
			DrawString(font, text, position, color, scale, 0, layerDepth);

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, float layerDepth = 0.0f) =>
			DrawString(font, text, position, color, Vector2.One, 0, layerDepth);

		/// <summary>
		/// Draws a rich text
		/// </summary>
		/// <param name="richText">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="sourceScale">A scaling of this text.</param>
		/// <param name="rotation">A rotation of this text in radians.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawRichText(RichTextLayout richText, Vector2 position, Color color,
			Vector2? sourceScale = null, float rotation = 0, float layerDepth = 0.0f,
			TextHorizontalAlignment horizontalAlignment = TextHorizontalAlignment.Left)
		{
			SetTextTextureFiltering();
			color = CrossEngineStuff.MultiplyColor(color, Opacity);
			position = Transform.Apply(position);

			var scale = sourceScale ?? Vector2.One;

			scale *= Transform.Scale;
			rotation += Transform.Rotation;

#if MONOGAME || FNA || STRIDE
			richText.Draw(_renderer, position, color, rotation, Vector2.Zero, scale, layerDepth, horizontalAlignment);
#else
			if (_fontStashRenderer != null)
			{
				richText.Draw(_fontStashRenderer, position, color, rotation, Vector2.Zero, scale, layerDepth, horizontalAlignment);
			}
			else
			{
				richText.Draw(_fontStashRenderer2, position, color, rotation, Vector2.Zero, scale, layerDepth, horizontalAlignment);
			}
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

		private void ReleaseUnmanagedResources()
		{
#if MONOGAME || FNA || STRIDE
			_renderer?.Dispose();
#endif
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~RenderContext()
		{
			ReleaseUnmanagedResources();
		}
	}
}