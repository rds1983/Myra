# Basics
`WrapPanel` is a container that arranges its child widgets in a sequence and wraps them to a new row (or column) when they no longer fit the available space. Unlike the [StackPanel](stackpanel-layout.md), which lays children out in a single line, the WrapPanel continues on the next line instead of overflowing.

The following code lays out 4 widgets that wrap when the width of the panel is exceeded:
```c#
var wrapPanel = new WrapPanel
{
  HorizontalSpacing = 8,
  Width = 100,
  Height = 50
};

wrapPanel.Widgets.Add(new Label
{
  Text = "Test1"
});

wrapPanel.Widgets.Add(new Label
{
  Text = "Test2"
});

wrapPanel.Widgets.Add(new Label
{
  Text = "Test3"
});

wrapPanel.Widgets.Add(new Label
{
  Text = "Test4"
});
```

It is equivalent to the following [MML](MML.md):
```xml
<Project>
  <WrapPanel HorizontalSpacing="8" Width="100" Height="50">
    <Label Text="Test1" />
    <Label Text="Test2" />
    <Label Text="Test3" />
    <Label Text="Test4" />
  </WrapPanel>
</Project>
```

This is the sample shipped with the project in `samples/UI/wrapPanel.xmmp`.

# Properties
WrapPanel exposes the following layout-related properties:

Name|Type|Default|Description
----|----|-------|-----------
Orientation|Orientation|Horizontal|Gets or sets the axis that is used first. When `Horizontal`, widgets are laid out in rows and wrap to a new row when the width is exceeded. When `Vertical`, widgets are laid out in columns and wrap to a new column when the height is exceeded.
HorizontalSpacing|int|0|Gets or sets the spacing between widgets in a row.
VerticalSpacing|int|0|Gets or sets the spacing between rows/columns.
Aligned|bool|true|Gets or sets a value indicating whether widgets in a row/column should be aligned to the row height/column width.
UniformSizing|bool|true|Gets or sets a value indicating whether all widgets should have the same size, based on the largest widget.
PreferredWidth|int?|null|Gets or sets the preferred width of the panel, used for measurement and arrangement when the available width is unbounded.
PreferredHeight|int?|null|Gets or sets the preferred height of the panel, used for measurement and arrangement when the available height is unbounded.

