using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class TextBlock : Widget
	{
		private readonly FormattedText _formattedText = new FormattedText();
		private bool _wrap = true;

		[EditCategory("Appearance")]
		public int VerticalSpacing
		{
			get { return _formattedText.VerticalSpacing; }
			set
			{
				_formattedText.VerticalSpacing = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Appearance")]
		public string Text
		{
			get { return _formattedText.Text; }
			set
			{
				_formattedText.Text = value;
				InvalidateMeasure();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Appearance")]
		public SpriteFont Font
		{
			get { return _formattedText.Font; }
			set
			{
				_formattedText.Font = value;
				InvalidateMeasure();
			}
		}

		[EditCategory("Appearance")]
		[DefaultValue(true)]
		public bool Wrap
		{
			get { return _wrap; }

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

		[EditCategory("Appearance")]
		public Color TextColor { get; set; }

		[EditCategory("Appearance")]
		public Color DisabledTextColor { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public bool IsMenuText
		{
			get { return _formattedText.IsMenuText; }

			set { _formattedText.IsMenuText = value; }
		}

		internal char? UnderscoreChar
		{
			get { return _formattedText.UnderscoreChar; }
		}

		public TextBlock(TextBlockStyle style)
		{
			if (style != null)
			{
				ApplyTextBlockStyle(style);
			}
		}

		public TextBlock(string style)
			: this(Stylesheet.Current.TextBlockStyles[style])
		{
		}

		public TextBlock() : this(Stylesheet.Current.TextBlockStyle)
		{
		}

		public override void InternalRender(RenderContext context)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			var bounds = ActualBounds;

			var color = Enabled ? TextColor : DisabledTextColor;
			_formattedText.Draw(context.Batch, bounds, color, context.Opacity);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Font == null)
			{
				return Point.Zero;
			}

			var width = availableSize.X;
			if (WidthHint != null && WidthHint.Value < width)
			{
				width = WidthHint.Value;
			}

			var result = Point.Zero;
			if (Font != null)
			{
				var formattedText = _formattedText.Clone();
				formattedText.Width = _wrap ? width : default(int?);

				result = formattedText.Size;
			}

			if (result.Y < Font.LineSpacing)
			{
				result.Y = Font.LineSpacing;
			}

			return result;
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
}