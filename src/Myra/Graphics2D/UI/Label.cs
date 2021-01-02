using System.ComponentModel;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using System;
using FontStashSharp;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public class Label : Widget
	{
		private readonly FormattedText _formattedText = new FormattedText
		{
			CalculateGlyphs = false,
			SupportsCommands = true
		};

		private bool _wrap = false;

		private AutoEllipsisMethod _autoEllipsisMethod = AutoEllipsisMethod.None;
		private FormattedText _autoEllipsisText;
		private string _autoEllipsisString = "...";

		[Category("Appearance")]
		[DefaultValue(0)]
		public int VerticalSpacing
		{
			get
			{
				return _formattedText.VerticalSpacing;
			}
			set
			{
				_formattedText.VerticalSpacing = value;
				InvalidateMeasure();
			}
		}

		[Category("Appearance")]
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return _formattedText.Text;
			}
			set
			{
				if (_formattedText.Text == value)
				{
					return;
				}

				_formattedText.Text = value;
				InvalidateMeasure();
			}
		}

		[Category("Appearance")]
		public SpriteFontBase Font
		{
			get
			{
				return _formattedText.Font;
			}
			set
			{
				_formattedText.Font = value;
				InvalidateMeasure();
			}
		}

		[Category("Appearance")]
		[DefaultValue(false)]
		public bool Wrap
		{
			get
			{
				return _wrap;
			}

			set
			{
				if (value == _wrap)
				{
					return;
				}

				_wrap = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// The method used to abbreviate overflowing text.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(AutoEllipsisMethod.None)]
		public AutoEllipsisMethod AutoEllipsisMethod
		{
			get => _autoEllipsisMethod;
			set
			{
				if (value == _autoEllipsisMethod) return;
				_autoEllipsisMethod = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// The string to use as ellipsis.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("...")]
		public string AutoEllipsisString
		{
			get => _autoEllipsisString;
			set
			{
				if (value == _autoEllipsisString) return;
				_autoEllipsisString = value;
				InvalidateMeasure();
			}
		}

		[Category("Appearance")]
		[DefaultValue(TextAlign.Left)]
		public TextAlign TextAlign
		{
			get; set;
		}

		[Category("Appearance")]
		public Color TextColor
		{
			get; set;
		}

		[Category("Appearance")]
		public Color? DisabledTextColor
		{
			get; set;
		}

		[Category("Appearance")]
		public Color? OverTextColor
		{
			get; set;
		}

		internal Color? PressedTextColor
		{
			get; set;
		}

		internal bool IsPressed
		{
			get; set;
		}

		public Label(string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(styleName);
		}

		public override void InternalRender(RenderContext context)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			var bounds = ActualBounds;

			var color = TextColor;
			var useChunkColor = true;
			if (!Enabled && DisabledTextColor != null)
			{
				color = DisabledTextColor.Value;
				useChunkColor = false;
			}
			else if (IsPressed && PressedTextColor != null)
			{
				color = PressedTextColor.Value;
				useChunkColor = false;
			}
			else if (UseHoverRenderable && OverTextColor != null)
			{
				color = OverTextColor.Value;
				useChunkColor = false;
			}

			var textToDraw = (_autoEllipsisMethod == AutoEllipsisMethod.None) 
				? _formattedText : _autoEllipsisText;
			textToDraw.Draw(context, TextAlign, bounds, context.View, color, useChunkColor);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Font == null)
			{
				return Mathematics.PointZero;
			}

			var width = availableSize.X;
			var height = availableSize.Y;
			var ellipsisEnabled = _autoEllipsisMethod != AutoEllipsisMethod.None;

			var result = Mathematics.PointZero;
			if (ellipsisEnabled)
			{
				_autoEllipsisText = ApplyAutoEllipsis(width, height);
				result = _autoEllipsisText.Measure(_wrap ? width : default(int?));
			}
			else if (Font != null)
			{
				result = _formattedText.Measure(_wrap ? width : default(int?));
			}

			if (result.Y < Font.FontSize)
			{
				result.Y = Font.FontSize;
			}

			return result;
		}

		private FormattedText ApplyAutoEllipsis(int width, int height)
		{
			var unchangedMeasure = _formattedText.Measure(_wrap ? width : default(int?));
			if (unchangedMeasure.X <= width && unchangedMeasure.Y <= height)
			{
				return _formattedText; // don't even need to do anything.
			}

			var origText = _formattedText.Text;
			var measureText = new FormattedText()
			{
				Text = _formattedText.Text,
				Font = _formattedText.Font,
				VerticalSpacing = _formattedText.VerticalSpacing,
				Width = _formattedText.Width,
				CalculateGlyphs = _formattedText.CalculateGlyphs,
				SupportsCommands = _formattedText.SupportsCommands
			};
			string result;

			// find longest possible string using binary search
			int left = 0;
			int right = origText.Length;
			int center = 0;

			while (left <= right)
			{
				center = left + ((right - left) / 2);
				measureText.Text = $"{origText.Substring(0, center)}{AutoEllipsisString}";

				var measure = GetMeasure();
				if (measure.X == width && measure.Y <= height)
				{
					break;
				}
				else if (measure.X > width || measure.Y > height)
				{
					right = center - 1;
				}
				else
				{
					left = center + 1;
				}
			}

			result = origText.Substring(0, center);

			if (AutoEllipsisMethod == AutoEllipsisMethod.Word)
			{
				// cut on spaces rather than in the middle of a word.
				// preserve a space character before the ellipsis if there is
				// enough room for it.
				try
				{
					var closestSpace = origText.LastIndexOf(' ', center);
					if (closestSpace > 0)
					{
						int subStrLength = closestSpace;
						measureText.Text = origText.Substring(0, closestSpace + 1) + AutoEllipsisString;
						if (GetMeasure().X < width)
						{
							subStrLength++;
						}
						result = origText.Substring(0, subStrLength);
					}
				}
				catch (ArgumentOutOfRangeException)
				{
					// do nothing
				}
			}

			return new FormattedText()
			{
				Text = result + AutoEllipsisString,
				Font = _formattedText.Font,
				VerticalSpacing =_formattedText.VerticalSpacing,
				Width = _formattedText.Width
			};

			Point GetMeasure()
			{
				return measureText.Measure(_wrap ? width : default(int?));
			}
		}

		public override void Arrange()
		{
			base.Arrange();

			_formattedText.Width = _wrap ? ActualBounds.Width : default(int?);
		}

		public void ApplyLabelStyle(LabelStyle style)
		{
			ApplyWidgetStyle(style);

			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			OverTextColor = style.OverTextColor;
			PressedTextColor = style.PressedTextColor;
			Font = style.Font;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyLabelStyle(stylesheet.LabelStyles[name]);
		}
	}

	public enum AutoEllipsisMethod
	{
		/// <summary>
		/// Autoellipsis is disabled.
		/// </summary>
		None,

		/// <summary>
		/// The text can be cut at any character.
		/// </summary>
		Character,

		/// <summary>
		/// The text will be cut at spaces.
		/// </summary>
		Word
	}
}