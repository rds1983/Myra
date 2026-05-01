using System.ComponentModel;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Container = Myra.Graphics2D.UI.Container;

namespace Myra.Graphics2D.UI.WrapPanel;

public class WrapPanel : Container
{
    private readonly WrapPanelLayout _layout = new();

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

    public WrapPanel()
    {
        ChildrenLayout = _layout;
    }

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
