using Microsoft.Xna.Framework;
using Myra.Graphics2D.TextureAtlases;

namespace Myra.Graphics2D.UI
{
	public class Image: Widget
	{
		private TextureRegion _TextureRegion;

		public TextureRegion TextureRegion
		{
			get
			{
				return _TextureRegion;
			}

			set
			{
				if (value == _TextureRegion)
				{
					return;
				}

				_TextureRegion = value;
				InvalidateMeasure();
			}
		}

		public Color Color { get; set; }

		public Image()
		{
			Color = Color.White;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (TextureRegion == null)
			{
				return Point.Zero;
			}

			return new Point(TextureRegion.Bounds.Width, (int)TextureRegion.Bounds.Height);
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (TextureRegion != null)
			{
				var bounds = ActualBounds;
				context.Draw(TextureRegion, bounds, Color);
			}
		}

		public void UpdateImageSize(TextureRegion image)
		{
			if (image == null)
			{
				return;
			}

			if (WidthHint == null || image.Bounds.Width > WidthHint.Value)
			{
				WidthHint = (int)image.Bounds.Width;
			}

			if (HeightHint == null || image.Bounds.Height > HeightHint.Value)
			{
				HeightHint = (int)image.Bounds.Height;
			}
		}
	}
}
