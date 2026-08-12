using Microsoft.Xna.Framework;
using ScottPlot;
using ScottPlot.Interactivity;
using SkiaSharp;

namespace Myra.Samples.ScottPlot;

/// <summary>
/// Custom Myra widget that inherits from SKCanvasWidget and implements ScottPlot's
/// IPlotControl interface. Provides a fully integrated ScottPlot chart widget with
/// interactive features (mouse/keyboard input, context menus, zooming, panning)
/// within Myra's UI system by leveraging the SkiaSharp rendering infrastructure.
/// </summary>
public class MyraPlot : SKCanvasWidget, IPlotControl
{
	/// <summary>
	/// The primary Plot displayed by this control.
	/// </summary>
	public Plot Plot { get; internal set; }

	/// <summary>
	/// The multiplot managed by this control.
	/// </summary>
	public IMultiplot Multiplot { get; set; }

	/// <summary>
	/// Processes UI events (mouse, keyboard) into plot actions.
	/// </summary>
	public UserInputProcessor UserInputProcessor { get; }

	/// <summary>
	/// Platform-specific logic for managing the context menu.
	/// </summary>
	public IPlotMenu Menu { get; set; }

	/// <summary>
	/// Context for hardware-accelerated graphics (null for this implementation).
	/// </summary>
	public GRContext GRContext => null;

	/// <summary>
	/// The value of the present display scaling.
	/// </summary>
	public float DisplayScale { get; set; }

	/// <summary>
	/// Initializes a new instance of MyraPlot.
	/// Sets up the Plot, Multiplot, UserInputProcessor, and display scaling.
	/// </summary>
	public MyraPlot()
	{
		Plot = new Plot { PlotControl = this };
		Multiplot = new Multiplot(Plot);
		DisplayScale = DetectDisplayScale();
		UserInputProcessor = new(this);
	}

	/// <summary>
	/// Disposes the current Plot and creates a new one for the control.
	/// </summary>
	public void Reset()
	{
		var plot = new Plot { PlotControl = this };
		Reset(plot);
	}

	/// <summary>
	/// Loads the given Plot into the control.
	/// </summary>
	public void Reset(Plot plot)
	{
		Reset(plot, disposeOldPlot: true);
	}

	/// <summary>
	/// Loads the given Plot into the control, optionally disposing the old one.
	/// </summary>
	public void Reset(Plot plot, bool disposeOldPlot)
	{
		var oldPlot = Plot;
		Plot = plot;
		if (disposeOldPlot)
			oldPlot?.Dispose();
		Plot.PlotControl = this;
		UserInputProcessor.Reset();
		Multiplot.Reset(plot);
	}

	/// <summary>
	/// Render the plot and update the image.
	/// </summary>
	public void Refresh()
	{
		// Invalidation is handled by Myra's rendering pipeline
	}

	/// <summary>
	/// Launch the default pop-up menu at the given position in the control.
	/// </summary>
	public void ShowContextMenu(Pixel position)
	{
		Menu?.ShowContextMenu(position);
	}

	/// <summary>
	/// Determine the DPI scaling ratio of the present display.
	/// A value of 1.0 means no scaling, and 1.5 means 150% scaling.
	/// </summary>
	public float DetectDisplayScale()
	{
		// Default to 1.0 for cross-platform compatibility
		// In a real implementation, this could query the platform's DPI
		return 1.0f;
	}

	/// <summary>
	/// Apply the platform-specific equivalent cursor.
	/// </summary>
	public void SetCursor(Cursor cursor)
	{
		// Cursor management would need to be implemented at the platform level
		// For now, this is a no-op in the Myra context
	}

	/// <summary>
	/// Overrides the virtual Paint method to render the ScottPlot chart.
	/// Called by SKCanvasWidget's InternalRender method.
	/// </summary>
	protected override void Paint(Point size, SKCanvas canvas)
	{
		// Clear the canvas with a white background
		canvas.Clear(SKColors.White);

		// Render the ScottPlot chart to the SkiaSharp canvas
		if (size.X > 0 && size.Y > 0)
		{
			var plotRect = new PixelRect(0, size.X, size.Y, 0);
			Multiplot.Render(canvas, plotRect);
		}

		// Flush all pending drawing operations to the surface
		canvas.Flush();
	}
}