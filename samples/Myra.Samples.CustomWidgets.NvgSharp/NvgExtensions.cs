using System.Text;
using Microsoft.Xna.Framework;
using FontStashSharp;

namespace NvgSharp;

public enum TextHorizontalAlignment
{
	/// <summary>
	/// Default, align text horizontally to left
	/// </summary>
	Left,

	/// <summary>
	/// Align text horizontally to center
	/// </summary>
	Center,

	/// <summary>
	/// Align text horizontally to right
	/// </summary>
	Right
}

public enum TextVerticalAlignment
{
	/// <summary>
	/// Default, Align text vertically to top
	/// </summary>
	Top,

	/// <summary>
	/// Align text vertically to middle
	/// </summary>
	Center,

	/// <summary>
	/// Align text vertically to bottom
	/// </summary>
	Bottom
}

internal static class NvgExtensions
{
	public static void TextAligned(this NvgContext context, SpriteFontBase font, string text, float x, float y, Vector2 scale,
		TextHorizontalAlignment horizontalAlignment = TextHorizontalAlignment.Left, TextVerticalAlignment verticalAlignment = TextVerticalAlignment.Top,
		float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f, FontSystemEffect effect = FontSystemEffect.Stroked,
			int effectAmount = 0)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}

		if (horizontalAlignment != TextHorizontalAlignment.Left)
		{
			var sz = font.MeasureString(text);
			if (horizontalAlignment == TextHorizontalAlignment.Center)
			{
				x -= sz.X / 2.0f;
			}
			else if (horizontalAlignment == TextHorizontalAlignment.Right)
			{
				x -= sz.X;
			}
		}

		if (verticalAlignment == TextVerticalAlignment.Center)
		{
			y -= font.LineHeight / 2.0f;
		}
		else if (verticalAlignment == TextVerticalAlignment.Bottom)
		{
			y -= font.LineHeight;
		}

		context.Text(font, text, x, y, scale, layerDepth, characterSpacing, lineSpacing, effect, effectAmount);
	}

	public static void TextAligned(this NvgContext context, SpriteFontBase font, StringBuilder text, float x, float y, Vector2 scale,
			TextHorizontalAlignment horizontalAlignment = TextHorizontalAlignment.Left, TextVerticalAlignment verticalAlignment = TextVerticalAlignment.Top,
		float layerDepth = 0.0f, float characterSpacing = 0.0f, float lineSpacing = 0.0f, FontSystemEffect effect = FontSystemEffect.Stroked,
			int effectAmount = 0)
	{
		if (text == null || text.Length == 0)
		{
			return;
		}

		if (horizontalAlignment != TextHorizontalAlignment.Left)
		{
			var sz = font.MeasureString(text);
			if (horizontalAlignment == TextHorizontalAlignment.Center)
			{
				x -= sz.X / 2.0f;
			}
			else if (horizontalAlignment == TextHorizontalAlignment.Right)
			{
				x -= sz.X;
			}
		}

		if (verticalAlignment == TextVerticalAlignment.Center)
		{
			y -= font.LineHeight / 2.0f;
		}
		else if (verticalAlignment == TextVerticalAlignment.Bottom)
		{
			y -= font.LineHeight;
		}

		context.Text(font, text, x, y, scale, layerDepth, characterSpacing, lineSpacing, effect, effectAmount);
	}
}