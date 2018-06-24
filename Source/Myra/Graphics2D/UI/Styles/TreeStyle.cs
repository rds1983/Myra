using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class TreeStyle : WidgetStyle
	{
		public TextureRegion RowSelectionBackground { get; set; }
		public TextureRegion RowSelectionBackgroundWithoutFocus { get; set; }
		public TextureRegion RowHoverBackground { get; set; }

		public ImageButtonStyle MarkStyle { get; set; }
		public TextBlockStyle LabelStyle { get; set; }

		public TreeStyle()
		{
			MarkStyle = new ImageButtonStyle();
			LabelStyle = new TextBlockStyle();
		}

		public TreeStyle(TreeStyle style)
		{
			RowSelectionBackground = style.RowSelectionBackground;
			RowSelectionBackgroundWithoutFocus = style.RowSelectionBackgroundWithoutFocus;
			RowHoverBackground = style.RowHoverBackground;

			MarkStyle = new ImageButtonStyle(style.MarkStyle);
			LabelStyle = new TextBlockStyle(style.LabelStyle);
		}

		public override WidgetStyle Clone()
		{
			return new TreeStyle(this);
		}
	}
}