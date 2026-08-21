## Overview
A complete Myra UI stylesheet consists of several files:

| File | Purpose |
|---|---|
| Stylesheet definition (**.xmms**) | XML that describes the styles of all the widgets. |
| Texture atlas (**.xmat**) | XML that describes the texture regions of the atlas image. It is produced by [MyraTexturePacker](https://github.com/MyraUI/MyraTexturePacker). |
| Atlas image | A single image (BMP, TGA, PNG, JPG, GIF or PSD) that contains all the UI images. |
| Font file(s) | One or more fonts used by the UI. |

Two font formats are supported:

* **TTF/OTF/TTC** - dynamic fonts that are rasterized into glyphs at runtime.
* **AngelCode .FNT** - static bitmap fonts.

The default UI stylesheet is an example of all these files:
https://github.com/MyraUI/Myra/tree/master/src/Myra/Resources

## Stylesheet XML (.xmms) Structure
The root element of a stylesheet references the texture atlas:

```xml
<Stylesheet TextureRegionAtlas="default_ui_skin.xmat">
```

Every image referenced by a style (backgrounds, cursors, icons, etc.) is resolved by its name through the texture atlas. For example:

```xml
<TextBoxStyle Background="textfield" TextColor="White" DisabledTextColor="Gray"
              Font="default-font" Cursor="cursor" Selection="selection" />
```

This style references three images ('textfield', 'cursor' and 'selection'), all present in the atlas, plus two colors by their names ('white' and 'grey') and a font by its id ('default-font').

Properties of type [IBrush](images.md) can also be set to explicit color values:

```xml
<VerticalMenuStyle Background="button" Border="#1BA1E2" BorderThickness="1"
                   SelectionHoverBackground="button-over" SelectionBackground="button-down"
                   SpecialCharColor="red" />
```

Here the `Border` brush is explicitly set to the color "#1BA1E2".

The rest of the document is a list of style collections, one per widget type (`LabelStyles`, `TextBoxStyles`, `ButtonStyles`, and so on). Each collection contains one or more style elements:

```xml
<ButtonStyles>
  <ButtonStyle Background="button" OverBackground="button-over" PressedBackground="button-down" />
  <ButtonStyle Id="blue" Background="button-blue" OverBackground="button-blue-down" PressedBackground="button-blue-down" />
</ButtonStyles>
```

A style without an `Id` attribute is the *default* style of the widget type. Styles with an `Id` attribute are *named* styles that can be applied explicitly, e.g. `button.StyleName = "blue"`. See [Style Variants](style-variants.md) for details.

To find what properties can be set for each style, see the following code:
https://github.com/MyraUI/Myra/tree/master/src/Myra/Graphics2D/UI/Styles

Every widget style corresponds to a style class and is loaded through reflection. For example, the `VerticalMenuStyle` is defined in [MenuStyle.cs](https://github.com/MyraUI/Myra/blob/master/src/Myra/Graphics2D/UI/Styles/MenuStyle.cs). `MenuStyle` inherits from [WidgetStyle](https://github.com/MyraUI/Myra/blob/master/src/Myra/Graphics2D/UI/Styles/WidgetStyle.cs), so `VerticalMenuStyle` has the properties of both classes.

## Fonts and the UsedSpace Attribute
This is how a stylesheet declares and resolves fonts:

```xml
<Fonts UsedSpace="0, 0, 1024, 160">
  <Font Id="default-font" File="Inter-Regular.ttf" Size="20"/>
</Fonts>
```

Each `Font` element declares a font by its id (`Id`), file (`File`) and, for dynamic fonts, pixel size (`Size`).

`UsedSpace` declares a rectangle (`X, Y, Width, Height`) inside the texture atlas that is already occupied by the UI images. FontStashSharp will use the atlas texture to store the font glyphs, but it will omit that rectangle, since it is reserved for the UI images. This way both the UI images and the font glyphs live in the same texture, and the renderer doesn't need to switch between textures.

The most important implications:

* **The rectangle is where the UI images are packed.** When packing the images with MyraTexturePacker, they are placed into the `UsedSpace` area, and the font glyphs are rendered into the remaining (unused) area of the atlas at runtime.
* **`UsedSpace` is optional.** If it is not declared, the font system creates a separate texture to store the glyphs. The renderer then has to switch between two textures (the atlas and the glyph texture) every frame, which causes texture swaps. Therefore, it is recommended to always use `UsedSpace`.
* `UsedSpace` is only relevant for dynamic (TTF/OTF) fonts. A static .FNT font references its glyphs directly and doesn't need `UsedSpace`. Depending on how the .FNT references its glyph image:
  * It can reference a texture region in the atlas (e.g. `file="ui_stylesheet.xmat:commodore-64"`). The glyphs then live in the atlas texture, and no new texture is created.
  * It can reference a separate image file (e.g. `file="commodore-64.png"`). In this case a new texture is created for the font.

See FontStashSharp's [How To Use Existing Texture As Font Glyphs Atlas](https://fontstashsharp.github.io/FontStashSharp/docs/using-existing-texture-as-font-glyphs-atlas.html) for more details.

## Editing a Stylesheet in MyraPad
MyraPad can edit the stylesheet of a project visually.

1. Load a MyraPad project (**\*.xmmp**) in [MyraPad](MyraPad.md).
2. Select **File/Load Stylesheet** and pick the stylesheet (**\*.xmms**) for the project.
3. A **Stylesheet** tab appears in the left panel. It shows the stylesheet as a tree: widget type, then each style (default and named).
4. Select a style in the tree to edit its properties in the property grid.
5. The stylesheet path is stored in the project XML as the `StylesheetPath` attribute of the root `<Project>` element:
   ```xml
   <Project StylesheetPath="ui_stylesheet.xmms">
   ```

  _**Note**. The tree shows only the widget types and styles that are already defined in the stylesheet. If you want to add a new widget type or a new (named) style for an existing widget, you need to add it to the .xmms manually with a text editor, and then reload the stylesheet (File/Reload or Ctrl+R).**_

When the project is saved (**File/Save** or **Ctrl+S**), both the project file and the stylesheet are written to disk. It is also possible to save only the stylesheet via **File/Save Stylesheet**.

If the stylesheet is edited with an external editor, use **File/Reload** (**Ctrl+R**) to apply the changes.

**File/Reset Stylesheet** removes the link to the custom stylesheet and reverts the project to the default stylesheet.

## Loading a Stylesheet
Actual stylesheet loading is done through [XNAssets](https://github.com/rds1983/XNAssets).
Example code:

```c#
protected override void LoadContent()
{
    base.LoadContent();

    MyraEnvironment.Game = this;

    // Create asset manager
    var assetManager = AssetManager.CreateResourceAssetManager(typeof(CustomUIStylesheetGame).Assembly, "Resources");

    // Load stylesheet
    Stylesheet.Current = assetManager.LoadStylesheet("ui_stylesheet.xmms");
    ...
}

```

  _**Note**. The default style is applied to a widget at the moment of its creation. Therefore, all changes to **Stylesheet.Current** should be done before the UI is created._

## Myra.Samples.CustomUIStylesheet
[Myra.Samples.CustomUIStylesheet](https://github.com/MyraUI/Myra/tree/master/samples/Myra.Samples.CustomUIStylesheet) is another example of a full Myra stylesheet. Its stylesheet files are stored as resources:
https://github.com/MyraUI/Myra/tree/master/samples/Myra.Samples.CustomUIStylesheet/Resources

The main difference from the default stylesheet is that the custom stylesheet uses a static font in AngelCode .FNT format:

```xml
<Fonts>
  <Font Id="commodore-64" File="commodore-64.fnt"/>
</Fonts>
```

Note that there is no `UsedSpace` attribute here - the .FNT font references its glyphs directly.

It also uses a single [underlying image](https://github.com/MyraUI/Myra/blob/master/samples/Myra.Samples.CustomUIStylesheet/Resources/ui_stylesheet_atlas.png) to store both the texture atlas images and the font glyphs. This is a good solution performance-wise, since the renderer doesn't need to switch between textures.

If you view [commodore-64.fnt](https://github.com/MyraUI/Myra/blob/master/samples/Myra.Samples.CustomUIStylesheet/Resources/commodore-64.fnt), you can see how the .FNT references the texture region with id 'commodore-64' in ui_stylesheet.xmat as the image containing the character glyphs:

```
page id=0 file="ui_stylesheet.xmat:commodore-64"
```