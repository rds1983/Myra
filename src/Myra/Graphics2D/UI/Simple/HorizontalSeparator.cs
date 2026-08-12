using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Collections;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A horizontal separator widget that visually divides UI sections.
	/// </summary>
	public class HorizontalSeparator : SeparatorWidget
	{
		/// <summary>
		/// Gets or sets the horizontal alignment of the separator.
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
		/// Gets or sets the vertical alignment of the separator.
		/// </summary>
		[DefaultValue(VerticalAlignment.Center)]
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
		/// Gets the orientation of the separator, which is always horizontal.
		/// </summary>
		public override Orientation Orientation => Orientation.Horizontal;

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalSeparator"/> class with the specified stylesheet and style.
		/// </summary>
		/// <param name="stylesheet">The stylesheet to use for applying the style.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalSeparator(Stylesheet stylesheet, string styleName = Stylesheet.DefaultStyleName) : base(stylesheet, styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Center;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HorizontalSeparator"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public HorizontalSeparator(string styleName = Stylesheet.DefaultStyleName) : this(Stylesheet.Current, styleName)
		{
		}

		internal override IDictionary GetStylesDictionary(Stylesheet stylesheet) => stylesheet.HorizontalSeparatorStyles;
	}
}