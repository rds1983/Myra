using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;
using System;

namespace Myra.Graphics2D.UI
{
	public class Image : Widget
	{
		private Drawable _drawable;

		[HiddenInEditor]
		[JsonIgnore]
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

		[HiddenInEditor]
		[JsonIgnore]
		[Obsolete("Property is obsolete and will be removed in future. Use Drawable.Color")]
		public Color Color
		{
			get
			{
				return _drawable.Color;
			}

			set
			{
				_drawable.Color = value;
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

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Drawable != null)
			{
				var bounds = ActualBounds;
				context.Draw(Drawable, bounds);
			}
		}

		public void UpdateImageSize(Drawable image)
		{
			if (image == null)
			{
				return;
			}

			if (WidthHint == null || image.Size.X > WidthHint.Value)
			{
				WidthHint = image.Size.X;
			}

			if (HeightHint == null || image.Size.Y > HeightHint.Value)
			{
				HeightHint = image.Size.Y;
			}
		}
	}
}