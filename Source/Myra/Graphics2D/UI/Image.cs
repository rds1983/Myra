using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Image: Widget
	{
		private TextureRegion _textureRegion;

		[HiddenInEditor]
		[JsonIgnore]
		public TextureRegion TextureRegion
		{
			get
			{
				return _textureRegion;
			}

			set
			{
				if (value == _textureRegion)
				{
					return;
				}

				_textureRegion = value;
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
