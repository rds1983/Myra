using System;
using FontStashSharp;
using FontStashSharp.RichText;

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

#if PLATFORM_AGNOSTIC
using Matrix = System.Numerics.Matrix3x2;
#endif

namespace Myra.Graphics2D
{
	/// <summary>
	/// Provides rendering context for drawing 2D graphics including shapes, text, and textured regions.
	/// </summary>
	public partial class RenderContext : IDisposable
	{
		private readonly Renderer _renderer = new Renderer();

		public Rectangle Scissor
		{
			get => _renderer.Scissor;

			set => _renderer.Scissor = value;
		}

		/// <summary>
		/// Gets or sets the opacity (alpha) value for rendering, ranging from 0.0 (fully transparent) to 1.0 (fully opaque).
		/// </summary>
		public float Opacity
		{
			get => _renderer.Opacity;

			set => _renderer.Opacity = value;
		}

		internal Transform Transform
		{
			get => _renderer.Transform;
			set => _renderer.Transform = value;
		}

		public void Dispose()
		{
			_renderer.Dispose();
			GC.SuppressFinalize(this);
		}

		public void AddOpacity(float opacity) => _renderer.AddOpacity(opacity);

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, float depth = 0.0f) => 
			_renderer.Draw(texture, destinationRectangle, sourceRectangle, color, rotation, depth);

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation) => Draw(texture, destinationRectangle, sourceRectangle, color, rotation, 0.0f);

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color) => Draw(texture, destinationRectangle, sourceRectangle, color, 0);

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color) => Draw(texture, destinationRectangle, null, color, 0);

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 scale, float depth = 0.0f) =>
			_renderer.Draw(texture, position, sourceRectangle, color, rotation, scale, depth);

		public void Draw(Texture2D texture, Vector2 pos, Color color, Vector2 scale, float rotation = 0.0f) =>
			Draw(texture, pos, null, color, rotation, scale);

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation) =>
			Draw(texture, position, sourceRectangle, color, rotation, Vector2.One);

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color) =>
			Draw(texture, position, sourceRectangle, color, 0, Vector2.One);

		public void Draw(Texture2D texture, Vector2 position, Color color) =>
			Draw(texture, position, null, color, 0, Vector2.One);

		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, Vector2 scale, float rotation, float layerDepth = 0.0f) =>
			_renderer.DrawString(font, text, position, color, scale, rotation, layerDepth);

		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, Vector2 scale, float layerDepth = 0.0f) =>
			DrawString(font, text, position, color, scale, 0, layerDepth);

		public void DrawString(SpriteFontBase font, string text, Vector2 position, Color color, float layerDepth = 0.0f) =>
			DrawString(font, text, position, color, Vector2.One, 0, layerDepth);

		public void DrawRichText(RichTextLayout richText, Vector2 position, Color color,
			Vector2? sourceScale = null, float rotation = 0, float layerDepth = 0.0f,
			TextHorizontalAlignment horizontalAlignment = TextHorizontalAlignment.Left) =>
			_renderer.DrawRichText(richText, position, color, sourceScale, rotation, layerDepth, horizontalAlignment);

		public void Begin() => _renderer.Begin();

		public void End() => _renderer.End();
	}
}