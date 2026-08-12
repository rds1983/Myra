## Image
Image displays an image.

Example code is here: [images.md#textureregion](images.md#textureregion)

Image has parameter ResizeMode that determines how the image is resized(Stretch/KeepAspectRatio).

## Separator
There are two types of separators: HorizontalSeparator and VerticalSeparator.

A separator is basically a separator image.

Example usage in C#:
```c#
var label1 = new Label();
label1.Text = "Test";

var verticalSeparator1 = new VerticalSeparator();

var label2 = new Label();
label2.Text = "Test2";

var label3 = new Label();
label3.Text = "Test3";

var verticalSeparator2 = new VerticalSeparator();

var label4 = new Label();
label4.Text = "Test4";

var horizontalStackPanel1 = new HorizontalStackPanel();
horizontalStackPanel1.Spacing = 8;
horizontalStackPanel1.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;
horizontalStackPanel1.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Center;
horizontalStackPanel1.Widgets.Add(label1);
horizontalStackPanel1.Widgets.Add(verticalSeparator1);
horizontalStackPanel1.Widgets.Add(label2);
horizontalStackPanel1.Widgets.Add(label3);
horizontalStackPanel1.Widgets.Add(verticalSeparator2);
horizontalStackPanel1.Widgets.Add(label4);
```

Which is equivalent to the MML:
```xml
<HorizontalStackPanel Spacing="8" HorizontalAlignment="Center" VerticalAlignment="Center">
	<Label Text="Test" />
	<VerticalSeparator />
	<Label Text="Test2" />
	<Label Text="Test3" />
	<VerticalSeparator />
	<Label Text="Test4" />
</HorizontalStackPanel>
```

It'll display following:

![alt text](~/images/simple-widgets-second.png)

## TextBox
TextBox is a widget to edit text.

It has following features:

Feature|Related Properties
-------|------------------
Hint text(aka text that is shown when TextBox is empty)|HintText
Multiline editing|Multiline
Text Wrapping|Wrap
Undo/Redo|
Password mode|PasswordField
Selection|SelectStart, SelectEnd

TextBox has following events:

Name|Description
----|-----------
ValueChanging|Fired when the value is about to be changed. Set Cancel to true if you want to cancel the change
TextChanged|Fired every time when the text had been changed
TextChangedByUser|Fired every time when the text had been changed by user (isn't fired if the Text had been changed through the code)
CursorPositionChanged|Fired when the cursor changes its position