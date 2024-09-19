using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Attributes;
using FontStashSharp;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	[Obsolete("Switch to Button")]
	[StyleTypeName("Button")]
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
		[StylePropertyPath("/LabelStyle/OverTextColor")]
		public Color? OverTextColor
		{
			get
			{
				return InternalChild.OverTextColor;
			}
			set
			{
				InternalChild.OverTextColor = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/PressedTextColor")]
		public Color? PressedTextColor
		{
			get
			{
				return InternalChild.PressedTextColor;
			}
			set
			{
				InternalChild.PressedTextColor = value;
			}
		}

		[Category("Appearance")]
		public SpriteFontBase Font
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

		public void ApplyTextButtonStyle(ButtonStyle style)
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
			ApplyTextButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}