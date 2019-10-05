using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Attributes;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	public class TextButton : ButtonBase<Label>
	{
		[Category("Appearance")]
		public string Text
		{
			get
			{
				return InternalChild.Text;
			}
			set
			{
				InternalChild.Text = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/TextColor")]
		public Color TextColor
		{
			get
			{
				return InternalChild.TextColor;
			}
			set
			{
				InternalChild.TextColor = value;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		[Category("Appearance")]
		public SpriteFont Font
		{
			get
			{
				return InternalChild.Font;
			}
			set
			{
				InternalChild.Font = value;
			}
		}

		public TextButton(TextButtonStyle style)
		{
			InternalChild = new Label(style != null ? style.LabelStyle : null)
			{
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Wrap = true
			};

			if (style != null)
			{
				ApplyTextButtonStyle(style);
			}
		}

		public TextButton(Stylesheet stylesheet, string style) :
			this(stylesheet.ButtonStyles[style].ToTextButtonStyle(stylesheet.LabelStyle))
		{
		}

		public TextButton(Stylesheet stylesheet) :
			this(stylesheet.ButtonStyle.ToTextButtonStyle(stylesheet.LabelStyle))
		{
		}

		public TextButton(string style) : this(Stylesheet.Current, style)
		{
		}

		public TextButton() : this(Stylesheet.Current)
		{
		}

		public void ApplyTextButtonStyle(TextButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.LabelStyle != null)
			{
				InternalChild.ApplyLabelStyle(style.LabelStyle);
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyTextButtonStyle(new TextButtonStyle(stylesheet.ButtonStyles[name], stylesheet.LabelStyle));
		}
	}
}