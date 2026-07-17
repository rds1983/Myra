using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using SkiaSharp;
using System;
using System.ComponentModel;

namespace Myra.SkiaSharp;

public class SKCanvasWidget : Widget
{
	private SKImageInfo _imageInfo;
	private SKSurface _surface = null;
	private byte[] _bytes = null;
	private Texture2D _texture = null;

	[DefaultValue(HorizontalAlignment.Stretch)]
	public override HorizontalAlignment HorizontalAlignment
	{
		get { return base.HorizontalAlignment; }
		set { base.HorizontalAlignment = value; }
	}

	[DefaultValue(VerticalAlignment.Stretch)]
	public override VerticalAlignment VerticalAlignment
	{
		get { return base.VerticalAlignment; }
		set { base.VerticalAlignment = value; }
	}

	[Browsable(false)]
	public Action<Point, SKCanvas> PaintHandler;

	public SKCanvasWidget()
	{
		HorizontalAlignment = HorizontalAlignment.Stretch;
		VerticalAlignment = VerticalAlignment.Stretch;
	}

	public override void InternalRender(RenderContext context)
	{
		base.InternalRender(context);

		var bounds = ActualBounds;


		if (_surface == null || _imageInfo.Width != bounds.Width || _imageInfo.Height != bounds.Height)
		{
			if (_surface != null)
			{
				_surface.Dispose();
			}

			_imageInfo = new SKImageInfo(bounds.Width, bounds.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
			_surface = SKSurface.Create(_imageInfo);
		}

		var canvas = _surface.Canvas;
		Paint(new Point(bounds.Width, bounds.Height), canvas);

		if (_texture == null || _texture.Width != bounds.Width || _texture.Height != bounds.Height)
		{
			_texture = new Texture2D(MyraEnvironment.GraphicsDevice, bounds.Width, bounds.Height);
			_bytes = new byte[bounds.Width * bounds.Height * 4];
		}

		using (var image = _surface.Snapshot())
		using (var pixmap = image.PeekPixels())
		{
			var source = pixmap.GetPixelSpan();
			source.CopyTo(new Span<byte>(_bytes));
			_texture.SetData(_bytes);
		}

		context.Draw(_texture, Vector2.Zero, Color.White);
	}

	protected virtual void Paint(Point size, SKCanvas canvas)
	{
		PaintHandler?.Invoke(size, canvas);
	}
}
