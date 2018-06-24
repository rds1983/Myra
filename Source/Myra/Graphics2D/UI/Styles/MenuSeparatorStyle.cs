using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class MenuSeparatorStyle: WidgetStyle
	{
		public TextureRegion Image { get; set; }
		public int Thickness { get; set; }

		public MenuSeparatorStyle()
		{
		}

		public MenuSeparatorStyle(MenuSeparatorStyle style): base(style)
		{
			Image = style.Image;
			Thickness = style.Thickness;
		}

		public override WidgetStyle Clone()
		{
			return new MenuSeparatorStyle(this);
		}
	}
}
