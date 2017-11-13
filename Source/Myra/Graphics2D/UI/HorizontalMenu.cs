using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class HorizontalMenu : Menu
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

		public HorizontalMenu(MenuStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalMenu(string style) : this(Stylesheet.Current.HorizontalMenuVariants[style])
		{
		}

		public HorizontalMenu() : base(Stylesheet.Current.HorizontalMenuStyle)
		{
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Left:
					MoveSelection(-1);
					break;
				case Keys.Right:
					MoveSelection(1);
					break;
			}
		}
	}
}
