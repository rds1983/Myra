using FontStashSharp;
using System;
using Myra.Utility;
using System.Text;

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
using Matrix = System.Numerics.Matrix3x2;
#endif

namespace Myra.Graphics2D
{
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
#endif
		private bool _beginCalled;
		private Rectangle _scissor;
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
#if MONOGAME || FNA || STRIDE
					InverseTransform = Matrix.Invert(_transform.Value);
#else
					Matrix inverse = Matrix.Identity;
					Matrix.Invert(_transform.Value, out inverse);
					InverseTransform = inverse;
#endif
				}

				Flush();
			}
		}

		internal Matrix InverseTransform { get; set; }

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

				if (Transform != null)
				{
					var pos = Vector2.Transform(new Vector2(value.X, value.Y), Transform.Value);
					var size = Vector2.Transform(new Vector2(value.Width, value.Height), Transform.Value);

					value = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
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

		public Rectangle View { get; set; }

		public float Opacity { get; set; }

		public RenderContext()
		{
#if MONOGAME || FNA || STRIDE
			_renderer = new SpriteBatch(MyraEnvironment.Game.GraphicsDevice);
#else
			_renderer = MyraEnvironment.Platform.CreateRenderer();
#endif

			_scissor = GetDeviceScissor();
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

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
		{
#if MONOGAME || FNA
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color);
#elif STRIDE
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero);
#else
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color);
#endif
		}

#if MONOGAME || FNA || STRIDE
		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
		{
			_renderer.Draw(texture, destinationRectangle, color);
		}

		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			_renderer.Draw(texture, position, color);
		}
		
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
			_renderer.Draw(texture, position, sourceRectangle, color);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, SpriteEffects effects, float layerDepth)
		{
#if MONOGAME || FNA
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, layerDepth);
#else
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, origin, effects, ImageOrientation.AsIs, layerDepth);
#endif
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
		{
#if MONOGAME || FNA
			_renderer.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
#else
			_renderer.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, ImageOrientation.AsIs, layerDepth);
#endif
		}
#endif

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
			font.DrawText(_renderer, text, position, color, scale, rotation, origin, layerDepth);
		}

		/// <summary>
		/// Draws a text
		/// </summary>
		/// <param name="text">The text which will be drawn.</param>
		/// <param name="position">The drawing location on screen.</param>
		/// <param name="color">A color mask.</param>
		/// <param name="scale">A scaling of this text.</param>
		/// <param name="layerDepth">A depth of the layer of this string.</param>
		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, Vector2 scale, float layerDepth = 0.0f)
		{
			font.DrawText(_renderer, text, position, color, scale, layerDepth);
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
			font.DrawText(_renderer, text, position, color, layerDepth);
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
			font.DrawText(_renderer, text, position, colors, scale, rotation, origin, layerDepth);
		}

		public void Begin()
		{
#if MONOGAME || FNA
			_renderer.Begin(SpriteSortMode.Deferred,
				BlendState.AlphaBlend,
				SamplerState.PointClamp,
				null,
				UIRasterizerState,
				null,
				Transform);
#elif STRIDE
			if (Transform == null)
			{
				_renderer.Begin(MyraEnvironment.Game.GraphicsContext,
					SpriteSortMode.Deferred,
					BlendStates.AlphaBlend,
					MyraEnvironment.Game.GraphicsDevice.SamplerStates.PointClamp,
					null,
					_uiRasterizerState);
			} else
			{
				_renderer.Begin(MyraEnvironment.Game.GraphicsContext,
					Transform.Value,
					SpriteSortMode.Deferred,
					BlendStates.AlphaBlend,
					MyraEnvironment.Game.GraphicsDevice.SamplerStates.PointClamp,
					null,
					_uiRasterizerState);
			}
#else
			_renderer.Begin(Transform);
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