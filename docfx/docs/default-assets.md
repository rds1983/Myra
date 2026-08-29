# DefaultAssets

`DefaultAssets` is a static class that provides access to the default assets included with Myra
All members are static and can be accessed as `DefaultAssets.<MemberName>`.

The class is located here: https://github.com/rds1983/Myra/blob/master/src/Myra/DefaultAssets.cs

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `DefaultStylesheet` | `Stylesheet` | Gets the default UI stylesheet (`default_ui_skin.xmms`) at normal scale. |
| `DefaultStylesheet2X` | `Stylesheet` | Gets the default UI stylesheet (`default_ui_skin_2x.xmms`) at 2x scale. |
| `DebugFont` | `SpriteFontBase` | Gets the font used for rendering debug information, such as the widget info overlay. |
| `WhiteRegion` | `TextureRegion` | Gets a default white texture region used for placeholder graphics and solid-color fills. |
