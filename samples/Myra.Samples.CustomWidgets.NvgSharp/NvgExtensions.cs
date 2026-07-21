using System.Text;
using Microsoft.Xna.Framework;
using FontStashSharp;

namespace NvgSharp;

/// <summary>
/// Horizontal text alignment options.
/// </summary>
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

/// <summary>
/// Vertical text alignment options.
/// </summary>
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

/// <summary>
/// Extension methods on <see cref="NvgContext"/> for drawing aligned text.
/// </summary>
internal static class NvgExtensions
{
	/// <summary>
	/// Draws a string at the specified position with horizontal and vertical alignment.
	/// The position is adjusted based on the measured text size and the requested alignment
	/// before calling into the NvgSharp text drawing API.
	/// </summary>
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

	/// <summary>
	/// Draws a <see cref="StringBuilder"/> at the specified position with horizontal and vertical alignment.
	/// Overload of <see cref="TextAligned(NvgContext, SpriteFontBase, string, float, float, Vector2, TextHorizontalAlignment, TextVerticalAlignment, float, float, float, FontSystemEffect, int)"/>
	/// for <see cref="StringBuilder"/> to avoid string allocation.
	/// </summary>
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