By default, Myra translates keys to characters in the TextBox widget, which essentially limits accepted characters to ASCII only.

Add the following code to Game.LoadContent to make Myra accept any characters by utilizing MonoGame TextInput:
```c#
    // Inform Myra that external text input is available
    // So it stops translating Keys to chars
    Desktop.HasExternalTextInput = true;

    // Provide that text input
    Window.TextInput += (s, a) =>
    {
        Desktop.OnChar(a.Character);
    };
```