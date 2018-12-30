using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Myra.Utility
{
	public static class GraphicsExtension
	{
		private class ColorInfo
		{
			public Color Color { get; set; }
			public string Name { get; set; }
		}

		private static readonly Dictionary<string, ColorInfo> _colors = new Dictionary<string, ColorInfo>();

		static GraphicsExtension()
		{
			var type = typeof (Color);

			var colors = type.GetRuntimeProperties();

			foreach (var c in colors)
			{
				if (!c.GetMethod.IsStatic &&
				    c.PropertyType != typeof (Color))
				{
					continue;
				}

				var value = (Color) c.GetValue(null, null);
				_colors[c.Name.ToLower()] = new ColorInfo
				{
					Color = value,
					Name = c.Name
				};
			}
		}

		public static string ToHexString(this Color c)
		{
			return string.Format("#{0}{1}{2}{3}",
				c.R.ToString("X2"),
				c.G.ToString("X2"),
				c.B.ToString("X2"),
				c.A.ToString("X2"));
		}

		public static string GetColorName(this Color color)
		{
			foreach (var c in _colors)
			{
				if (c.Value.Color == color)
				{
					return c.Value.Name;
				}
			}

			return null;
		}

		public static Color? FromName(this string name)
		{
			if (name.StartsWith("#"))
			{
				name = name.Substring(1);
				uint u;
				if (uint.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out u))
				{
					// Parsed value contains color in RGBA form
					// Extract color components

					byte r, g, b, a;

					unchecked
					{
						r = (byte) (u >> 24);
						g = (byte) (u >> 16);
						b = (byte) (u >> 8);
						a = (byte) u;
					}

					return new Color(r, g, b, a);
				}
			}
			else
			{
				ColorInfo result;
				if (_colors.TryGetValue(name.ToLower(), out result))
				{
					return result.Color;
				}
			}

			return null;
		}

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

			return new Color(r, g, b, 255);
		}
	}
}