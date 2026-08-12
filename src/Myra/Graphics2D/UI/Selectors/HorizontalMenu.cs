using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Collections;


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
	/// A menu control that displays menu items horizontally in a row.
	/// </summary>
	public class HorizontalMenu : Menu
	{
		/// <summary>
		/// Gets the orientation of the menu, which is always horizontal.
		/// </summary>
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the menu.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the menu.
		/// </summary>
		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalMenu"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalMenu(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName) : base(stylesheet, styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalMenu"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalMenu(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.HorizontalMenuStyles;

		/// <summary>
		/// Handles keyboard input for horizontal menu navigation using left and right arrow keys.
		/// </summary>
		/// <param name="k">The key being pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Left:
					MoveHover(-1);
					break;
				case Keys.Right:
					MoveHover(1);
					break;
			}
		}
	}
}
