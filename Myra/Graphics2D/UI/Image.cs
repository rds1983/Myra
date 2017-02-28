using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics2D.UI
{
	public class Image: Widget
	{
		private Drawable _drawable;

		public Drawable Drawable
		{
			get
			{
				return _drawable;
			}

			set
			{
				if (value == _drawable)
				{
					return;
				}

				_drawable = value;
				InvalidateMeasure();
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Drawable == null)
			{
				return Point.Zero;
			}

			return Drawable.Size;
		}

		public override void InternalRender(SpriteBatch batch)
		{
			base.InternalRender(batch);

			if (Drawable != null)
			{
				var bounds = AbsoluteBounds;
				Drawable.Draw(batch, bounds);
			}
		}


		public void UpdateImageSize(Drawable drawable)
		{
			if (drawable == null)
			{
				return;
			}

			if (WidthHint == null || drawable.Size.X > WidthHint.Value)
			{
				WidthHint = drawable.Size.X;
			}

			if (HeightHint == null || drawable.Size.Y > HeightHint.Value)
			{
				HeightHint = drawable.Size.Y;
			}
		}
	}
}
