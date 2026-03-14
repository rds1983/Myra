### Window
Window can be dragged across the desktop and closed either by clicking 'x' or pressing the Escape key. It has Closed event that fires when the window is closed.

I.e. following code creates and show simple window with button inside:
```c#
Window window = new Window
{
    Title = "Simple Window"
};

Button button = new Button
{
    Content = new Label
    {
        Text = "Push Me!"
    },
    HorizontalAlignment = HorizontalAlignment.Center
};

window.Content = button;

window.Closed += (s, a) =>
{
    // Called when window is closed
};

window.ShowModal(_desktop);
```
It is equivalent to the [MML](MML.md):
```xml
<Project>
  <Project.ExportOptions Namespace="Test" Class="Test" OutputPath="D:\Temp" />
  <Window Title="Simple Window" Left="678" Top="298">
    <Button HorizontalAlignment="Center">
      <Label Text="Push Me!" />
    </Button>
  </Window>
</Project>
```
And would result in following:

![alt text](~/images/windows-and-dialogs1.png)

### Dialog
Dialog is enhanced version of Window that also have "Ok" and "Cancel" button. It has Result(bool) property that indicates whether "Ok"(it also fires if Enter key is down) or "Cancel" was clicked.
Following code creates simple "Enter Your Name" dialog:
```c#
Dialog dialog = new Dialog
{
    Title = "Enter Your Name"
};

var stackPanel = new HorizontalStackPanel
{
    Spacing = 8
};

var label1 = new Label
{
    Text = "Name:"
};
stackPanel.Widgets.Add(label1);

var textBox1 = new TextBox();
StackPanel.SetProportionType(textBox1, ProportionType.Fill);
stackPanel.Widgets.Add(textBox1);

dialog.Content = stackPanel;

dialog.Closed += (s, a) => {
    if (!dialog.Result)
    {
        // Dialog was either closed or "Cancel" clicked
        return;
    }

    // "Ok" was clicked or Enter key pressed
    // ...
};

dialog.ShowModal(_desktop);
```
It is equivalent to the following [MML](MML.md):
```xml
<Project>
  <Project.ExportOptions />
  <Dialog Title="Enter Your Name" Left="387" Top="193">
    <HorizontalStackPanel Spacing="8">
      <Label Text="Name:" />
      <TextBox StackPanel.ProportionType="Fill" />
    </HorizontalStackPanel>
  </Dialog>
</Project>
```
And would result in following:

![alt text](~/images/windows-and-dialogs2.png)

Also Dialog contains static method CreateMessageBox that creates dialog with Content set to Label.
It could be used following way:
```c#
    Dialog dialog = Dialog.CreateMessageBox("Quit?", "Would you like to quit?");

    dialog.Closed += (s, a) =>
    {
        if (!dialog.Result)
        {
            // Escape or "Cancel"
            return;
        }

        // Enter or "Ok"
        Exit();
    };

    dialog.ShowModal(_desktop);
```

### FileDialog
FileDialog operates with files. It could work in one of three modes(chosen mode should be passed to the constructor): OpenFile, SaveFile, ChooseFolder.

FileDialog has Filter(string) property, which - if not null/empty - is passed to the [Directory.EnumerateFiles](https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.enumeratefiles?view=netframework-4.8).

Following code opens FileDialog in OpenFile mode in the folder "D:\Temp" with the filter set to XML files:
```c#
    FileDialog dialog = new FileDialog(FileDialogMode.OpenFile)
    {
        Filter = "*.xml",
        Folder = @"D:\Temp"
    };

    dialog.Closed += (s, a) =>
    {
        if (!dialog.Result)
        {
            // "Cancel" or Escape
            return;
        }

        // "Ok" or Enter
        // ...
    };

    dialog.ShowModal(_desktop);
```
It would result in following:

![alt text](~/images/windows-and-dialogs3.png)

### ColorPickerDialog
ColorPickerDialog helps to choose a color.

Following code opens it and sets initial color to Red:
```c#
    ColorPickerDialog dialog = new ColorPickerDialog
    {
        Color = Color.Red
    };

    dialog.Closed += (s, a) =>
    {
        if (!dialog.Result)
        {
            // "Cancel" or Escape
            return;
        }

        // "Ok" or Enter
        // ...
    };

    dialog.ShowModal(_desktop);
```
It would result in following:

![alt text](~/images/windows-and-dialogs4.png)

### DebugOptionsWindow
DebugOptionsWindow helps to turn on and off various Myra debugging options(they also could be set in the code through MyraEnvironment static class).

Following code creates DebugOptionsWindow:
```c#
    DebugOptionsWindow debugOptions = new DebugOptionsWindow();
    debugOptions.ShowModal(_desktop);
```
It would result in following:

![alt text](~/images/windows-and-dialogs5.png)
