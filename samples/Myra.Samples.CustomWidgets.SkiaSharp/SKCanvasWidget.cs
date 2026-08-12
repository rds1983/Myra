using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using SkiaSharp;
using System;
using System.ComponentModel;

namespace Myra.Samples.SkiaSharp;

/// <summary>
/// Custom Myra widget that integrates SkiaSharp's 2D rendering capabilities.
/// This widget creates an SkiaSharp surface, renders graphics to it, then converts
/// the rendered pixels to an XNA texture for display within Myra's UI system.
/// </summary>
public class SKCanvasWidget : Widget
{
	private SKImageInfo _imageInfo;
	private SKSurface _surface = null;
	private byte[] _bytes = null;
	private Texture2D _texture = null;

	/// <summary>
	/// Override default alignment to Stretch horizontally to fill available space.
	/// </summary>
	[DefaultValue(HorizontalAlignment.Stretch)]
	public override HorizontalAlignment HorizontalAlignment
	{
		get { return base.HorizontalAlignment; }
		set { base.HorizontalAlignment = value; }
	}

	/// <summary>
	/// Override default alignment to Stretch vertically to fill available space.
	/// </summary>
	[DefaultValue(VerticalAlignment.Stretch)]
	public override VerticalAlignment VerticalAlignment
	{
		get { return base.VerticalAlignment; }
		set { base.VerticalAlignment = value; }
	}

	/// <summary>
	/// Callback delegate for custom SkiaSharp drawing operations.
	/// Receives the widget size and an SKCanvas to draw on.
	/// </summary>
	[Browsable(false)]
	public Action<Point, SKCanvas> PaintHandler;

	/// <summary>
	/// Initializes a new instance of SKCanvasWidget with default Stretch alignment.
	/// </summary>
	public SKCanvasWidget()
	{
		HorizontalAlignment = HorizontalAlignment.Stretch;
		VerticalAlignment = VerticalAlignment.Stretch;
	}

	/// <summary>
	/// Main render method that manages the SkiaSharp surface lifecycle and converts
	/// rendered pixels to an XNA texture for display within Myra.
	/// </summary>
	public override void InternalRender(RenderContext context)
	{
		base.InternalRender(context);

		var bounds = ActualBounds;

		// Create or recreate the SkiaSharp surface when widget size changes
		if (_surface == null || _imageInfo.Width != bounds.Width || _imageInfo.Height != bounds.Height)
		{
			if (_surface != null)
			{
				_surface.Dispose();
			}

			_imageInfo = new SKImageInfo(bounds.Width, bounds.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
			_surface = SKSurface.Create(_imageInfo);
		}

		// Execute custom SkiaSharp drawing operations
		var canvas = _surface.Canvas;
		Paint(new Point(bounds.Width, bounds.Height), canvas);

		// Create or recreate the XNA texture when widget size changes
		if (_texture == null || _texture.Width != bounds.Width || _texture.Height != bounds.Height)
		{
			if (_texture != null)
			{
				_texture.Dispose();
			}

			_texture = new Texture2D(MyraEnvironment.GraphicsDevice, bounds.Width, bounds.Height);
			_bytes = new byte[bounds.Width * bounds.Height * 4];
		}

		// Convert SkiaSharp surface pixels to XNA texture
		using (var image = _surface.Snapshot())
		using (var pixmap = image.PeekPixels())
		{
			var source = pixmap.GetPixelSpan();
			source.CopyTo(new Span<byte>(_bytes));
			_texture.SetData(_bytes);
		}

		// Draw the rendered texture to Myra's render context
		context.Draw(_texture, Vector2.Zero, Color.White);
	}

	/// <summary>
	/// Virtual method that can be overridden to provide custom SkiaSharp drawing.
	/// Default implementation invokes the PaintHandler delegate if assigned.
	/// </summary>
	protected virtual void Paint(Point size, SKCanvas canvas)
	{
		PaintHandler?.Invoke(size, canvas);
	}
}
