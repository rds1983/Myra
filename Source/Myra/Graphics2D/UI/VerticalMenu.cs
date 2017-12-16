using System.Linq;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI.Styles;

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
			VerticalAlignment = VerticalAlignment.Top;
		}

		public VerticalMenu(string style)
			: this(Stylesheet.Current.VerticalMenuStyles[style])
		{
		}

		public VerticalMenu()
			: this(Stylesheet.Current.VerticalMenuStyle)
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

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyMenuStyle(stylesheet.VerticalMenuStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.VerticalMenuStyles.Keys.ToArray();
		}
	}
}