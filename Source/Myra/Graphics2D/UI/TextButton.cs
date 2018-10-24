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
		private readonly TextBlock _textBlock;

		[EditCategory("Appearance")]
		public string Text
		{
			get { return _textBlock.Text; }
			set { _textBlock.Text = value; }
		}

		[EditCategory("Appearance")]
		[StylePropertyPath("LabelStyle.TextColor")]
		public Color TextColor
		{
			get { return _textBlock.TextColor; }
			set { _textBlock.TextColor = value; }
		}

		[JsonIgnore]
		[HiddenInEditor]
		[EditCategory("Appearance")]
		public SpriteFont Font
		{
			get { return _textBlock.Font; }
			set { _textBlock.Font = value; }
		}

		public TextButton(TextButtonStyle style)
		{
			_textBlock = new TextBlock
			{
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center
			};

			Widget = _textBlock;

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
				_textBlock.ApplyTextBlockStyle(style.LabelStyle);
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

		protected override void FireDown()
		{
			base.FireDown();
			Widget.IsPressed = IsPressed;
		}

		protected override void FireUp()
		{
			base.FireUp();
			Widget.IsPressed = IsPressed;
		}
	}
}