using Microsoft.Xna.Framework;

namespace Myra.Samples;

/// <summary>
/// Colour and math helpers used by the NvgSharp demo drawing routines.
/// Provides platform-agnostic colour construction and HSL-to-RGB conversion.
/// </summary>
internal static class Utility
{
	public static float Mod(float a, float b)
	{
		return (float)(a % b);
	}

	public static float Clamp(float a, float mn, float mx)
	{
		return (float)((a) < (mn) ? mn : ((a) > (mx) ? mx : a));
	}

	/// <summary>
	/// Computes a single HSL channel component given a hue value and two mix constants.
	/// Used internally by <see cref="HSLA"/>.
	/// </summary>
	public static float Hue(float h, float m1, float m2)
	{
		if ((h) < (0))
			h += (float)(1);
		if ((h) > (1))
			h -= (float)(1);
		if ((h) < (1.0f / 6.0f))
			return (float)(m1 + (m2 - m1) * h * 6.0f);
		else if ((h) < (3.0f / 6.0f))
			return (float)(m2);
		else if ((h) < (4.0f / 6.0f))
			return (float)(m1 + (m2 - m1) * (2.0f / 3.0f - h) * 6.0f);
		return (float)(m1);
	}

	/// <summary>
	/// Converts HSL (hue, saturation, lightness) plus alpha to an RGBA <see cref="Color"/>.
	/// All inputs are in the 0-1 range except alpha which is 0-255.
	/// </summary>
	public static Color HSLA(float h, float s, float l, byte a)
	{
		h = (float)(Mod((float)(h), (float)(1.0f)));
		if ((h) < (0.0f))
			h += (float)(1.0f);
		s = (float)(Clamp((float)(s), (float)(0.0f), (float)(1.0f)));
		l = (float)(Clamp((float)(l), (float)(0.0f), (float)(1.0f)));
		var m2 = (float)(l <= 0.5f ? (l * (1 + s)) : (l + s - l * s));
		var m1 = (float)(2 * l - m2);

		float fr = (float)(Clamp((float)(Hue((float)(h + 1.0f / 3.0f), (float)(m1), (float)(m2))), (float)(0.0f), (float)(1.0f)));
		float fg = (float)(Clamp((float)(Hue((float)(h), (float)(m1), (float)(m2))), (float)(0.0f), (float)(1.0f)));
		float fb = (float)(Clamp((float)(Hue((float)(h - 1.0f / 3.0f), (float)(m1), (float)(m2))), (float)(0.0f), (float)(1.0f)));
		float fa = (float)(a / 255.0f);

		return FromRGBA((byte)(int)(fr * 255), (byte)(int)(fg * 255), (byte)(int)(fb * 255), (byte)(int)(fa * 255));
	}

	public static float DegToRad(float deg)
	{
		return (float)(deg / 180.0f * 3.14159274);
	}

	public static float RadToDeg(float rad)
	{
		return (float)(rad / 3.14159274 * 180.0f);
	}

	/// <summary>
	/// Creates a platform-specific <see cref="Color"/> from RGBA byte components,
	/// handling differences between MonoGame/FNA/Stride and other frameworks.
	/// </summary>
	public static Color FromRGBA(byte r, byte g, byte b, byte a)
	{
#if MONOGAME || FNA || STRIDE
		return new Color(r, g, b, a);
#else
		return Color.FromArgb(a, r, g, b);
#endif
	}
}
