using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class VerticalMenu : Menu
	{
		public override Orientation Orientation
		{
			get { return Orientation.Vertical; }
		}

		public VerticalMenu(MenuStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public VerticalMenu(string style)
			: this(Stylesheet.Current.VerticalMenuVariants[style])
		{
		}

		public VerticalMenu()
			: base(Stylesheet.Current.VerticalMenuStyle)
		{
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Up:
					MoveSelection(-1);
					break;
				case Keys.Down:
					MoveSelection(1);
					break;
			}
		}
	}
}
