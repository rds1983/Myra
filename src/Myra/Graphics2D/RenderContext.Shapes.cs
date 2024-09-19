// This code had been borrowed from the MonoGame.Extended project: https://github.com/craftworkgames/MonoGame.Extended

using System;
using Myra.Utility;
using System.Runtime.CompilerServices;
using Myra.Graphics2D.UI.Styles;
using Myra.Graphics2D.TextureAtlases;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Graphics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using System.Numerics;
using Texture2D = System.Object;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D
{
	partial class RenderContext
	{
		/// <summary>
		///     Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public void FillRectangle(Rectangle rectangle, Color color) =>
			FillRectangle(new Vector2(rectangle.X, rectangle.Y), new Vector2(rectangle.Width, rectangle.Height), color);

		/// <summary>
		///     Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="location">Where to draw</param>
		/// <param name="size">The size of the rectangle</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public void FillRectangle(Vector2 location, Vector2 size, Color color) =>
			Stylesheet.Current.WhiteRegion.Draw(this,
				new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y),
				color);

		/// <summary>
		///     Draws a filled rectangle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="x">The X coord of the left side</param>
		/// <param name="y">The Y coord of the upper side</param>
		/// <param name="width">Width</param>
		/// <param name="height">Height</param>
		/// <param name="color">The color to draw the rectangle in</param>
		public void FillRectangle(float x, float y, float width, float height, Color color) =>
			FillRectangle(new Vector2(x, y), new Vector2(width, height), color);

		/// <summary>
		///     Draws a rectangle with the thickness provided
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="rectangle">The rectangle to draw</param>
		/// <param name="color">The color to draw the rectangle in</param>
		/// <param name="thickness">The thickness of the lines</param>
		public void DrawRectangle(Rectangle rectangle, Color color, float thickness = 1f)
		{
			var region = Stylesheet.Current.WhiteRegion;
			var t = (int)thickness;

			var c = CrossEngineStuff.MultiplyColor(color, Opacity);

			// Top
			region.Draw(this, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, t), c);

			// Bottom
			region.Draw(this, new Rectangle(rectangle.X, rectangle.Bottom - t, rectangle.Width, t), c);

			// Left
			region.Draw(this, new Rectangle(rectangle.X, rectangle.Y, t, rectangle.Height), c);

			// Right
			region.Draw(this, new Rectangle(rectangle.Right - t, rectangle.Y, t, rectangle.Height), c);
		}

		/// <summary>
		///     Draws a rectangle with the thickness provided
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="location">Where to draw</param>
		/// <param name="size">The size of the rectangle</param>
		/// <param name="color">The color to draw the rectangle in</param>
		/// <param name="thickness">The thickness of the line</param>
		public void DrawRectangle(Vector2 location, Vector2 size, Color color, float thickness = 1f)
		{
			DrawRectangle(new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y), color,
				thickness);
		}

		/// <summary>
		///     Draws a closed polygon from an array of points
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// ///
		/// <param name="offset">Where to offset the points</param>
		/// <param name="points">The points to connect with lines</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the lines</param>
		public void DrawPolygon(Vector2 offset, Vector2[] points, Color color, float thickness = 1f)
		{
			if (points.Length == 0)
				return;

			if (points.Length == 1)
			{
				DrawPoint(points[0], color, (int)thickness);
				return;
			}

			var whiteRegion = Stylesheet.Current.WhiteRegion;

			for (var i = 0; i < points.Length - 1; i++)
				DrawPolygonEdge(whiteRegion, points[i] + offset, points[i + 1] + offset, color, thickness);

			DrawPolygonEdge(whiteRegion, points[points.Length - 1] + offset, points[0] + offset, color,
				thickness);
		}

		private void DrawPolygonEdge(TextureRegion region, Vector2 point1, Vector2 point2, Color color, float thickness)
		{
			var length = Vector2.Distance(point1, point2);
			var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

			var scale = new Vector2(length, thickness);
			Draw(region.Texture, point1, region.Bounds, color, angle, scale);
		}

		/// <summary>
		///     Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="x1">The X coord of the first point</param>
		/// <param name="y1">The Y coord of the first point</param>
		/// <param name="x2">The X coord of the second point</param>
		/// <param name="y2">The Y coord of the second point</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the line</param>
		public void DrawLine(float x1, float y1, float x2, float y2, Color color, float thickness = 1f) =>
			DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness);

		/// <summary>
		///     Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="point1">The first point</param>
		/// <param name="point2">The second point</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the line</param>
		public void DrawLine(Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
		{
			// calculate the distance between the two vectors
			var distance = Vector2.Distance(point1, point2);

			// calculate the angle between the two vectors
			var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

			DrawLine(point1, distance, angle, color, thickness);
		}

		/// <summary>
		///     Draws a line from point1 to point2 with an offset
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="point">The starting point</param>
		/// <param name="length">The length of the line</param>
		/// <param name="angle">The angle of this line from the starting point</param>
		/// <param name="color">The color to use</param>
		/// <param name="thickness">The thickness of the line</param>
		public void DrawLine(Vector2 point, float length, float angle, Color color, float thickness = 1f)
		{
			var scale = new Vector2(length, thickness);

			point.Y -= (int)(thickness * Math.Cos(angle) / 2.0f);

			var whiteRegion = Stylesheet.Current.WhiteRegion;
			Draw(whiteRegion.Texture, point, whiteRegion.Bounds, color, angle, scale, 0);
		}

		/// <summary>
		///     Draws a point at the specified x, y position. The center of the point will be at the position.
		/// </summary>
		public void DrawPoint(float x, float y, Color color, float size = 1f)
		{
			DrawPoint(new Vector2(x, y), color, size);
		}

		/// <summary>
		///     Draws a point at the specified position. The center of the point will be at the position.
		/// </summary>
		public void DrawPoint(Vector2 position, Color color, float size = 1f)
		{
			var scale = Vector2.One * size;
			var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);

			var whiteRegion = Stylesheet.Current.WhiteRegion;
			Draw(whiteRegion.Texture, position + offset, whiteRegion.Bounds, color, 0.0f, scale);
		}

		/// <summary>
		///     Draw a circle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="center">The center of the circle</param>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <param name="color">The color of the circle</param>
		/// <param name="thickness">The thickness of the lines used</param>
		public void DrawCircle(Vector2 center, float radius, int sides, Color color, float thickness = 1f) =>
			DrawPolygon(center, CreateCircle(radius, sides), color, thickness);

		/// <summary>
		///     Draw a circle
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="x">The center X of the circle</param>
		/// <param name="y">The center Y of the circle</param>
		/// <param name="radius">The radius of the circle</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <param name="color">The color of the circle</param>
		/// <param name="thickness">The thickness of the line</param>
		public void DrawCircle(float x, float y, float radius, int sides, Color color, float thickness = 1f) =>
			DrawPolygon(new Vector2(x, y), CreateCircle(radius, sides), color, thickness);

		/// <summary>
		///     Draw an Arc
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="center">The center of the Arc</param>
		/// <param name="radius">The radius of the Arc</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <param name="color">The color of the Arc</param>
		/// <param name="thickness">The thickness of the lines used</param>
		/// <param name="startAngle">The start angle of the line in radians</param>
		/// <param name="endAngle">The end angle of the line in radians</param>
		public void DrawArc(Vector2 center, float radius, int sides, Color color, float startAngle, float endAngle, float thickness = 1f) =>
			DrawPolygon(center, CreateArc(radius, sides, startAngle, endAngle), color, thickness);

		/// <summary>
		///     Draw a Arc
		/// </summary>
		/// <param name="spriteBatch">The destination drawing surface</param>
		/// <param name="x">The center X of the Arc</param>
		/// <param name="y">The center Y of the Arc</param>
		/// <param name="radius">The radius of the Arc</param>
		/// <param name="sides">The number of sides to generate</param>
		/// <param name="color">The color of the Arc</param>
		/// <param name="thickness">The thickness of the line</param>
		/// <param name="startAngle">The start angle of the line in radians</param>
		/// <param name="endAngle">The end angle of the line in radians</param>
		public void DrawArc(float x, float y, float radius, int sides, Color color, float startAngle, float endAngle, float thickness = 1f) =>
			DrawPolygon(new Vector2(x, y), CreateArc(radius, sides, startAngle, endAngle), color, thickness);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector2[] CreateArcHelper(double radius, int sides, double step, double theta = 0.0)
		{
			var points = new Vector2[sides];
			for (var i = 0; i < sides; i++)
			{
				points[i] = new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
				theta += step;
			}
			return points;
		}

		private Vector2[] CreateCircle(double radius, int sides)
		{
			const double max = 2.0 * Math.PI;
			var step = max / sides;
			return CreateArcHelper(radius, sides, step);
		}

		private Vector2[] CreateArc(double radius, int sides, double startAngle, double endAngle)
		{
			var max = Math.Max(endAngle - startAngle, 0);
			var step = max / sides;
			var theta = startAngle;
			return CreateArcHelper(radius, sides, step, theta);
		}
	}
}