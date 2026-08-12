using Myra.Graphics2D.UI.Styles;
using System.Collections;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A radio button widget that can be part of a group where only one button can be selected at a time.
	/// </summary>
	public class RadioButton : CheckButtonBase
	{
		/// <summary>
		/// Gets or sets a value indicating whether this radio button is pressed/selected.
		/// Only one radio button in a group can be pressed at a time.
		/// </summary>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="RadioButton"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public RadioButton(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RadioButton"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public RadioButton(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.RadioButtonStyles;

		/// <summary>
		/// Handles the pressed state change, deselecting other radio buttons in the same parent when this button is pressed.
		/// </summary>
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
	}
}