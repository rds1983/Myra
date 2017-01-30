using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalMenu: Menu
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		public HorizontalMenu(MenuStyle style): base(style)
		{
		}

		public HorizontalMenu() : base(DefaultAssets.UIStylesheet.HorizontalMenuStyle)
		{
		}
	}
}
