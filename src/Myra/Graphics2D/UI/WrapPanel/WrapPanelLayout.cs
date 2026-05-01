using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Graphics2D.UI.WrapPanel;

/// <summary>
/// Implements a layout that arranges widgets in rows or columns, wrapping when the edge is reached.
/// </summary>
public class WrapPanelLayout : ILayout
{
    /// <summary>
    /// Gets or sets the orientation of the layout.
    /// </summary>
    /// <remarks>
    /// The orientation determines what axis of space will be used first;
    /// For <see cref="Orientation.Horizontal"/>, horizontal space will be used first, wrapping
    /// to a new row when necessary.
    /// For <see cref="Orientation.Vertical"/>, vertical space will be used first, wrapping
    /// to a new column when necessary.
    /// </remarks>
    public Orientation Orientation { get; set; } = Orientation.Horizontal;

    /// <summary>
    /// Gets or sets the horizontal spacing between widgets.
    /// </summary>
    public int HorizontalSpacing { get; set; }

    /// <summary>
    /// Gets or sets the vertical spacing between widgets.
    /// </summary>
    public int VerticalSpacing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether widgets in a row/column should be aligned to the row height/column width.
    /// </summary>
    public bool Aligned { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether all widgets should have the same size, based on the largest widget.
    /// </summary>
    public bool UniformSizing { get; set; } = true;

    /// <summary>
    /// Gets or sets the preferred width for measurement and arrangement.
    /// </summary>
    public int? PreferredWidth { get; set; }

    /// <summary>
    /// Gets or sets the preferred height for measurement and arrangement.
    /// </summary>
    public int? PreferredHeight { get; set; }

    /// <summary>
    /// Calculates the uniform size for all widgets based on the largest widget's measurements.
    /// </summary>
    /// <param name="widgets">The collection of widgets from which to determine the 'max' widget size </param>
    /// <param name="availableSize">The available size for measurement.</param>
    /// <returns>The uniform size for widgets.</returns>
    private static Point GetUniformSize(IEnumerable<Widget> widgets, Point availableSize)
    {
        int maxChildWidth = 0;
        int maxChildHeight = 0;

        foreach (Widget widget in widgets)
        {
            if (!widget.Visible)
                continue;

            Point size = widget.Measure(availableSize);
            maxChildWidth = Math.Max(maxChildWidth, size.X);
            maxChildHeight = Math.Max(maxChildHeight, size.Y);
        }

        return new Point(maxChildWidth, maxChildHeight);
    }

    /// <summary>
    /// Measures the total size required for the widgets given the available size.
    /// </summary>
    /// <param name="widgets">The collection of widgets to measure.</param>
    /// <param name="availableSize">The available size for measurement.</param>
    /// <returns>The required size for the layout.</returns>
    public Point Measure(IEnumerable<Widget> widgets, Point availableSize)
    {
        // The measurement methods are pretty difficult to keep DRY;
        // We either it somewhat duplicated or we start having to pass around
        // many parameters.
        return Orientation == Orientation.Horizontal
            ? MeasureWithHorizontalBias(widgets, availableSize)
            : MeasureWithVerticalBias(widgets, availableSize);
    }

    /// <summary>
    /// Measures the total size required for the widgets given the available size.
    /// This has a horizontal bias, meaning it'll attempt to use whatever horizontal space is
    /// available prior to wrapping into a new row
    /// </summary>
    /// <param name="widgets">The collection of widgets to measure.</param>
    /// <param name="availableSize">The available size for measurement.</param>
    /// <returns>The required size for the layout.</returns>
    private Point MeasureWithHorizontalBias(IEnumerable<Widget> widgets, Point availableSize)
    {
        Point result = Point.Zero;
        int rowWidth = 0;
        int rowHeight = 0;

        Point effectiveAvailableSize = GetEffectiveAvailableSize(availableSize);
        Widget[] widgetsArr = widgets.ToArray();

        // Determine the uniform size if uniform sizing is enabled
        Point uniformSize = UniformSizing ? GetUniformSize(widgetsArr, effectiveAvailableSize) : Point.Zero;

        bool firstInRow = true;
        foreach (Widget widget in widgetsArr)
        {
            if (!widget.Visible)
                continue;

            Point widgetSize = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

            // Check if the current widget exceeds the available row width
            if (!firstInRow && effectiveAvailableSize.X > 0 &&
                rowWidth + HorizontalSpacing + widgetSize.X > effectiveAvailableSize.X)
            {
                // Move to the next row
                result.X = Math.Max(result.X, rowWidth);
                result.Y += rowHeight + VerticalSpacing;
                rowWidth = widgetSize.X;
                rowHeight = widgetSize.Y;
            }
            else
            {
                // Add widget to the current row
                if (!firstInRow)
                    rowWidth += HorizontalSpacing;
                rowWidth += widgetSize.X;
                rowHeight = Math.Max(rowHeight, widgetSize.Y);
            }

            firstInRow = false;
        }

        // Finalize measurement for the last row
        result.X = Math.Max(result.X, rowWidth);
        result.Y += rowHeight;

        if (PreferredWidth.HasValue && availableSize.X is <= 0 or >= 1000000)
            result.X = Math.Max(result.X, PreferredWidth.Value);

        return result;
    }

    /// <summary>
    /// Measures the total size required for the widgets given the available size.
    /// This has a vertical bias, meaning it'll attempt to use whatever vertical space is
    /// available prior to wrapping into a new column
    /// </summary>
    /// <param name="widgets">The collection of widgets to measure.</param>
    /// <param name="availableSize">The available size for measurement.</param>
    /// <returns>The required size for the layout.</returns>
    private Point MeasureWithVerticalBias(IEnumerable<Widget> widgets, Point availableSize)
    {
        Point result = Point.Zero;
        int rowWidth = 0;
        int rowHeight = 0;

        Point effectiveAvailableSize = GetEffectiveAvailableSize(availableSize);
        Widget[] widgetsArr = widgets.ToArray();

        // Determine the uniform size if uniform sizing is enabled
        Point uniformSize = UniformSizing ? GetUniformSize(widgetsArr, effectiveAvailableSize) : Point.Zero;

        bool firstInCol = true;
        foreach (Widget widget in widgetsArr)
        {
            if (!widget.Visible)
                continue;

            Point widgetSize = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

            // Check if the current widget exceeds the available column height
            if (!firstInCol && effectiveAvailableSize.Y > 0 &&
                rowHeight + VerticalSpacing + widgetSize.Y > effectiveAvailableSize.Y)
            {
                // Move to the next column
                result.Y = Math.Max(result.Y, rowHeight);
                result.X += rowWidth + HorizontalSpacing;
                rowHeight = widgetSize.Y;
                rowWidth = widgetSize.X;
            }
            else
            {
                // Add widget to the current column
                if (!firstInCol)
                    rowHeight += VerticalSpacing;
                rowHeight += widgetSize.Y;
                rowWidth = Math.Max(rowWidth, widgetSize.X);
            }

            firstInCol = false;
        }

        // Finalize measurement for the last column
        result.Y = Math.Max(result.Y, rowHeight);
        result.X += rowWidth;

        if (PreferredHeight.HasValue && availableSize.Y is <= 0 or >= 1000000)
            result.Y = Math.Max(result.Y, PreferredHeight.Value);

        return result;
    }

    /// <summary>
    /// Gets the effective available size, considering the preferred width/height and orientation.
    /// </summary>
    /// <param name="availableSize">The original available size.</param>
    /// <returns>The effective available size for layout calculations.</returns>
    private Point GetEffectiveAvailableSize(Point availableSize)
    {
        Point effectiveAvailableSize = availableSize;
        switch (Orientation)
        {
            case Orientation.Horizontal:
            default:
            {
                if (PreferredWidth.HasValue)
                    effectiveAvailableSize.X = availableSize.X is <= 0 or >= 1000000
                        ? PreferredWidth.Value
                        : Math.Min(availableSize.X, PreferredWidth.Value);
                break;
            }
            case Orientation.Vertical:
            {
                if (PreferredHeight.HasValue)
                    effectiveAvailableSize.Y = availableSize.Y is <= 0 or >= 1000000
                        ? PreferredHeight.Value
                        : Math.Min(availableSize.Y, PreferredHeight.Value);
                break;
            }
        }

        return effectiveAvailableSize;
    }

    /// <summary>
    /// Arranges the widgets within the specified bounds.
    /// </summary>
    /// <param name="widgets">The collection of widgets to arrange.</param>
    /// <param name="bounds">The bounds to arrange the widgets within.</param>
    public void Arrange(IEnumerable<Widget> widgets, Rectangle bounds)
    {
        if (Orientation == Orientation.Horizontal)
            ArrangeHorizontal(widgets, bounds);
        else
            ArrangeVertical(widgets, bounds);
    }

    /// <summary>
    /// Arranges widgets horizontally in rows.
    /// </summary>
    /// <param name="widgets">The collection of widgets to arrange.</param>
    /// <param name="bounds">The bounds to arrange the widgets within.</param>
    private void ArrangeHorizontal(IEnumerable<Widget> widgets, Rectangle bounds)
    {
        Point actualAvailableSize = bounds.Size();
        Point effectiveAvailableSize = actualAvailableSize;

        if (PreferredWidth.HasValue)
            effectiveAvailableSize.X = actualAvailableSize.X is <= 0 or >= 1000000
                ? PreferredWidth.Value
                : Math.Min(actualAvailableSize.X, PreferredWidth.Value);

        int x = bounds.X;
        int y = bounds.Y;
        int rowHeight = 0;
        var rowWidgets = new List<Widget>();

        Widget[] widgetsArr = widgets.ToArray();
        Point uniformSize = UniformSizing ? GetUniformSize(widgetsArr, effectiveAvailableSize) : Point.Zero;

        foreach (Widget widget in widgetsArr)
        {
            if (!widget.Visible) continue;

            Point size = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

            // Wrap to next row if current widget exceeds horizontal bounds
            if (rowWidgets.Count > 0 && effectiveAvailableSize.X > 0 &&
                x + HorizontalSpacing + size.X > bounds.X + effectiveAvailableSize.X)
            {
                // Arrange previous row
                ArrangeRow(rowWidgets, bounds.X, y, rowHeight, effectiveAvailableSize, uniformSize);

                y += rowHeight + VerticalSpacing;
                x = bounds.X;
                rowHeight = 0;
                rowWidgets.Clear();
            }

            if (rowWidgets.Count > 0) x += HorizontalSpacing;

            rowWidgets.Add(widget);
            x += size.X;
            rowHeight = Math.Max(rowHeight, size.Y);
        }

        // Arrange the last remaining row
        if (rowWidgets.Count > 0)
            ArrangeRow(rowWidgets, bounds.X, y, rowHeight, effectiveAvailableSize, uniformSize);
    }

    /// <summary>
    /// Arranges widgets vertically in columns.
    /// </summary>
    /// <param name="widgets">The collection of widgets to arrange.</param>
    /// <param name="bounds">The bounds to arrange the widgets within.</param>
    private void ArrangeVertical(IEnumerable<Widget> widgets, Rectangle bounds)
    {
        Point actualAvailableSize = bounds.Size();
        Point effectiveAvailableSize = actualAvailableSize;

        if (PreferredHeight.HasValue)
            effectiveAvailableSize.Y = actualAvailableSize.Y is <= 0 or >= 1000000
                ? PreferredHeight.Value
                : Math.Min(actualAvailableSize.Y, PreferredHeight.Value);

        int x = bounds.X;
        int y = bounds.Y;
        int rowWidth = 0;
        var colWidgets = new List<Widget>();

        Widget[] widgetsArr = widgets.ToArray();
        Point uniformSize = UniformSizing ? GetUniformSize(widgetsArr, effectiveAvailableSize) : Point.Zero;

        foreach (Widget widget in widgetsArr)
        {
            if (!widget.Visible) continue;

            Point size = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

            // Wrap to next column if current widget exceeds vertical bounds
            if (colWidgets.Count > 0 && effectiveAvailableSize.Y > 0 &&
                y + VerticalSpacing + size.Y > bounds.Y + effectiveAvailableSize.Y)
            {
                // Arrange previous column
                ArrangeCol(colWidgets, x, bounds.Y, rowWidth, effectiveAvailableSize, uniformSize);

                x += rowWidth + HorizontalSpacing;
                y = bounds.Y;
                rowWidth = 0;
                colWidgets.Clear();
            }

            if (colWidgets.Count > 0) y += VerticalSpacing;

            colWidgets.Add(widget);
            y += size.Y;
            rowWidth = Math.Max(rowWidth, size.X);
        }

        // Arrange the last remaining column
        if (colWidgets.Count > 0)
            ArrangeCol(colWidgets, x, bounds.Y, rowWidth, effectiveAvailableSize, uniformSize);
    }

    /// <summary>
    /// Finalizes the arrangement of a single row.
    /// </summary>
    /// <param name="widgets">The widgets in the row.</param>
    /// <param name="x">The starting X coordinate.</param>
    /// <param name="y">The starting Y coordinate.</param>
    /// <param name="rowHeight">The height of the row.</param>
    /// <param name="availableSize">The total available size for measurement.</param>
    /// <param name="uniformSize">The uniform size for widgets, if enabled.</param>
    private void ArrangeRow(List<Widget> widgets, int x, int y, int rowHeight, Point availableSize, Point uniformSize)
    {
        foreach (Widget widget in widgets)
        {
            Point size = UniformSizing ? uniformSize : widget.Measure(availableSize);
            int height = Aligned ? rowHeight : size.Y;
            var widgetBounds = new Rectangle(x, y, size.X, height);
            widget.Arrange(widgetBounds);

            x += size.X + HorizontalSpacing;
        }
    }

    /// <summary>
    /// Finalizes the arrangement of a single column.
    /// </summary>
    /// <param name="widgets">The widgets in the column.</param>
    /// <param name="x">The starting X coordinate.</param>
    /// <param name="y">The starting Y coordinate.</param>
    /// <param name="colWidth">The width of the column.</param>
    /// <param name="availableSize">The total available size for measurement.</param>
    /// <param name="uniformSize">The uniform size for widgets, if enabled.</param>
    private void ArrangeCol(List<Widget> widgets, int x, int y, int colWidth, Point availableSize, Point uniformSize)
    {
        foreach (Widget widget in widgets)
        {
            Point size = UniformSizing ? uniformSize : widget.Measure(availableSize);
            int width = Aligned ? colWidth : size.X;
            var widgetBounds = new Rectangle(x, y, width, size.Y);
            widget.Arrange(widgetBounds);

            y += size.Y + VerticalSpacing;
        }
    }
}