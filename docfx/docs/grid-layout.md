# Basics
Grid is a container. It partitions the available space into cells and places the child widgets there. The partitioning configuration should be explicity set by creating Proportion objects, setting their properties and adding to either RowsProportions or ColumnsProportions property.

Every child Widget should also explicity specify which cell(s) it would occupy by setting attached properties GridColumn, GridRow, GridColumnSpan and GridRowSpan.

I.e. following code describes simple 2x2 grid with 8 pixels spacing beetween rows and columns, and 3 child widgets:
```c#
// Create grid
var grid = new Grid
{
  ShowGridLines = true,
  ColumnSpacing = 8,
  RowSpacing = 8
};

// Set partitioning configuration
grid.ColumnsProportions.Add(new Proportion());
grid.ColumnsProportions.Add(new Proportion());
grid.RowsProportions.Add(new Proportion());
grid.RowsProportions.Add(new Proportion());

// Add widgets
var button = new Button
{
  Content = new Label
  {
    Text = "Button"
  }
};
grid.Widgets.Add(button);

var longButton = new Button
{
  Content = new Label
  {
    Text = "Long Button"
  }
};
Grid.SetColumn(longButton, 1);
grid.Widgets.Add(longButton);

var veryLongButton = new Button
{
  Content = new Label
  {
    Text = "Very Long Button"
  }
};
Grid.SetRow(veryLongButton, 1);
Grid.SetColumnSpan(veryLongButton, 2);
grid.Widgets.Add(veryLongButton);
```

It is equivalent to the [MML](MML.md):
```xml
<Project>
  <Grid ShowGridLines="True" ColumnSpacing="8" RowSpacing="8">
    <Grid.ColumnsProportions>
      <Proportion Type="Auto" />
      <Proportion Type="Auto" />
    </Grid.ColumnsProportions>
    <Grid.RowsProportions>
      <Proportion Type="Auto" />
      <Proportion Type="Auto" />
    </Grid.RowsProportions>
    <Button>
      <Label Text="Button" />
    </Button>
    <Button Grid.Column="1">
      <Label Text="Long Button" />
    </Button>
    <Button Grid.Row="1" Grid.ColumnSpan="2">
      <Label Text="Very Long Button" />
    </Button>
  </Grid>
</Project>
```
It would result in following:

![alt text](~/images/grid-layout1.png)

*Note.* There are white lines separating the cells because "ShowGridLines" is set to "True". It's a useful property to debug the grid behavior.

# Proportion Types
Proportion class contains two properties: Type(enum) and Value(float). The Type could have following values:

Name|Description
----|-----------
Auto(default)|The column/row is sized automatically based on the widget with the biggest width/height
Pixels|The column/row has fixed size in pixels specified by the Value property
Part|The column/row size is calculated by following formula: size=(Value*availableSpace)/totalParts. Where availableSpace is space left after processing Auto and Pixels columns/rows. And totalParts is sum of all columns/rows' Value where Type is Part.
Fill|The column/row takes all unused space

The following code demonstrates different proportion types:
```c#
// Create grid
var grid = new Grid
{
  ShowGridLines = true,
  ColumnSpacing = 8,
  RowSpacing = 8,
};

// Set partitioning configuration
// First column uses default proportion type(Auto)
grid.ColumnsProportions.Add(new Proportion());

// Second column width is 200 pixels
grid.ColumnsProportions.Add(new Proportion
{
  Type = ProportionType.Pixels,
  Value = 200,
});

// Third column will occupy rest of available space
grid.ColumnsProportions.Add(new Proportion
{
  Type = ProportionType.Fill,
});

// First row will take 1/3 of available height
grid.RowsProportions.Add(new Proportion
{
  Type = ProportionType.Part,
});

// Second row will take 2/3 of available height
grid.RowsProportions.Add(new Proportion
{
  Type = ProportionType.Part,
  Value = 2,
});

// Add widgets
var button = new Button
{
  Content = new Label
  {
    Text = "Button"
  }
};

grid.Widgets.Add(button);

var longButton = new Button
{
  Content = new Label
  {
    Text = "Long Button",
    HorizontalAlignment = HorizontalAlignment.Center
  },
  Width = 150
};
Grid.SetColumn(longButton, 1);
grid.Widgets.Add(longButton);

var veryLongButton = new Button
{
  Content = new Label
  {
    Text = "Very Long Button"
  },
  Left = -10,
  Top = -10,
  HorizontalAlignment = HorizontalAlignment.Right,
  VerticalAlignment = VerticalAlignment.Bottom
};
Grid.SetRow(veryLongButton, 1);
Grid.SetColumnSpan(veryLongButton, 2);
grid.Widgets.Add(veryLongButton);

var longestButton = new Button
{
  Content = new Label
  {
    Text = "The Longest Button"
  },
  HorizontalAlignment = HorizontalAlignment.Center,
  VerticalAlignment = VerticalAlignment.Center,
};
Grid.SetColumn(longestButton, 2);
Grid.SetRow(longestButton, 1);
grid.Widgets.Add(longestButton);
```

It is equivalent to [MML](MML.md):
```xml
<Project>
  <Project.ExportOptions />
  <Grid ShowGridLines="True" ColumnSpacing="8" RowSpacing="8">
    <Grid.ColumnsProportions>
      <Proportion Type="Auto" />
      <Proportion Type="Pixels" Value="200" />
      <Proportion Type="Fill" />
    </Grid.ColumnsProportions>
    <Grid.RowsProportions>
      <Proportion Type="Part" />
      <Proportion Type="Part" Value="2" />
    </Grid.RowsProportions>
    <Button>
      <Label Text="Button" />
    </Button>
    <Button Width="150" Grid.Column="1">
      <Label Text="Long Button" HorizontalAlignment="Center" />
    </Button>
    <Button Left="-10" Top="-10" HorizontalAlignment="Right" VerticalAlignment="Bottom" Grid.Row="1" Grid.ColumnSpan="2">
      <Label Text="Very Long Button" />
    </Button>
    <Button HorizontalAlignment="Center" VerticalAlignment="Center" Grid.Column="2" Grid.Row="1">
      <Label Text="The Longest Button" />
    </Button>
  </Grid>
</Project>
```
It would result in following:

![alt text](~/images/grid-layout2.png)

*Note*. We have set Left/Top/HorizontalAlignment/VerticalAlignment in the child widgets, hovewer their behavior is different than in the [Panel](basic-layout-and-panel.md) container. In Panel those properties applied relative to the whole container, while in the grid it is applied to the widget's cell.

# Default Proportions

It is possible to set default value for both horizontal and vertical proportions.
So following MML:
```xml
<Grid ColumnSpacing="8" RowSpacing="8">
  <Grid.DefaultRowProportion Type="Auto" />
</Grid>
```
would be equivalent to:
```xml
<Grid ColumnSpacing="8" RowSpacing="8">
  <Grid.RowsProportions>
    <Proportion Type="Auto" />
    <Proportion Type="Auto" />
    <Proportion Type="Auto" />
  </Grid.RowsProportions>
</Grid>
```