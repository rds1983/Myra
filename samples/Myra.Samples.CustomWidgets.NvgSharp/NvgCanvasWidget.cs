using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using NvgSharp;
using System;

namespace Myra.Samples;

/// <summary>
/// A custom Myra widget that provides a canvas for NvgSharp vector graphics rendering.
/// Wraps an <see cref="NvgContext"/> and hooks into Myra's render pipeline to draw
/// NanoVG-style 2D vector graphics within the widget layout system.
/// </summary>
public class NvgCanvasWidget : Widget
{
	private NvgContext _nvgContext;
	private bool _edgeAntialiasing = true, _stencilStrokes = true;

	/// <summary>
	/// Gets or sets whether edge antialiasing is enabled for NvgSharp rendering.
	/// Changing this value recreates the <see cref="NvgContext"/> on the next render.
	/// </summary>
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

	/// <summary>
	/// Gets or sets whether stencil-based strokes are used for NvgSharp rendering.
	/// Changing this value recreates the <see cref="NvgContext"/> on the next render.
	/// </summary>
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

	/// <summary>
	/// Delegate invoked during rendering. Receives the active <see cref="NvgContext"/>
	/// and the size of the canvas in pixels. Assign this to perform custom drawing.
	/// </summary>
	public Action<NvgContext, Point> PaintHandler;


	public NvgCanvasWidget()
	{
		HorizontalAlignment = HorizontalAlignment.Stretch;
		VerticalAlignment = VerticalAlignment.Stretch;
	}

	/// <summary>
	/// Renders the NvgSharp canvas. Temporarily suspends Myra's render context,
	/// sets the GPU viewport to the widget's bounds, draws via <see cref="OnRender"/>,
	/// then restores the original context state.
	/// </summary>
	public override void InternalRender(RenderContext context)
	{
		base.InternalRender(context);

		// Temporarily end the Myra render context so we can issue raw GPU commands
		context.End();

		var device = MyraEnvironment.GraphicsDevice;

		// Save and replace the viewport so NvgSharp draws inside this widget's bounds
		var oldViewPort = device.Viewport;

		var bounds = ActualBounds;
		var screenPosition = ToGlobal(Point.Zero);
		device.Viewport = new Viewport(screenPosition.X, screenPosition.Y, bounds.Width, bounds.Height);

		// Lazily create the NvgContext (recreated when antialiasing/stencil settings change)
		if (_nvgContext == null)
		{
			_nvgContext = new NvgContext(device, EdgeAntialiasing, StencilStrokes);
		}

		_nvgContext.ResetState();

		OnRender(_nvgContext, new Point(bounds.Width, bounds.Height));

		_nvgContext.Flush();

		device.Viewport = oldViewPort;

		// Restart the Myra render context
		context.Begin();
	}

	/// <summary>
	/// Called during rendering to draw NvgSharp content. Override in a derived class
	/// to perform custom vector drawing. The default implementation invokes <see cref="PaintHandler"/>.
	/// </summary>
	/// <param name="context">The NvgSharp context to draw with.</param>
	/// <param name="size">The pixel dimensions of the canvas.</param>
	protected virtual void OnRender(NvgContext context, Point size)
	{
		PaintHandler?.Invoke(context, size);
	}
}
