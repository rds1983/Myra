## Overview
Myra is UI Library build on top of [MonoGame](http://www.monogame.net/) and [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended).  
It has following functionality:
* **UI Widgets.** Button, CheckBox, ComboBox, Grid, Image, Menu, ProgressBar, ScrollPane, SplitPane(with arbitrary number of splitters), Slider, TextBlock, TextField, SpinButton, Tree and Window. Almost all of it are demonstrated on the following screenshot: ![](/Screenshots/GridSample.png)
* **UI Skinning.** The awesome default skin had been borrowed from the [VisUI](https://github.com/kotcrab/vis-editor/wiki/VisUI) project. The default skin could be replaced with a custom skin loaded from the JSON.
* **[UI Editor](https://github.com/rds1983/Myra/wiki/Using-UI-Editor).** [Myra's binary distribution](https://github.com/rds1983/Myra/releases) contains standalone UI Editor application: ![](/Screenshots/UIEditor.png)

## Quick Start
1. The easiest way of adding Myra to the MonoGame project is through Nuget: `install-package Myra`. Alternative way is to download the latest binary release: https://github.com/rds1983/Myra/releases, install it and reference Myra.dll manually.
2. Add following field in the Game class:
  ```c#
  private Desktop _host;
  ```
3. Add following code in the LoadContent method, which will create 2x2 grid and populate it with some widgets:
  ```c# 
  MyraEnvironment.Game = this;

  var grid = new Grid
  {
	RowSpacing = 8,
	ColumnSpacing = 8
  };

  grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
  grid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
  grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
  grid.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

  // TextBlock
  var helloWorld = new TextBlock
  {
	Text = "Hello, World!"
  };
  grid.Widgets.Add(helloWorld);

  // ComboBox
  var combo = new ComboBox
  {
	GridPositionX = 1,
	GridPositionY = 0
  };

  combo.Items.Add(new ListItem("Red", Color.Red));
  combo.Items.Add(new ListItem("Green", Color.Green));
  combo.Items.Add(new ListItem("Blue", Color.Blue));
  grid.Widgets.Add(combo);

  // Button
  var button = new Button
  {
	GridPositionX = 0,
	GridPositionY = 1,
	Text = "Show"
  };

  button.Down += (s, a) =>
  {
	var messageBox = Dialog.CreateMessageBox("Message", "Some message!");
	messageBox.ShowModal(_host);
  };

  grid.Widgets.Add(button);

  // Spin button
  var spinButton = new SpinButton
  {
	GridPositionX = 1,
	GridPositionY = 1,
	WidthHint = 100,
	Nullable = true
  };
  grid.Widgets.Add(spinButton);

  // Add it to the desktop
  _host = new Desktop();
  _host.Widgets.Add(grid);
  ```
4. Add following code to the Draw method:
  ```c#
  _host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
  _host.Render();
  ```
 
It would result in following screenshot(assuming the background is black):
![](/Screenshots/QuickStart.png)

## Credits
* [MonoGame](http://www.monogame.net/)
* [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended)
* [VisUI](https://github.com/kotcrab/vis-editor/wiki/VisUI)
* [LibGDX](http://libgdx.badlogicgames.com/)
* [Cyotek.Drawing.BitmapFont](https://github.com/cyotek/Cyotek.Drawing.BitmapFont)