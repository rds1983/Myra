using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
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

		public HorizontalMenu(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
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

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyMenuStyle(stylesheet.HorizontalMenuStyles.SafelyGetStyle(name));
		}
	}
}
