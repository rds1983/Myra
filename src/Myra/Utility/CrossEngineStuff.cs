#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Graphics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
#endif

namespace Myra.Utility
{
	internal static class CrossEngineStuff
	{
		public static Color CreateColor(int r, int g, int b, int a = 255)
		{
#if MONOGAME || FNA || STRIDE
			return new Color((byte)r, (byte)g, (byte)b, (byte)a);
#else
			return Color.FromArgb(a, r, g, b);
#endif
		}

		public static Color MultiplyColor(Color color, float value)
		{
#if MONOGAME || FNA || STRIDE
			return color * value;
#else
			if (value < 0)
			{
				value = 0;
			}

			if (value > 1)
			{
				value = 1;
			}

			return Color.FromArgb((int)(color.A * value),
				(int)(color.R * value),
				(int)(color.G * value),
				(int)(color.G * value));
#endif
		}
	}
}
