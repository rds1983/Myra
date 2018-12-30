using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSeparator : SeparatorWidget
	{
		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[DefaultValue(VerticalAlignment.Center)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		public HorizontalSeparator(SeparatorStyle style) : base(Orientation.Horizontal, style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Center;
		}

		public HorizontalSeparator(string style)
			: this(Stylesheet.Current.HorizontalSeparatorStyles[style])
		{
		}

		public HorizontalSeparator()
			: this(Stylesheet.Current.HorizontalSeparatorStyle)
		{
		}
	}
}