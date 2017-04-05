using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class TreeStyle : WidgetStyle
	{
		public TextureRegion2D RowSelectionBackground { get; set; }
		public TextureRegion2D RowSelectionBackgroundWithoutFocus { get; set; }

		public ImageButtonStyle MarkStyle { get; set; }

		public TextBlockStyle LabelStyle { get; set; }

		public TreeStyle()
		{
			MarkStyle = new ImageButtonStyle();
			LabelStyle = new TextBlockStyle();
		}
	}
}