using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class HorizontalSeparator : SeparatorWidget
	{
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

		public override Orientation Orientation => Orientation.Horizontal;

		public HorizontalSeparator(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Center;
		}


		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySeparatorStyle(stylesheet.HorizontalSeparatorStyles.SafelyGetStyle(name));
		}
	}
}