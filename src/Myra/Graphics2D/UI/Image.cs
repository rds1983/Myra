using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class Image : Widget
	{
		private IRenderable _image, _overImage, _pressedImage;
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
		public IRenderable OverRenderable
		{
			get
			{
				return _overImage;
			}

			set
			{
				if (value == _overImage)
				{
					return;
				}

				_overImage = value;
				InvalidateMeasure();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public IRenderable PressedRenderable
		{
			get
			{
				return _pressedImage;
			}

			set
			{
				if (value == _pressedImage)
				{
					return;
				}

				_pressedImage = value;
				InvalidateMeasure();
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public bool IsPressed
		{
			get; set;
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
			var result = _image != null ? _image.Size : Point.Zero;

			var overSize = _overImage != null ? _overImage.Size : Point.Zero;
			if (overSize.X > result.X)
			{
				result.X = overSize.X;
			}

			if (overSize.Y > result.Y)
			{
				result.Y = overSize.Y;
			}

			var pressedSize = _pressedImage != null ? _pressedImage.Size : Point.Zero;
			if (pressedSize.X > result.X)
			{
				result.X = pressedSize.X;
			}

			if (pressedSize.Y > result.Y)
			{
				result.Y = pressedSize.Y;
			}

			return result;
		}

		public override void InternalRender(RenderContext context)
		{
			var image = Renderable;

			if (IsMouseOver && OverRenderable != null)
			{
				image = OverRenderable;
			}

			if (IsPressed && PressedRenderable != null)
			{
				image = PressedRenderable;
			}

			if (image != null)
			{
				var bounds = ActualBounds;
				context.Draw(image, bounds, Color);
			}
		}
	}
}