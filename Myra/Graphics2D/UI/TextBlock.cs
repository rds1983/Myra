using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class TextBlock : Widget
	{
		private readonly FormattedText _formattedText = new FormattedText();

		public int VerticalSpacing
		{
			get { return _formattedText.VerticalSpacing; }
			set
			{
				_formattedText.VerticalSpacing = value;
				FireMeasureChanged();
			}
		}

		public string Text
		{
			get { return _formattedText.Text; }
			set
			{
				_formattedText.Text = value;
				FireMeasureChanged();
			}
		}

		public BitmapFont Font
		{
			get { return _formattedText.Font; }
			set
			{
				_formattedText.Font = value;
				FireMeasureChanged();
			}
		}

		public bool Wrap
		{
			get { return _formattedText.Wrap; }

			set
			{
				_formattedText.Wrap = value;
				FireMeasureChanged();
			}
		}

		public Color TextColor { get; set; }

		public TextBlock(TextBlockStyle style)
		{
			if (style != null)
			{
				ApplyTextBlockStyle(style);
			}
		}

		public TextBlock() : this(Stylesheet.Current.TextBlockStyle)
		{
		}

		public override void InternalRender(SpriteBatch batch)
		{
			if (_formattedText.Font == null)
			{
				return;
			}

			_formattedText.Draw(batch, ClientBounds, TextColor);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var width = availableSize.X;
			if (WidthHint != null && WidthHint.Value < width)
			{
				width = WidthHint.Value;
			}

			var result = Point.Zero;
			if (Font != null)
			{
				_formattedText.Size = new Point(width, _formattedText.Size.Y);

				var strings = _formattedText.Strings;
				foreach (var si in strings)
				{
					if (si.Bounds.Width > result.X)
					{
						result.X = si.Bounds.Width;
					}

					result.Y += si.Bounds.Height;
				}

				if (strings.Length > 1)
				{
					result.Y += (strings.Length - 1)*_formattedText.VerticalSpacing;
				}
			}

			return result;
		}

		public override void Arrange()
		{
			base.Arrange();

			_formattedText.Size = new Point(ClientBounds.Width, _formattedText.Size.Y);
		}

		public void ApplyTextBlockStyle(TextBlockStyle style)
		{
			ApplyWidgetStyle(style);

			TextColor = style.TextColor;
			Font = style.Font;
		}
	}
}