using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class VerticalProgressBar : ProgressBar
	{
		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		public VerticalProgressBar(ProgressBarStyle style)
			: base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public VerticalProgressBar(string style)
			: this(Stylesheet.Current.VerticalProgressBarStyles[style])
		{
		}

		public VerticalProgressBar()
			: this(Stylesheet.Current.VerticalProgressBarStyle)
		{
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyProgressBarStyle(stylesheet.VerticalProgressBarStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.VerticalProgressBarStyles.Keys.ToArray();
		}
	}
}