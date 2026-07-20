using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using NvgSharp;
using System;

namespace Myra.Samples;

public class NvgCanvasWidget : Widget
{
	private NvgContext _nvgContext;
	private bool _edgeAntialiasing = true, _stencilStrokes = true;

	public bool EdgeAntialiasing
	{
		get => _edgeAntialiasing;

		set
		{
			if (value == _edgeAntialiasing)
			{
				return;
			}

			_edgeAntialiasing = value;
			_nvgContext = null;
		}
	}

	public bool StencilStrokes
	{
		get => _stencilStrokes;

		set
		{
			if (value == _stencilStrokes)
			{
				return;
			}

			_stencilStrokes = value;
			_nvgContext = null;
		}
	}

	public Action<NvgContext, Point> PaintHandler;


	public NvgCanvasWidget()
	{
		HorizontalAlignment = HorizontalAlignment.Stretch;
		VerticalAlignment = VerticalAlignment.Stretch;
	}

	public override void InternalRender(RenderContext context)
	{
		base.InternalRender(context);

		// Temporarily end the context
		context.End();

		var device = MyraEnvironment.GraphicsDevice;

		var oldViewPort = device.Viewport;

		var bounds = ActualBounds;
		var screenPosition = ToGlobal(Point.Zero);
		device.Viewport = new Viewport(screenPosition.X, screenPosition.Y, bounds.Width, bounds.Height);

		if (_nvgContext == null)
		{
			_nvgContext = new NvgContext(device, EdgeAntialiasing, StencilStrokes);
		}

		_nvgContext.ResetState();

		OnRender(_nvgContext, new Point(bounds.Width, bounds.Height));

		_nvgContext.Flush();

		device.Viewport = oldViewPort;

		// Don't forget to start the context again
		context.Begin();
	}

	protected virtual void OnRender(NvgContext context, Point size)
	{
		PaintHandler?.Invoke(context, size);
	}
}
