using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class VerticalSeparator : SeparatorWidget
	{
		[DefaultValue(HorizontalAlignment.Center)]
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

		public override Orientation Orientation => Orientation.Vertical;

		public VerticalSeparator(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Center;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySeparatorStyle(stylesheet.VerticalSeparatorStyles.SafelyGetStyle(name));
		}
	}
}