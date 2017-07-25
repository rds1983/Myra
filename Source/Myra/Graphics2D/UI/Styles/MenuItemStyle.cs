using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class MenuItemStyle: ButtonStyle
	{
		public TextureRegion2D OpenBackground { get; set; }

		public int? IconWidth { get; set; }
		public int? ShortcutWidth { get; set; }

		public MenuItemStyle()
		{
		}

		public MenuItemStyle(MenuItemStyle style) : base(style)
		{
			OpenBackground = style.OpenBackground;

			IconWidth = style.IconWidth;
			ShortcutWidth = style.ShortcutWidth;
		}

		public override WidgetStyle Clone()
		{
			return new MenuItemStyle(this);
		}
	}
}
