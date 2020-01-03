using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra
{
	public static class ColorStorage
	{
		public class ColorInfo
		{
			public Color Color { get; set; }
			public string Name { get; set; }
		}

		public static readonly Dictionary<string, ColorInfo> Colors = new Dictionary<string, ColorInfo>();

		static ColorStorage()
		{
			var type = typeof(Color);

			var colors = type.GetRuntimeProperties();

			foreach (var c in colors)
			{
				if (!c.GetMethod.IsStatic &&
					c.PropertyType != typeof(Color))
				{
					continue;
				}

				var value = (Color)c.GetValue(null, null);
				Colors[c.Name.ToLower()] = new ColorInfo
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
			foreach (var c in Colors)
			{
				if (c.Value.Color == color)
				{
					return c.Value.Name;
				}
			}

			return null;
		}

		public static Color? FromName(string name)
		{
			if (name.StartsWith("#"))
			{
				name = name.Substring(1);
				uint u;
				if (uint.TryParse(name, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out u))
				{
					// Parsed value contains color in RGBA form
					// Extract color components

					byte r = 0, g = 0, b = 0, a = 0;

					unchecked
					{
						if (name.Length == 6)
						{
							r = (byte)(u >> 16);
							g = (byte)(u >> 8);
							b = (byte)u;
							a = 255;
						}
						else if (name.Length == 8)
						{
							r = (byte)(u >> 24);
							g = (byte)(u >> 16);
							b = (byte)(u >> 8);
							a = (byte)u;
						}
					}

					return new Color(r, g, b, a);
				}
			}
			else
			{
				ColorInfo result;
				if (Colors.TryGetValue(name.ToLower(), out result))
				{
					return result.Color;
				}
			}

			return null;
		}
	}
}
