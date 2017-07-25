using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ListItemStyle: ButtonStyle
	{
		public TextureRegion2D SelectedBackground { get; set; }

		public ListItemStyle()
		{
		}

		public ListItemStyle(ListItemStyle style) : base(style)
		{
			SelectedBackground = style.SelectedBackground;
		}

		public override WidgetStyle Clone()
		{
			return new ListItemStyle(this);
		}
	}
}
