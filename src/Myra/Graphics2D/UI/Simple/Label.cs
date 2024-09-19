using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System;
using FontStashSharp;
using Myra.Utility;
using FontStashSharp.RichText;

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
	public class Label : Widget
	{
		private readonly RichTextLayout _richText = new RichTextLayout
		{
			SupportsCommands = true
		};

		private bool _wrap = false;

		private readonly RichTextLayout _errorText = new RichTextLayout
		{
			SupportsCommands = false
		};

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
			set => _richText.AutoEllipsisMethod = value;
		}

		/// <summary>
		/// The string to use as ellipsis.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("...")]
		public string AutoEllipsisString
		{
			get => _richText.AutoEllipsisString;
			set => _richText.AutoEllipsisString = value;
		}

		[Category("Appearance")]
		[DefaultValue(TextHorizontalAlignment.Left)]
		public TextHorizontalAlignment TextAlign
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
			if (_richText.Font == null)
			{
				return;
			}

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
			else if (IsMouseInside && OverTextColor != null)
			{
				color = OverTextColor.Value;
				useChunkColor = false;
			}

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

		protected override void InternalArrange()
		{
			base.InternalArrange();

			_richText.Width = _wrap ? ActualBounds.Width : default(int?);
			_richText.Height = _wrap ? ActualBounds.Height : default(int?);
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
			ApplyLabelStyle(stylesheet.LabelStyles.SafelyGetStyle(name));
		}

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
			TextColor = label.TextColor;
			DisabledTextColor = label.DisabledTextColor;
			OverTextColor= label.OverTextColor;
			PressedTextColor= label.PressedTextColor;
		}
	}
}