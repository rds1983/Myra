using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class HorizontalBox : Box
	{
		public override Orientation Orientation => Orientation.Horizontal;

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

		public HorizontalBox()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
		}
	}
}
