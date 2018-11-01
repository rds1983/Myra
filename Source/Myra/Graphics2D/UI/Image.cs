using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class Image : Widget
	{
		private IRenderable _image;
		private Color _color = Color.White;

		[HiddenInEditor]
		[JsonIgnore]
		public IRenderable Renderable
		{
			get
			{
				return _image;
			}

			set
			{
				if (value == _image)
				{
					return;
				}

				_image = value;
				InvalidateMeasure();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		[Obsolete("Use Renderable")]
		public IRenderable TextureRegion
		{
			get
			{
				return Renderable;
			}

			set
			{
				Renderable = value;
			}
		}

		[DefaultValue("#FFFFFFFF")]
		public Color Color
		{
			get
			{
				return _color;
			}

			set
			{
				_color = value;
			}
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Renderable == null)
			{
				return Point.Zero;
			}

			return Renderable.Size;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Renderable != null)
			{
				var bounds = ActualBounds;
				context.Draw(Renderable, bounds, Color);
			}
		}

		public void UpdateImageSize(IRenderable image)
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