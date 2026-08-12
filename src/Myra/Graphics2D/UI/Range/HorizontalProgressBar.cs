using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Collections;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A progress bar control that displays progress horizontally from left to right.
	/// </summary>
	public class HorizontalProgressBar : ProgressBar
	{
		/// <summary>
		/// Gets the orientation of the progress bar, which is always horizontal.
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Horizontal;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the progress bar.
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
		/// Gets or sets the vertical alignment of the progress bar.
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
		/// Initializes a new instance of the <see cref="HorizontalProgressBar"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalProgressBar(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName) : base(stylesheet, styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalProgressBar"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalProgressBar(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.HorizontalProgressBarStyles;
	}
}