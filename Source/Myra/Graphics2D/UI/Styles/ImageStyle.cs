using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI.Styles
{
	public class ImageStyle: WidgetStyle
	{
		public TextureRegion2D Image { get; set; }
		public TextureRegion2D OverImage { get; set; }

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
