## Overview
It's possible to mix styles from multiple stylesheets. This is useful when you have a custom stylesheet that doesn't define styles for every widget type.

## The Problem
Let's say you've made a custom stylesheet, but omitted some widgets (such as FileDialog) that aren't going to be used in the game.

However, you still want to create a FileDialog in, say, developer mode.

If you create it after setting **Stylesheet.Current** to your custom stylesheet, it'll throw an exception:

```c#
Stylesheet.Current = assetManager.LoadStylesheet("ui_stylesheet.xmms");

// Throws: "Stylesheet doesnt define default style for FileDialogStyle."
var dialog = new FileDialog(FileDialogMode.OpenFile);
```

## Creating Widgets With an Explicit Stylesheet
The solution is to pass the stylesheet that defines the widget's style explicitly.

A FileDialog created with the default stylesheet works even if the current stylesheet doesn't define a FileDialog style, since the default stylesheet defines styles for all widget types:

```c#
var dialog = new FileDialog(FileDialogMode.OpenFile, DefaultAssets.DefaultStylesheet);
```

The same approach applies to any widget, not only FileDialog. For example, ColorPickerDialog can be created the same way.

## Myra.Samples.CustomUIStylesheet
[Myra.Samples.CustomUIStylesheet](https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.CustomUIStylesheet) is an example of mixing stylesheets.

Its custom stylesheet ([ui_stylesheet.xmms](https://github.com/rds1983/Myra/blob/master/samples/Myra.Samples.CustomUIStylesheet/Resources/ui_stylesheet.xmms)) doesn't define styles for dialogs, such as FileDialog and ColorPickerDialog. However, the sample still creates them using the default stylesheet:

```c#
var fileDialog = new FileDialog(FileDialogMode.OpenFile, DefaultAssets.DefaultStylesheet);
```

and

```c#
var colorWindow = new ColorPickerDialog(DefaultAssets.DefaultStylesheet);
```