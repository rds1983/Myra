using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Graphics2D.UI.WrapPanel;

public class WrapPanelLayout : ILayout
{
    public Orientation Orientation { get; set; } = Orientation.Horizontal;
    public int HorizontalSpacing { get; set; }
    public int VerticalSpacing { get; set; }
    public bool Aligned { get; set; } = true;
    public bool UniformSizing { get; set; } = true;
    public int? PreferredWidth { get; set; }
    public int? PreferredHeight { get; set; }

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

    public Point Measure(IEnumerable<Widget> widgets, Point availableSize)
    {
        Point result = Point.Zero;
        int rowWidth = 0;
        int rowHeight = 0;

        Point effectiveAvailableSize = GetEffectiveAvailableSize(availableSize);
        Widget[] widgetsArr = widgets.ToArray();
        Point uniformSize = UniformSizing ? GetUniformSize(widgetsArr, effectiveAvailableSize) : Point.Zero;

        if (Orientation == Orientation.Horizontal)
        {
            bool firstInRow = true;
            foreach (Widget widget in widgetsArr)
            {
                if (!widget.Visible) continue;

                Point size = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

                if (!firstInRow && effectiveAvailableSize.X > 0 &&
                    rowWidth + HorizontalSpacing + size.X > effectiveAvailableSize.X)
                {
                    result.X = Math.Max(result.X, rowWidth);
                    result.Y += rowHeight + VerticalSpacing;
                    rowWidth = size.X;
                    rowHeight = size.Y;
                    firstInRow = true;
                }
                else
                {
                    if (!firstInRow) rowWidth += HorizontalSpacing;
                    rowWidth += size.X;
                    rowHeight = Math.Max(rowHeight, size.Y);
                    firstInRow = false;
                }
            }

            result.X = Math.Max(result.X, rowWidth);
            result.Y += rowHeight;

            if (PreferredWidth.HasValue && availableSize.X is <= 0 or >= 1000000)
                result.X = Math.Max(result.X, PreferredWidth.Value);
        }
        else
        {
            bool firstInCol = true;
            foreach (Widget widget in widgetsArr)
            {
                if (!widget.Visible) continue;

                Point size = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

                if (!firstInCol && effectiveAvailableSize.Y > 0 &&
                    rowHeight + VerticalSpacing + size.Y > effectiveAvailableSize.Y)
                {
                    result.Y = Math.Max(result.Y, rowHeight);
                    result.X += rowWidth + HorizontalSpacing;
                    rowHeight = size.Y;
                    rowWidth = size.X;
                    firstInCol = true;
                }
                else
                {
                    if (!firstInCol) rowHeight += VerticalSpacing;
                    rowHeight += size.Y;
                    rowWidth = Math.Max(rowWidth, size.X);
                    firstInCol = false;
                }
            }

            result.Y = Math.Max(result.Y, rowHeight);
            result.X += rowWidth;

            if (PreferredHeight.HasValue && availableSize.Y is <= 0 or >= 1000000)
                result.Y = Math.Max(result.Y, PreferredHeight.Value);
        }

        return result;
    }

    private Point GetEffectiveAvailableSize(Point availableSize)
    {
        Point effectiveAvailableSize = availableSize;
        switch (Orientation)
        {
            case Orientation.Horizontal:
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

    public void Arrange(IEnumerable<Widget> widgets, Rectangle bounds)
    {
        if (Orientation == Orientation.Horizontal)
            ArrangeHorizontal(widgets, bounds);
        else
            ArrangeVertical(widgets, bounds);
    }

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
        Point uniformSize = UniformSizing ? GetUniformSize(widgets, effectiveAvailableSize) : Point.Zero;

        foreach (Widget widget in widgets)
        {
            if (!widget.Visible) continue;

            Point size = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

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

        if (rowWidgets.Count > 0)
            ArrangeRow(rowWidgets, bounds.X, y, rowHeight, effectiveAvailableSize, uniformSize);
    }

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
        Point uniformSize = UniformSizing ? GetUniformSize(widgets, effectiveAvailableSize) : Point.Zero;

        foreach (Widget widget in widgets)
        {
            if (!widget.Visible) continue;

            Point size = UniformSizing ? uniformSize : widget.Measure(effectiveAvailableSize);

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

        if (colWidgets.Count > 0)
            ArrangeCol(colWidgets, x, bounds.Y, rowWidth, effectiveAvailableSize, uniformSize);
    }

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
