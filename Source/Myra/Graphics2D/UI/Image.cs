using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.UI
{
	public class Image: Widget
	{
		private TextureRegion2D _TextureRegion2D;

		public TextureRegion2D TextureRegion2D
		{
			get
			{
				return _TextureRegion2D;
			}

			set
			{
				if (value == _TextureRegion2D)
				{
					return;
				}

				_TextureRegion2D = value;
				InvalidateMeasure();
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (TextureRegion2D == null)
			{
				return Point.Zero;
			}

			return new Point((int)TextureRegion2D.Size.Width, (int)TextureRegion2D.Size.Height);
		}

		public override void InternalRender(SpriteBatch batch)
		{
			base.InternalRender(batch);

			if (TextureRegion2D != null)
			{
				var bounds = AbsoluteBounds;
				batch.Draw(TextureRegion2D, bounds);
			}
		}


		public void UpdateImageSize(TextureRegion2D image)
		{
			if (image == null)
			{
				return;
			}

			if (WidthHint == null || image.Size.Width > WidthHint.Value)
			{
				WidthHint = (int)image.Size.Width;
			}

			if (HeightHint == null || image.Size.Height > HeightHint.Value)
			{
				HeightHint = (int)image.Size.Height;
			}
		}
	}
}
