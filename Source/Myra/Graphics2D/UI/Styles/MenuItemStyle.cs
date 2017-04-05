using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class MenuItemStyle: ButtonStyle
	{
		public TextureRegion2D OpenBackground { get; set; }

		public int? IconWidth { get; set; }
		public int? ShortcutWidth { get; set; }
	}
}
