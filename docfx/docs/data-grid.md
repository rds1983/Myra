## Overview
DataGrid displays data in a tabular format with resizable columns, vertical scrolling, and row selection.

## Columns
Columns are defined by creating an array of [DataGridColumnBase](~/api/Myra.Graphics2D.UI.Data.DataGridColumnBase.yml) subclasses. The base class is abstract. The built-in [DataGridTextColumn](~/api/Myra.Graphics2D.UI.Data.DataGridTextColumn.yml) renders cells as text.

Property|Description
--------|-----------
`Header`|Text displayed in the column header
`Property`|Name of the public field or property on the data object to display
`Width`|Initial width in pixels

`DataGridTextColumn` adds:

Property|Description
--------|-----------
`Format`|Optional format string applied to the value (e.g. `"{0:C2}"`). When `null`, `ToString()` is used.

Both types provide convenience constructors:

```c#
// Using object initializer
var col = new DataGridTextColumn { Header = "Name", Property = "Name", Width = 120 };

// Using constructor with property, header, width, and optional format
var col2 = new DataGridTextColumn("Price", "Price", 100, "{0:C2}");

// Using constructor with property only (uses property name as header)
var col3 = new DataGridTextColumn("Name", 120);
```

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

## Custom Columns
Subclass [DataGridColumnBase](~/api/Myra.Graphics2D.UI.Data.DataGridColumnBase.yml) and override [CreateWidget](~/api/Myra.Graphics2D.UI.Data.DataGridColumnBase.yml#Myra_Graphics2D_UI_Data_DataGridColumnBase_CreateWidget_System_Object_) to control how each cell is rendered:

```c#
public class DataGridBoolColumn : DataGridColumnBase
{
    public override Widget CreateWidget(object value)
    {
        if (value == null)
            return null;

        return new CheckButton
        {
            IsChecked = (bool)value,
            Enabled = false
        };
    }
}
```

## Binding Data
Assign a collection to the [Data](~/api/Myra.Graphics2D.UI.Data.DataGrid.yml#Myra_Graphics2D_UI_Data_DataGrid_Data) property. Each object becomes a row and its property values are resolved by name via reflection.

```c#
var dataGrid = new DataGrid
{
    Columns = new DataGridColumnBase[]
    {
        new DataGridTextColumn { Header = "First Name", Property = "FirstName", Width = 120 },
        new DataGridTextColumn { Header = "Last Name", Property = "LastName", Width = 120 },
        new DataGridTextColumn { Header = "Company", Property = "Company", Width = 200 },
    },
    Data = GetCustomers() // IList of objects
};
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

## Selection
Set [GridSelectionMode](~/api/Myra.Graphics2D.UI.Data.DataGrid.yml#Myra_Graphics2D_UI_Data_DataGrid_GridSelectionMode) to control selection behavior.

Value|Description
-----|-----------
`None`|Selection disabled
`Row`|Entire rows can be selected
`Column`|Entire columns can be selected
`Cell`|Individual cells can be selected

Property|Description
--------|-----------
`SelectedRowIndex`|Zero-based index of the selected data row, or `null` when no row is selected
`SelectedItem`|The data object of the selected row, or `null` when no row is selected
`SelectedIndexChanged`|Event raised when the selected row index changes
`HoverIndexCanBeNull`|Whether the hover highlight clears when the pointer leaves the grid (default `true`)
`CanSelectNothing`|Whether clicking an already-selected row deselects it (default `false`)

## Sorting
When `SortableHeaders` is `true` (default), clicking a header cell sorts the data by that column. Clicking the same header again reverses the sort direction. Set `SortableHeaders = false` to disable interactive sorting while still allowing programmatic sorting.

Property|Description
--------|-----------
`SortableHeaders`|Whether header cells are clickable and trigger sorting (default `true`)
`SortColumn`|Zero-based index of the column being sorted, or `null` when no sort is applied
`SortDirection`|`Ascending` or `Descending` (default `Ascending`)
`SortAscendingImage`|Image displayed next to the header when sorted ascending
`SortDescendingImage`|Image displayed next to the header when sorted descending

Sorting can also be applied programmatically:

```c#
dataGrid.SortColumn = 2;           // Sort by the third column
dataGrid.SortDirection = ListSortDirection.Descending;
```

## Header Row
When at least one column has a non-empty `Header`, the first row is treated as a header. It is excluded from hover highlighting and selection and is always visible regardless of the scroll position. Header cells are rendered as buttons.

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
`SortAscendingImage`|Image for the ascending sort indicator in column headers
`SortDescendingImage`|Image for the descending sort indicator in column headers

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
    },
    Data = customers
};

_desktop = new Desktop { Root = dataGrid };
```

Full sample is available here: https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.DataGrid
