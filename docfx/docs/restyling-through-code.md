Restyling through code is quite easy. There's a static object **Stylesheet.Current** that contains all default widget styles.  
For example, the following code will make all Labels green:
```c#
  Stylesheet.Current.LabelStyle.TextColor = Color.Green;
```

  **Note:** The default style is applied to the widget at the moment of creation. Therefore, all changes 
to **Stylesheet.Current** should be made before the UI is created.









