using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

		public static Texture2D PremultipliedTextureFromStream(Stream stream)
		{
			var texture = Texture2D.FromStream(MyraEnvironment.GraphicsDevice, stream);

			if (!MyraEnvironment.IsWindowsDX)
			{
				// Manually premultiply alpha
				var data = new Color[texture.Width * texture.Height];
				texture.GetData(data);

				for (var i = 0; i < data.Length; ++i)
				{
					data[i].R = ApplyAlpha(data[i].R, data[i].A);
					data[i].G = ApplyAlpha(data[i].G, data[i].A);
					data[i].B = ApplyAlpha(data[i].B, data[i].A);
				}

				texture.SetData(data);
			}

			return texture;
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
	}
}