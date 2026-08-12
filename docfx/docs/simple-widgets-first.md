## Label
Label is the most basic widget that displays text. It supports [Rich Text](rich-text.md).

Example code:
```c#
var label = new Label();
label.Text = "StbImageSharp";
label.Margin = new Thickness(8);
label.Border = new SolidBrush("#808000FF");
label.BorderThickness = new Thickness(8);
label.Padding = new Thickness(16);
label.Background = new SolidBrush("#008000FF");
```
It is equivalent to the following MML:
```xml
<Label Text="StbImageSharp" Margin="8" Border="#808000FF" BorderThickness="8" Padding="16" Background="#008000FF" />
```
It would display following:

![alt text](~/images/simple-widgets-first1.png)

## Button

Button can have any content. The following code creates a Button with a Label:
```c#
Button button = new Button
{
	Content = new Label
	{
		Text = "Test"
	}
};
```

It is equivalent to the following MML:
```xml
<Button>
  <Label Text="Test" />
</Button>
```

It would display following:

![alt text](~/images/simple-widgets-first2.png)

More complicated example of sized button with aligned text:
```c#
Button button = new Button
{
	Width = 100,
	Height = 30,
	Content = new Label
	{
		HorizontalAlignment = HorizontalAlignment.Center,
		VerticalAlignment = VerticalAlignment.Center,
		Text = "Test"
	}
};
```

Corresponding MML:
```xml
<Button Width="100" Height="30" HorizontalAlignment="Center" VerticalAlignment="Center" >
	<Label Text="Test" HorizontalAlignment="Center" VerticalAlignment="Center" />
</Button>
```

Image:

![alt text](~/images/simple-widgets-first3.png)

Finally example of Button with image and text:
```c#
var image = new Image();
image.Renderable = new TextureRegion(Texture2D.FromFile(GraphicsDevice, "tree1.png"));

var label = new Label();
label.Text = "Test";
label.VerticalAlignment = VerticalAlignment.Center;

var horizontalStackPanel = new HorizontalStackPanel();
horizontalStackPanel.Spacing = 8;
horizontalStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
horizontalStackPanel.VerticalAlignment = VerticalAlignment.Center;
horizontalStackPanel.Widgets.Add(image);
horizontalStackPanel.Widgets.Add(label);

var button = new Button();
button.Width = 100;
button.Height = 40;
button.HorizontalAlignment = HorizontalAlignment.Center;
button.VerticalAlignment = VerticalAlignment.Center;
button.Content = horizontalStackPanel;
```

MML:
```xml
<Button Width="100" Height="40" HorizontalAlignment="Center" VerticalAlignment="Center">
	<HorizontalStackPanel Spacing="8" HorizontalAlignment="Center" VerticalAlignment="Center">
		<Image Renderable="tree1.png" />
		<Label Text="Test" VerticalAlignment="Center" />
	</HorizontalStackPanel>
</Button>
```

Image:

![alt text](~/images/simple-widgets-first4.png)

Button has property IsPressed that determines whether it is pressed.

Button has following events related to mouse clicks and touches:

Name|Description
----|-----------
TouchDown|Fired when the mouse or touch is down over the button
TouchUp|Fired when the mouse or touch is released over the button
Click|Fired when the mouse or touch is down and then released over the button
IsPressedChanged|Fired when IsPressed changes its value

## ToggleButton
ToggleButton has similar API as Button, but different behavior.
If it is touched/mouse clicked, then it becomes pressed. And another touch/mouse click is required to return the unpressed state.

## CheckButton
CheckButton has similar API and behavior as ToggleButton, but different appearance.

It has check image rendered next to the content. It's possible to control the position(to the left/to the right) of the check image and the spacing, through the properties CheckPosition and CheckContentSpacing.

## RadioButton
RadioButton also has the check image, however its behavior is different.
Only one radio button can be pressed within one container.
