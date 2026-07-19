## Overview
DataGrid displays data in a tabular format with resizable columns, vertical scrolling, and row selection.

## Columns
Columns are defined by creating an array of [DataGridColumnBase](~/api/Myra.Graphics2D.UI.Data.DataGridColumnBase.yml) subclasses. Currently the only concrete type is [DataGridTextColumn](~/api/Myra.Graphics2D.UI.Data.DataGridTextColumn.yml), which renders cells as text.

Property|Description
--------|-----------
`Header`|Text displayed in the column header
`Property`|Name of the public field or property on the data object to display
`Width`|Initial width in pixels

## Index Column
DataGrid optionally displays a row-number index column as the first column (enabled by default). Control it with:

Property|Description
--------|-----------
`HasIndexColumn`|Set to `false` to hide the index column
`IndexColumnWidth`|Width of the index column in pixels (default `50`)

```c#
var dataGrid = new DataGrid
{
    HasIndexColumn = false
};
```

## Binding Data
Call [Build](~/api/Myra.Graphics2D.UI.Data.DataGrid.yml#Myra_Graphics2D_UI_Data_DataGrid_Build_System_Collections_IEnumerable) with an `IEnumerable` of objects. Each object becomes a row and its properties are resolved by name via reflection.

```c#
var dataGrid = new DataGrid
{
    Columns = new DataGridColumnBase[]
    {
        new DataGridTextColumn { Header = "First Name", Property = "FirstName", Width = 120 },
        new DataGridTextColumn { Header = "Last Name", Property = "LastName", Width = 120 },
        new DataGridTextColumn { Header = "Company", Property = "Company", Width = 200 },
    }
};

var customers = GetCustomers(); // List<Customer>
dataGrid.Build(customers);
```

*Note.* `Property` must match a public instance field or property name on the object exactly.

## Column Resizing
Columns can be resized interactively by dragging the boundary between two header cells. The cursor changes to a horizontal resize indicator (`SizeWE`) when hovering over a boundary. The minimum column width is 20 pixels.

## Scrolling
DataGrid provides a vertical scrollbar when the data exceeds the visible area. Mouse wheel and touch-drag scrolling are supported.

Property|Description
--------|-----------
`VerticalScrollBackground`|Image for the scrollbar track
`VerticalScrollKnob`|Image for the scrollbar thumb
`ScrollMultiplier`|Rows scrolled per mouse wheel tick (default `10`)

## Row Selection
Set [GridSelectionMode](~/api/Myra.Graphics2D.UI.Data.DataGrid.yml#Myra_Graphics2D_UI_Data_DataGrid_GridSelectionMode) to control selection behavior.

Value|Description
-----|-----------
`None`|Selection disabled
`Row`|Entire rows can be selected
`Column`|Entire columns can be selected
`Cell`|Individual cells can be selected

The selected row index is exposed through the internal grid's `SelectedRowIndex` property.

Behavior property|Description
-----------------|-----------
`HoverIndexCanBeNull`|Whether the hover highlight clears when the pointer leaves the grid (default `true`)
`CanSelectNothing`|Whether clicking an already-selected row deselects it (default `false`)

## Header Row
When at least one column has a non-empty `Header`, the first row is treated as a header. It is excluded from hover highlighting and selection and is always visible regardless of the scroll position.

## Styling
DataGrid look and feel is controlled through a [DataGridStyle](~/api/Myra.Graphics2D.UI.Styles.DataGridStyle.yml), which inherits from [GridStyle](~/api/Myra.Graphics2D.UI.Styles.GridStyle.yml), assigned via the active [Stylesheet](~/api/Myra.Graphics2D.UI.Styles.Stylesheet.yml).

Property|Description
--------|-----------
`ShowGridLines`|Whether grid lines are drawn between cells
`GridLinesColor`|Color of the grid lines
`ColumnSpacing`|Horizontal padding inside each cell
`RowSpacing`|Vertical padding inside each cell
`SelectionBackground`|Brush used for the selected row/column/cell
`SelectionHoverBackground`|Brush used for the hovered row/column/cell
`VerticalScrollBackground`|Image for the scrollbar track
`VerticalScrollKnob`|Image for the scrollbar thumb

Apply a custom style in code:

```c#
var style = new DataGridStyle
{
    ShowGridLines = true,
    GridLinesColor = Color.Gray,
    SelectionHoverBackground = new SolidBrush(Color.FromArgb(40, Color.White))
};

var dataGrid = new DataGrid
{
    GridStyle = style
};
```

## Example
The following example displays a list of customer records:

```c#
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Data;

public class Customer
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
}

// Build data
var customers = new List<Customer>
{
    new Customer { FirstName = "Alice", LastName = "Smith", Company = "Acme", City = "Portland", Country = "US" },
    new Customer { FirstName = "Bob", LastName = "Jones", Company = "Globex", City = "Seattle", Country = "US" },
};

// Create the DataGrid
var dataGrid = new DataGrid
{
    Columns = new DataGridColumnBase[]
    {
        new DataGridTextColumn { Header = "First Name", Property = "FirstName", Width = 120 },
        new DataGridTextColumn { Header = "Last Name", Property = "LastName", Width = 120 },
        new DataGridTextColumn { Header = "Company", Property = "Company", Width = 200 },
        new DataGridTextColumn { Header = "City", Property = "City", Width = 150 },
        new DataGridTextColumn { Header = "Country", Property = "Country", Width = 100 },
    }
};

dataGrid.Build(customers);

_desktop = new Desktop { Root = dataGrid };
```

Full sample is available here: https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.DataGrid
