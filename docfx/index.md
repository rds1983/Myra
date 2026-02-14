1. Create MonoGame/FNA project.
2. Reference Myra for MonoGame from nuget: https://www.nuget.org/packages/Myra

   See [this](docs/adding-reference-to-fna.md) on how reference Myra for a FNA project.
3. Add the following using statements:
```c#
  using Myra;
  using Myra.Graphics2D.UI;
```
4. Add following code to the `Game1` constructor to make the mouse cursor visible:
```c#
  IsMouseVisible = true;
```
5. Add following field to the Game class:
```c#
  private Desktop _desktop;
```
6. Add following code in the LoadContent method, which will create 2x2 grid and populate it with some widgets:
```c# 
  MyraEnvironment.Game = this;

  var grid = new Grid
  {
	RowSpacing = 8,
	ColumnSpacing = 8
  };

  grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
  grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
  grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
  grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

  var helloWorld = new Label
  {
    Id = "label",
    Text = "Hello, World!"
  };
  grid.Widgets.Add(helloWorld);

  // ComboBox
  var combo = new ComboView();
  Grid.SetColumn(combo, 1);
  Grid.SetRow(combo, 0);

  combo.Widgets.Add(new Label{Text = "Red", TextColor = Color.Red});
  combo.Widgets.Add(new Label{Text = "Green", TextColor = Color.Green});
  combo.Widgets.Add(new Label{Text = "Blue", TextColor = Color.Blue});

  grid.Widgets.Add(combo);

  // Button
  var button = new Button
  {
    Content = new Label
    {
      Text = "Show"
    }
  };
  Grid.SetColumn(button, 0);
  Grid.SetRow(button, 1);

  button.Click += (s, a) =>
  {
	var messageBox = Dialog.CreateMessageBox("Message", "Some message!");
	messageBox.ShowModal(_desktop);
  };

  grid.Widgets.Add(button);

  // Spin button
  var spinButton = new SpinButton
  {
	Width = 100,
	Nullable = true
  };
  Grid.SetColumn(spinButton, 1);
  Grid.SetRow(spinButton, 1);

  grid.Widgets.Add(spinButton);

  // Add it to the desktop
  _desktop = new Desktop();
  _desktop.Root = grid;
```
7. Add following code to the Draw method:
```c#
  GraphicsDevice.Clear(Color.Black);
  _desktop.Render();
```

It would result in following:

![alt text](~/images/getting-started.gif)

