using System.ComponentModel;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Container = Myra.Graphics2D.UI.Container;

namespace Myra.Graphics2D.UI.WrapPanel;

/// <summary>
/// A container that arranges its child widgets in a sequence that wraps at the edge of the container.
/// </summary>
public class WrapPanel : Container
{
    private readonly WrapPanelLayout _layout = new();

    /// <summary>
    /// Gets or sets the orientation of the layout (Horizontal or Vertical).
    /// </summary>
    [Category("Layout")]
    [DefaultValue(Orientation.Horizontal)]
    public Orientation Orientation
    {
        get => _layout.Orientation;
        set
        {
            if (value == _layout.Orientation)
                return;

            _layout.Orientation = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the horizontal spacing between child widgets.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(0)]
    public int HorizontalSpacing
    {
        get => _layout.HorizontalSpacing;
        set
        {
            if (value == _layout.HorizontalSpacing)
                return;

            _layout.HorizontalSpacing = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the vertical spacing between child widgets.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(0)]
    public int VerticalSpacing
    {
        get => _layout.VerticalSpacing;
        set
        {
            if (value == _layout.VerticalSpacing)
                return;

            _layout.VerticalSpacing = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether child widgets in a row/column should be aligned to the row height/column width.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(true)]
    public bool Aligned
    {
        get => _layout.Aligned;
        set
        {
            if (value == _layout.Aligned)
                return;

            _layout.Aligned = value;
            InvalidateArrange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether all child widgets should have the same size, based on the largest child.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(true)]
    public bool UniformSizing
    {
        get => _layout.UniformSizing;
        set
        {
            if (value == _layout.UniformSizing)
                return;

            _layout.UniformSizing = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the preferred width of the wrap panel.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(null)]
    public int? PreferredWidth
    {
        get => _layout.PreferredWidth;
        set
        {
            if (value == _layout.PreferredWidth)
                return;

            _layout.PreferredWidth = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Gets or sets the preferred height of the wrap panel.
    /// </summary>
    [Category("Layout")]
    [DefaultValue(null)]
    public int? PreferredHeight
    {
        get => _layout.PreferredHeight;
        set
        {
            if (value == _layout.PreferredHeight)
                return;

            _layout.PreferredHeight = value;
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WrapPanel"/> class.
    /// </summary>
    public WrapPanel()
    {
        ChildrenLayout = _layout;
    }

    /// <summary>
    /// Copies the properties from another widget into this one.
    /// </summary>
    /// <param name="w">The widget to copy from.</param>
    protected internal override void CopyFrom(Widget w)
    {
        base.CopyFrom(w);

        var wrapPanel = (WrapPanel)w;
        Orientation = wrapPanel.Orientation;
        HorizontalSpacing = wrapPanel.HorizontalSpacing;
        VerticalSpacing = wrapPanel.VerticalSpacing;
        Aligned = wrapPanel.Aligned;
        UniformSizing = wrapPanel.UniformSizing;
        PreferredWidth = wrapPanel.PreferredWidth;
        PreferredHeight = wrapPanel.PreferredHeight;
    }
}
