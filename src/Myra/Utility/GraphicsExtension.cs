using System;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Texture2D = Xenko.Graphics.Texture;
#endif

namespace Myra.Utility
{
	public static class GraphicsExtension
	{
		public static Point Size(this Rectangle r)
		{
			return new Point(r.Width, r.Height);
		}

		public static void SetSize(ref Rectangle r, Point size)
		{
			r.Width = size.X;
			r.Height = size.Y;
		}

		public static Rectangle Add(this Rectangle r, Point p)
		{
			return new Rectangle(r.X + p.X, r.Y + p.Y, r.Width, r.Height);
		}

		/// <summary>
		/// Converts RGB to HSV color system
		/// </summary>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <returns>hue(0-360), saturation(0-100) and value(0-100)</returns>
		public static ColorHSV ToHSV(this Color color)
		{
			var r = color.R / 255.0f;
			var g = color.G / 255.0f;
			var b = color.B / 255.0f;

			float h, s, v;
			float min, max, delta;

			min = Math.Min(Math.Min(r, g), b);
			max = Math.Max(Math.Max(r, g), b);

			v = max;

			delta = max - min;

			if (max != 0)
				s = delta / max;
			else
			{
				s = 0;
				h = 0;

				return new ColorHSV
				{
					H = (int)Math.Round(h),
					S = (int)Math.Round(s),
					V = (int)Math.Round(v),
				};
			}

			if (delta == 0)
				h = 0;
			else
			{

				if (r == max)
					h = (g - b) / delta;
				else if (g == max)
					h = 2 + (b - r) / delta;
				else
					h = 4 + (r - g) / delta;
			}

			h *= 60;
			if (h < 0)
				h += 360;

			s *= 100;
			v *= 100;

			return new ColorHSV
			{
				H = (int)Math.Round(h),
				S = (int)Math.Round(s),
				V = (int)Math.Round(v),
			};
		}

		/// <summary>
		/// Converts HSV color system to RGB
		/// </summary>
		/// <returns></returns>
		public static Color ToRGB(this ColorHSV colorHSV)
		{
			float h = colorHSV.H;
			if (colorHSV.H == 360) h = 359;
			float s = colorHSV.S;
			float v = colorHSV.V;

			int i;
			float f, p, q, t;
			h = Math.Max(0.0f, Math.Min(360.0f, h));
			s = Math.Max(0.0f, Math.Min(100.0f, s));
			v = Math.Max(0.0f, Math.Min(100.0f, v));
			s /= 100;
			v /= 100;
			h /= 60;
			i = (int)Math.Floor(h);
			f = h - i;
			p = v * (1 - s);
			q = v * (1 - s * f);
			t = v * (1 - s * (1 - f));

			int r, g, b;
			switch (i)
			{
				case 0:
					r = (int)Math.Round(255 * v);
					g = (int)Math.Round(255 * t);
					b = (int)Math.Round(255 * p);
					break;
				case 1:
					r = (int)Math.Round(255 * q);
					g = (int)Math.Round(255 * v);
					b = (int)Math.Round(255 * p);
					break;
				case 2:
					r = (int)Math.Round(255 * p);
					g = (int)Math.Round(255 * v);
					b = (int)Math.Round(255 * t);
					break;
				case 3:
					r = (int)Math.Round(255 * p);
					g = (int)Math.Round(255 * q);
					b = (int)Math.Round(255 * v);
					break;
				case 4:
					r = (int)Math.Round(255 * t);
					g = (int)Math.Round(255 * p);
					b = (int)Math.Round(255 * v);
					break;
				default:
					r = (int)Math.Round(255 * v);
					g = (int)Math.Round(255 * p);
					b = (int)Math.Round(255 * q);
					break;
			}

			return new Color((byte)r, (byte)g, (byte)b, (byte)255);
		}

		private static byte ApplyAlpha(byte color, byte alpha)
		{
			var fc = color / 255.0f;
			var fa = alpha / 255.0f;
			var fr = (int)(255.0f * fc * fa);
			if (fr < 0)
			{
				fr = 0;
			}
			if (fr > 255)
			{
				fr = 255;
			}
			return (byte)fr;
		}

		internal static Color ApplyAlpha(this Color src)
		{
			byte a = src.A;

			src.R = ApplyAlpha(src.R, a);
			src.G = ApplyAlpha(src.G, a);
			src.B = ApplyAlpha(src.B, a);

			return src;
		}

		/// <summary>
		/// Updates texture colors by alpha
		/// </summary>
		/// <param name="texture"></param>
		public static void PremultiplyAlpha(this Texture2D texture)
		{
			var data = CrossEngineStuff.GetData(texture);

			for (int i = 0; i < data.Length; ++i)
			{
				data[i] = ApplyAlpha(data[i]);
			}

			CrossEngineStuff.SetData(texture, data);
		}
	}
}