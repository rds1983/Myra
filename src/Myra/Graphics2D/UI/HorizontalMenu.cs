using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;

#if !XENKO
using Microsoft.Xna.Framework.Input;
#else
using Xenko.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class HorizontalMenu : Menu
	{
		public override Orientation Orientation
		{
			get { return Orientation.Horizontal; }
		}

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

		[DefaultValue(VerticalAlignment.Top)]
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

		public HorizontalMenu(MenuStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public HorizontalMenu(Stylesheet stylesheet, string style) : this(stylesheet.HorizontalMenuStyles[style])
		{
		}

		public HorizontalMenu(Stylesheet stylesheet) : this(stylesheet.HorizontalMenuStyle)
		{
		}

		public HorizontalMenu(string style) : this(Stylesheet.Current, style)
		{
		}

		public HorizontalMenu() : this(Stylesheet.Current)
		{
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Left:
					MoveHover(-1);
					break;
				case Keys.Right:
					MoveHover(1);
					break;
			}
		}

		public override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyMenuStyle(stylesheet.HorizontalMenuStyles[name]);
		}
	}
}
