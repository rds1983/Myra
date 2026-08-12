using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using Myra.Events;
using System.Collections;
using Myra.Attributes;


#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A clickable button widget that can contain any widget as its content.
	/// </summary>
	public class Button : ButtonBase
	{
		private readonly SingleItemLayout<Widget> _layout;
		internal bool ReleaseOnTouchLeft;

		/// <summary>
		/// Gets or sets the desktop that contains this button.
		/// </summary>
		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				// If we're not releasing the button on touch left,
				// we have to do it on touch up
				if (!ReleaseOnTouchLeft && Desktop != null)
				{
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (!ReleaseOnTouchLeft && Desktop != null)
				{
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
		}

		/// <summary>
		/// Gets or sets the content widget displayed inside the button.
		/// </summary>
		[Browsable(false)]
		[Content]
		public override Widget Content
		{
			get => _layout.Child;
			set => _layout.Child = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Button"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public Button(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName)
		{
			_layout = new SingleItemLayout<Widget>(this);
			ChildrenLayout = _layout;
			ReleaseOnTouchLeft = true;

			SetStyle(stylesheet, styleName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Button"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public Button(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.ButtonStyles;

		/// <summary>
		/// Handles the event when the cursor/touch leaves the button.
		/// </summary>
		public override void OnTouchLeft()
		{
			base.OnTouchLeft();

			if (ReleaseOnTouchLeft)
			{
				SetIsPressedByUser(false);
			}
		}

		/// <summary>
		/// Called when a touch point is released on the button.
		/// </summary>
		protected override void InternalOnTouchUp()
		{
			SetIsPressedByUser(false);
		}

		/// <summary>
		/// Called when a touch point is pressed on the button.
		/// </summary>
		protected override void InternalOnTouchDown()
		{
			SetIsPressedByUser(true);
		}

		/// <summary>
		/// Handles keyboard input for the button, simulating a click when Space is pressed.
		/// </summary>
		/// <param name="k">The key being pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (!Enabled)
			{
				return;
			}

			if (k == Keys.Space)
			{
				// Emulate click
				DoClick();
			}
		}

		private void DesktopTouchUp(object sender, MyraEventArgs args)
		{
			IsPressed = false;
		}

		/// <summary>
		/// Creates a button with a text label as its content.
		/// </summary>
		/// <param name="text">The text to display on the button.</param>
		/// <returns>A new Button with a text label.</returns>
		public static Button CreateTextButton(string text)
		{
			return new Button
			{
				Content = new Label
				{
					Text = text
				}
			};
		}
	}
}