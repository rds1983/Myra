Myra uses [FontStashSharp](https://github.com/FontStashSharp/FontStashSharp) for the text rendering.

Sample code for setting font:
```c#
byte[] ttfData = File.ReadAllBytes("DroidSans.ttf");

FontSystem fontSystem = new FontSystem();
fontSystem.AddFont(ttfData);
_label1.Font = fontSystem.GetFont(32);

```
