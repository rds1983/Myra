using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ImageStyle: WidgetStyle
	{
		public TextureRegion Image { get; set; }
		public TextureRegion OverImage { get; set; }

		public ImageStyle()
		{
		}

		public ImageStyle(ImageStyle style): base(style)
		{
			Image = style.Image;
			OverImage = style.OverImage;
		}

		public override WidgetStyle Clone()
		{
			return new ImageStyle(this);
		}
	}
}
