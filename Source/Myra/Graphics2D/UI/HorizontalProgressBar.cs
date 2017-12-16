using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalProgressBar : ProgressBar
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		public HorizontalProgressBar(ProgressBarStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalProgressBar(string style)
			: this(Stylesheet.Current.HorizontalProgressBarStyles[style])
		{
		}

		public HorizontalProgressBar()
			: this(Stylesheet.Current.HorizontalProgressBarStyle)
		{
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyProgressBarStyle(stylesheet.HorizontalProgressBarStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.HorizontalProgressBarStyles.Keys.ToArray();
		}
	}
}