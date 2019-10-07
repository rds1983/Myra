using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class RadioButton : ImageTextButton
	{
		[Browsable(false)]
		[XmlIgnore]
		[Category("Behavior")]
		[DefaultValue(true)]
		public override bool Toggleable
		{
			get
			{
				return base.Toggleable;
			}
			set
			{
				base.Toggleable = value;
			}
		}

		public RadioButton(string styleName = Stylesheet.DefaultStyleName): base(styleName)
		{
			Toggleable = true;
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (IsPressed)
			{
				foreach (var child in Parent.ChildrenCopy)
				{
					var asRadio = child as RadioButton;

					if (asRadio == null || asRadio == this)
					{
						continue;
					}

					asRadio.IsPressed = false;
				}
			}
		}

		protected override bool CanChangeToggleable(bool value)
		{
			return !IsPressed;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(stylesheet.RadioButtonStyles[name]);
		}
	}
}