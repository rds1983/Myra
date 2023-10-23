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

		public override bool IsPressed
		{
			get => base.IsPressed;

			set
			{
				if (IsPressed && Parent != null)
				{
					// If this is last pressed button
					// Don't allow it to be unpressed
					var allow = false;
					foreach (var child in Parent.ChildrenCopy)
					{
						var asRadio = child as RadioButton;
						if (asRadio == null || asRadio == this)
						{
							continue;
						}

						if (asRadio.IsPressed)
						{
							allow = true;
							break;
						}
					}

					if (!allow)
					{
						return;
					}
				}

				base.IsPressed = value;
			}
		}

		public RadioButton(string styleName = Stylesheet.DefaultStyleName): base(styleName)
		{
			Toggleable = true;
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (!IsPressed)
			{
				return;
			}

			// Release other pressed radio buttons
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

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(stylesheet.RadioButtonStyles.SafelyGetStyle(name));
		}
	}
}