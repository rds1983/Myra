using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class VerticalBox : Box
	{
		public override Orientation Orientation => Orientation.Vertical;

		[DefaultValue(VerticalAlignment.Stretch)]
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

		public VerticalBox()
		{
			VerticalAlignment = VerticalAlignment.Stretch;
		}
	}
}
