using System.ComponentModel;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using System;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	public class TextBlock : Widget
	{
		private readonly FormattedText _formattedText = new FormattedText();
		private bool _wrap = false;

        private FormattedText _autoEllipsisText;
        private bool _autoEllipsis = false;
        private string _autoEllipsisString = "...";
        private AutoEllipsisMethod _autoEllipsisMethod;

        [EditCategory("Appearance")]
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

		[EditCategory("Appearance")]
		public string Text
		{
			get
			{
				return _formattedText.Text;
			}
			set
			{
				_formattedText.Text = value;
				InvalidateMeasure();
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Appearance")]
		public SpriteFont Font
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

		[EditCategory("Appearance")]
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
        /// Replaces overflowing text with an ellipsis.
        /// </summary>
        [EditCategory("Appearance")]
        [DefaultValue(false)]    
        public bool AutoEllipsis
        {
            get => _autoEllipsis;
            set
            {
                if (value == _autoEllipsis) return;
                _autoEllipsis = value;
                InvalidateMeasure();
            }
        }

        /// <summary>
        /// The string to use as ellipsis.
        /// </summary>
        [EditCategory("Appearance")]
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

        [EditCategory("Appearance")]
        [DefaultValue(AutoEllipsisMethod.Character)]
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

        [EditCategory("Appearance")]
		public Color TextColor
		{
			get; set;
		}

		[EditCategory("Appearance")]
		public Color? DisabledTextColor
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Color? OverTextColor
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public Color? PressedTextColor
		{
			get; set;
		}

		[HiddenInEditor]
		[XmlIgnore]
		public bool IsPressed
		{
			get; set;
		}

		public TextBlock(TextBlockStyle style)
		{
			if (style != null)
			{
				ApplyTextBlockStyle(style);
			}
		}

		public TextBlock(Stylesheet stylesheet, string style) : this(stylesheet.TextBlockStyles[style])
		{
		}

		public TextBlock(Stylesheet stylesheet) : this(stylesheet.TextBlockStyle)
		{
		}

		public TextBlock(string style) : this(Stylesheet.Current, style)
		{
		}

		public TextBlock() : this(Stylesheet.Current)
		{
		}

		public override void InternalRender(RenderContext context)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			var bounds = ActualBounds;

			var color = TextColor;
			if (!Enabled && DisabledTextColor != null)
			{
				color = DisabledTextColor.Value;
			}
			else if (IsPressed && PressedTextColor != null)
			{
				color = PressedTextColor.Value;
			}
			else if (IsMouseOver && OverTextColor != null)
			{
				color = OverTextColor.Value;
			}

            var textToDraw = _autoEllipsis ? _autoEllipsisText : _formattedText;
            textToDraw.Draw(context.Batch, bounds.Location, context.View, color, context.Opacity);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
            if (Font == null)
			{
				return Point.Zero;
			}

			var width = availableSize.X;
            var height = availableSize.Y;
			if (Width != null && Width.Value < width)
			{
				width = Width.Value;
			}

			var result = Point.Zero;
			if (Font != null)
			{
				result = _formattedText.Measure(_wrap ? width : default(int?));
			}

            if(_autoEllipsis)
            {
                _autoEllipsisText = ApplyAutoEllipsis(width, height);
                result = _autoEllipsisText.Measure(_wrap ? width : default(int?));
            }

            if (result.Y < CrossEngineStuff.LineSpacing(Font))
			{
				result.Y = CrossEngineStuff.LineSpacing(Font);
			}

			return result;
		}

        private FormattedText ApplyAutoEllipsis(int width, int height)
        {
            var unchangedMeasure = _formattedText.Measure(_wrap ? width : default(int?));
            if (unchangedMeasure.X <= width && unchangedMeasure.Y <= height)
            {
                return _formattedText; // don't even need to do anything
            }

            var origText = _formattedText.Text;
            var measureText = new FormattedText()
            {
                Text = _formattedText.Text,
                Font = _formattedText.Font,
                IsPassword = _formattedText.IsPassword,
                VerticalSpacing = _formattedText.VerticalSpacing,
                Width = _formattedText.Width
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
                IsPassword = _formattedText.IsPassword,
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

			_formattedText.Width = _wrap ? Bounds.Width : default(int?);
		}

		public void ApplyTextBlockStyle(TextBlockStyle style)
		{
			ApplyWidgetStyle(style);

			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			OverTextColor = style.OverTextColor;
			PressedTextColor = style.PressedTextColor;
			Font = style.Font;
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyTextBlockStyle(stylesheet.TextBlockStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.TextBlockStyles.Keys.ToArray();
		}
	}

    public enum AutoEllipsisMethod
    {
        Character,
        Word
    }
}