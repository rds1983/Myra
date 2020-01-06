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
		[DefaultValue(null)]
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

		public TextButton(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Label(null)
			{
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Wrap = true
			};

			SetStyle(styleName);
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

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTextButtonStyle(new TextButtonStyle(stylesheet.ButtonStyles[name], stylesheet.LabelStyle));
		}
	}
}