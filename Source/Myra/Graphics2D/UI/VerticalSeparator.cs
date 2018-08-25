using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class VerticalSeparator : SeparatorWidget
	{
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