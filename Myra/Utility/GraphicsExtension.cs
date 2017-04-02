using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Myra.Utility
{
	public static class GraphicsExtension
	{
		private static readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();

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
				_colors[c.Name.ToLower()] = value;
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
				Color result;
				if (_colors.TryGetValue(name.ToLower(), out result))
				{
					return result;
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
	}
}