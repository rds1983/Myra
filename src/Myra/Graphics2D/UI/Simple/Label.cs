using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System;
using FontStashSharp;
using Myra.Utility;
using FontStashSharp.RichText;
using System.Collections;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A text label widget that displays formatted text with support for rich text formatting.
	/// </summary>
	public class Label : Widget
	{
		private readonly Color?[] _colors = new Color?[WidgetVisualStateTotal];
		private readonly RichTextLayout _richText = new RichTextLayout
		{
			SupportsCommands = true
		};

		private bool _wrap = false;

		private readonly RichTextLayout _errorText = new RichTextLayout
		{
			SupportsCommands = false
		};

		/// <summary>
		/// Gets or sets the vertical spacing in pixels between lines of text.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(0)]
		public int VerticalSpacing
		{
			get
			{
				return _richText.VerticalSpacing;
			}
			set
			{
				_richText.VerticalSpacing = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the text to display, with optional rich text formatting commands.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return _richText.Text;
			}
			set
			{
				if (_richText.Text == value)
				{
					return;
				}

				_richText.Text = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the font used to render the label text.
		/// </summary>
		[Category("Appearance")]
		public SpriteFontBase Font
		{
			get
			{
				return _richText.Font;
			}
			set
			{
				_richText.Font = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the text wraps to multiple lines.
		/// </summary>
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
			get => _richText.AutoEllipsisMethod;
			set
			{
				if (value == _richText.AutoEllipsisMethod)
				{
					return;
				}

				_richText.AutoEllipsisMethod = value;
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
			get => _richText.AutoEllipsisString;
			set
			{
				if (value == _richText.AutoEllipsisString)
				{
					return;
				}

				_richText.AutoEllipsisString = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the text.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(TextHorizontalAlignment.Left)]
		public TextHorizontalAlignment TextAlign { get; set; }

		/// <summary>
		/// Gets or sets the text color in the label's normal state.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color TextColor
		{
			get => _colors[WidgetVisualStateNormal].Value;
			set => _colors[WidgetVisualStateNormal] = value;
		}

		/// <summary>
		/// Gets or sets the text color when the mouse is over the label.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? OverTextColor
		{
			get => _colors[WidgetVisualStateOver];
			set => _colors[WidgetVisualStateOver] = value;
		}

		/// <summary>
		/// Gets or sets the text color when the label is disabled.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? DisabledTextColor
		{
			get => _colors[WidgetVisualStateDisabled];
			set => _colors[WidgetVisualStateDisabled] = value;
		}

		/// <summary>
		/// Gets or sets the text color when the label has focus.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? FocusedTextColor
		{
			get => _colors[WidgetVisualStateFocused];
			set => _colors[WidgetVisualStateFocused] = value;
		}

		/// <summary>
		/// Gets or sets the text color when the label is pressed.
		/// </summary>
		[Category("Appearance/TextColor")]
		public Color? PressedTextColor
		{
			get => _colors[WidgetVisualStatePressed];
			set => _colors[WidgetVisualStatePressed] = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the text supports rich text formatting commands.
		/// </summary>
		[DefaultValue(true)]
		public bool SupportsCommands
		{
			get => _richText.SupportsCommands;

			set => _richText.SupportsCommands = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Label"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public Label(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			TextColor = Color.Black;
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Label"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public Label(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		/// <summary>
		/// Renders the label's text with appropriate color based on its state.
		/// </summary>
		/// <param name="context">The render context to draw with.</param>
		public override void InternalRender(RenderContext context)
		{
			if (_richText.Font == null)
			{
				return;
			}

			var nullableColor = GetCurrentVisual(_colors);
			if (nullableColor == null)
			{
				return;
			}

			var color = nullableColor.Value;
			var useChunkColor = color == TextColor;

			var textToDraw = _richText;

			textToDraw.IgnoreColorCommand = !useChunkColor;
			var bounds = ActualBounds;

			var x = bounds.X;
			if (TextAlign == TextHorizontalAlignment.Center)
			{
				x += bounds.Width / 2;
			}
			else if (TextAlign == TextHorizontalAlignment.Right)
			{
				x += bounds.Width;
			}

			try
			{
				context.DrawRichText(textToDraw, new Vector2(x, bounds.Y), color, horizontalAlignment: TextAlign);
			}
			catch (Exception ex)
			{
				x = bounds.X;
				_errorText.Font = Font;
				_errorText.Text = BuildRtlError(ex);
				context.DrawRichText(_errorText, new Vector2(x, bounds.Y), Color.Red);
			}
		}

		private static string BuildRtlError(Exception ex)
		{
			return "RTL Error: " + ex.Message;
		}

		/// <summary>
		/// Measures the size required to display the label text.
		/// </summary>
		/// <param name="availableSize">The available size for the label.</param>
		/// <returns>The measured size needed for the label.</returns>
		protected override Point InternalMeasure(Point availableSize)
		{
			if (Font == null)
			{
				return Mathematics.PointZero;
			}

			var width = availableSize.X;
			var height = availableSize.Y;

			var result = Mathematics.PointZero;
			try
			{
				result = _richText.Measure(_wrap ? width : default(int?));
			}
			catch (Exception ex)
			{
				_errorText.Font = Font;
				_errorText.Text = BuildRtlError(ex);
				result = _errorText.Measure(_wrap ? width : default(int?));
			}

			if (result.Y < Font.LineHeight)
			{
				result.Y = Font.LineHeight;
			}

			return result;
		}

		/// <summary>
		/// Arranges the label's text layout within the label's bounds.
		/// </summary>
		protected override void InternalArrange()
		{
			base.InternalArrange();

			if (_wrap)
			{
				_richText.Width = ActualBounds.Width;
				_richText.Height = ActualBounds.Height;
			}
			else
			{
				if (AutoEllipsisMethod != AutoEllipsisMethod.None)
				{
					_richText.Width = ActualBounds.Width;
					_richText.Height = Font.LineHeight;
				}
				else
				{
					_richText.Width = default(int?);
					_richText.Height = default(int?);
				}
			}
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.LabelStyles;

		/// <summary>
		/// Applies the specified widget style to this label.
		/// </summary>
		/// <param name="style">The widget style to apply.</param>
		protected override void ApplyStyle(WidgetStyle style)
		{
			base.ApplyStyle(style);

			var labelStyle = (LabelStyle)style;
			TextColor = labelStyle.TextColor;
			DisabledTextColor = labelStyle.DisabledTextColor;
			FocusedTextColor = labelStyle.FocusedTextColor;
			OverTextColor = labelStyle.OverTextColor;
			PressedTextColor = labelStyle.PressedTextColor;
			Font = labelStyle.Font;
		}

		/// <summary>
		/// Applies the specified label style to this label.
		/// </summary>
		/// <param name="style">The label style to apply.</param>
		public void ApplyLabelStyle(LabelStyle style) => ApplyStyle(style);

		/// <summary>
		/// Copies all properties from another widget to this label.
		/// </summary>
		/// <param name="w">The widget to copy properties from.</param>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var label = (Label)w;

			VerticalSpacing = label.VerticalSpacing;
			Text = label.Text;
			Font = label.Font;
			Wrap = label.Wrap;
			AutoEllipsisMethod = label.AutoEllipsisMethod;
			AutoEllipsisString = label.AutoEllipsisString;
			TextAlign = label.TextAlign;

			for (var i = 0; i < WidgetVisualStateTotal; ++i)
			{
				_colors[i] = label._colors[i];
			}
		}
	}
}