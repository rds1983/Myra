using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#else
using Xenko.Core.Mathematics;
using Xenko.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	public class TextButton : ButtonBase<TextBlock>
	{
		[EditCategory("Appearance")]
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

		[EditCategory("Appearance")]
		[StylePropertyPath("/TextBlockStyle/TextColor")]
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
		[HiddenInEditor]
		[EditCategory("Appearance")]
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
			InternalChild = new TextBlock
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
			this(stylesheet.ButtonStyles[style].ToTextButtonStyle(stylesheet.TextBlockStyle))
		{
		}

		public TextButton(Stylesheet stylesheet) :
			this(stylesheet.ButtonStyle.ToTextButtonStyle(stylesheet.TextBlockStyle))
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
				InternalChild.ApplyTextBlockStyle(style.LabelStyle);
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyTextButtonStyle(new TextButtonStyle(stylesheet.ButtonStyles[name], stylesheet.TextBlockStyle));
		}
	}
}