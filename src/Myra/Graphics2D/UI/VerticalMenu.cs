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
	public class VerticalMenu : Menu
	{
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
		}

		[DefaultValue(HorizontalAlignment.Left)]
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

		public VerticalMenu(MenuStyle style) : base(style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
		}

		public VerticalMenu(Stylesheet stylesheet, string style) : this(stylesheet.VerticalMenuStyles[style])
		{
		}

		public VerticalMenu(Stylesheet stylesheet) : this(stylesheet.VerticalMenuStyle)
		{
		}

		public VerticalMenu(string style) : this(Stylesheet.Current, style)
		{
		}

		public VerticalMenu() : this(Stylesheet.Current)
		{
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Up:
					MoveHover(-1);
					break;
				case Keys.Down:
					MoveHover(1);
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