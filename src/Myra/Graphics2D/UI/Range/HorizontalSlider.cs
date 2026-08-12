using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Collections;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A slider control that allows users to select a value from a horizontal range.
	/// </summary>
	public class HorizontalSlider : Slider
	{
		/// <summary>
		/// Gets the orientation of the slider, which is always horizontal.
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Horizontal;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the slider.
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
		/// Initializes a new instance of the <see cref="HorizontalSlider"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalSlider(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName) : base(stylesheet, styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalSlider"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalSlider(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.HorizontalSliderStyles;
	}
}