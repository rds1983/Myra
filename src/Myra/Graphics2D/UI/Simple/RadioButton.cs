using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class RadioButton : CheckButton
	{
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
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (Parent == null || !IsPressed)
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
			ApplyCheckButtonStyle(stylesheet.RadioButtonStyles.SafelyGetStyle(name));
		}
	}
}