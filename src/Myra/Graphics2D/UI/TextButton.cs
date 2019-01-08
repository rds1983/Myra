using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

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
		[StylePropertyPath("LabelStyle.TextColor")]
		public Color TextColor
		{
			get { return InternalChild.TextColor; }
			set { InternalChild.TextColor = value; }
		}

		[JsonIgnore]
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

		public TextButton(string style)
			: this(Stylesheet.Current.TextButtonStyles[style])
		{
		}

		public TextButton()
			: this(Stylesheet.Current.TextButtonStyle)
		{
		}

		public void ApplyButtonStyle(TextButtonStyle style)
		{
			ApplyButtonBaseStyle(style);

			if (style.LabelStyle != null)
			{
				InternalChild.ApplyTextBlockStyle(style.LabelStyle);
			}
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.TextButtonStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.TextButtonStyles.Keys.ToArray();
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}
	}
}