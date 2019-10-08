namespace Myra.Graphics2D.UI.Styles
{
	public class MenuItemStyle: ButtonStyle
	{
		public PressableImageStyle ImageStyle
		{
			get; set;
		}

		public LabelStyle LabelStyle
		{
			get; set;
		}

		public LabelStyle ShortcutStyle
		{
			get; set;
		}

		public MenuItemStyle()
		{
		}

		public MenuItemStyle(MenuItemStyle style) : base(style)
		{
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
			ShortcutStyle = style.ShortcutStyle != null ? new LabelStyle(style.ShortcutStyle) : null;
		}

		public override WidgetStyle Clone()
		{
			return new MenuItemStyle(this);
		}
	}
}
