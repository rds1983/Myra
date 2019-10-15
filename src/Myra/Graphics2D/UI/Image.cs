using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class Image : Widget
	{
		private IRenderable _image, _overImage, _pressedImage;
		private Color _color = Color.White;

		[Browsable(false)]
		[XmlIgnore]
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

		[Browsable(false)]
		[XmlIgnore]
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

		[Browsable(false)]
		[XmlIgnore]
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

		[Browsable(false)]
		[XmlIgnore]
		public bool IsPressed
		{
			get; set;
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

			if (UseHoverRenderable && OverRenderable != null)
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

		public void ApplyPressableImageStyle(PressableImageStyle imageStyle)
		{
			ApplyWidgetStyle(imageStyle);

			Renderable = imageStyle.Image;
			OverRenderable = imageStyle.OverImage;
			PressedRenderable = imageStyle.PressedImage;
		}
	}
}