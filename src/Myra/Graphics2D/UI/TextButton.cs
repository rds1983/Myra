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
			get { return InternalChild.Text; }
			set { InternalChild.Text = value; }
		}

		[EditCategory("Appearance")]
		[StylePropertyPath("/TextBlockStyle/TextColor")]
		public Color TextColor
		{
			get { return InternalChild.TextColor; }
			set { InternalChild.TextColor = value; }
		}

		[XmlIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public SpriteFont Font
		{
			get { return InternalChild.Font; }
			set { InternalChild.Font = value; }
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
				ApplyButtonStyle(style);
			}
		}

		public TextButton(string style) :
			this(new TextButtonStyle(Stylesheet.Current.ButtonStyles[style], Stylesheet.Current.TextBlockStyle))
		{
		}

		public TextButton() :
			this(new TextButtonStyle(Stylesheet.Current.ButtonStyle, Stylesheet.Current.TextBlockStyle))
		{
		}

		public void ApplyButtonStyle(TextButtonStyle style)
		{
			ApplyButtonStyle((ButtonStyle)style);

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
	}
}