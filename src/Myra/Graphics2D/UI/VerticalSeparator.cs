using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class VerticalSeparator : SeparatorWidget
	{
		[DefaultValue(HorizontalAlignment.Center)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		public VerticalSeparator(SeparatorStyle style) : base(Orientation.Vertical, style)
		{
			HorizontalAlignment = HorizontalAlignment.Center;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public VerticalSeparator(string style)
			: this(Stylesheet.Current.VerticalSeparatorStyles[style])
		{
		}

		public VerticalSeparator()
			: this(Stylesheet.Current.VerticalSeparatorStyle)
		{
		}
	}
}