## Overview
A complete UI stylesheet consists of the following essential files: a stylesheet file in XML format (.xmms), a texture atlas file in [MyraTexturePacker](https://github.com/rds1983/MyraTexturePacker) XML format (.xmat), font(s) in TTF/OTF/TTC or AngelCode FNT format, and a texture atlas image in BMP, TGA, PNG, JPG, GIF, or PSD format.

Default UI stylesheet is example of all those files: https://github.com/rds1983/Myra/tree/master/src/Myra/Resources

Below is the detailed explanation of the [default_ui_skin.xmms](https://github.com/rds1983/Myra/blob/master/src/Myra/Resources/default_ui_skin.xmms) contents.

## Images
Notice how the stylesheet explicitly references [texture atlas](images.md#textureregionatlas) in the following string:
```xml
  <Stylesheet TextureRegionAtlas="default_ui_skin.xmat">
```

Now if any style references an image, it is resolved through the texture atlas.

For example:
```xml
    <TextBoxStyle Background="textfield" TextColor="white" DisabledTextColor="grey" Font="default-font" Cursor="cursor" Selection="selection" />
``` 
This style references 3 images: 'textfield', 'cursor', and 'selection'. All of them are present in the texture atlas.
It also references two colors by their names ('white' and 'grey').

It is possible to set style properties of type [IBrush](images.md) to explicit color values.

I.e. see VerticalMenuStyle declaration:
```
    <VerticalMenuStyle Background="button" Border="#1BA1E2" BorderThickness="1"
                       SelectionHoverBackground="button-over" SelectionBackground="button-down"
                       SpecialCharColor="red">
```
The VerticalMenuStyle Border brush is explicitly set to the color "#1BA1E2".

To find what properties can be set for each style, see the following code: https://github.com/rds1983/Myra/tree/master/src/Myra/Graphics2D/UI/Styles

Every widget style corresponds to a style class and is loaded through reflection.

For example, the VerticalMenuStyle is defined in [MenuStyle.cs](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/UI/Styles/MenuStyle.cs). MenuStyle inherits from [WidgetStyle](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/UI/Styles/WidgetStyle.cs). Therefore, VerticalMenuStyle has properties from both of these classes.

## Fonts
This is how stylesheet references and resolves fonts:
```xml
  <Fonts UsedSpace="0, 0, 1024, 160">
    <Font Id="default-font" File="Inter-Regular.ttf" Size="20"/>
  </Fonts>
```
`UsedSpace` declares a rectangle in the texture atlas that stores all UI images. The font system will use the texture atlas to store the font glyphs, omitting that rectangle.

It's important to note that the `UsedSpace` property is optional. If it is not declared, the font system will create a separate texture to store the font glyphs, which would result in texture swaps (the renderer would have to switch between the texture containing UI images and the texture containing font glyphs). Therefore, it is recommended to use `UsedSpace`. See FontStashSharp's [How To Use Existing Texture As Font Glyphs Atlas](https://github.com/rds1983/FontStashSharp/wiki/How-To-Use-Existing-Texture-As-Font-Glyphs-Atlas) for more details.

## Loading
Actual stylesheet loading is done through [AssetManagementBase](https://github.com/rds1983/AssetManagementBase).

The loading code is located here: https://github.com/rds1983/Myra/blob/master/src/Myra/DefaultAssets.cs

Since the default stylesheet files are stored as resources the AssetManager creation code is:
```c#
  private static readonly AssetManager _assetManager = AssetManager.CreateResourceAssetManager(typeof(DefaultAssets).Assembly, "Resources.");
```
And the actual stylesheet loading code:
```c#
  _uiStylesheet = _assetManager.LoadStylesheet("default_ui_skin.xmms");
```

## Myra.Samples.CustomUIStylesheet
Myra.Samples.CustomUIStylesheet is another example of full Myra stylesheet.
Again stylesheet files are stored as resources: https://github.com/rds1983/Myra/tree/master/samples/Myra.Samples.CustomUIStylesheet/Resources

The major difference with the default stylesheet is that custom stylesheet uses static font in AngelCode .FNT format:
```xml
  <Fonts>
    <Font Id="commodore-64" File="commodore-64.fnt"/>
  </Fonts>
```

It's important to notice that it uses single [underlying image](https://github.com/rds1983/Myra/blob/master/samples/Myra.Samples.CustomUIStylesheet/Resources/ui_stylesheet_atlas.png) to store both texture atlas images and font glyphs. It's good solution performance-wise as the renderer doesn't need to switch between textures.

Now if you view [commodore-64.fnt](https://github.com/rds1983/Myra/blob/master/samples/Myra.Samples.CustomUIStylesheet/Resources/commodore-64.fnt), you would see how .fnt references the texture region in default_ui_skin.xmat with id 'default' as the image with character glyphs: 
```
file="ui_stylesheet.xmat:commodore-64"
```

And the loading code is:
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

  _**Note**. The default style is being applied to the widget in the moment of the creation. Therefore all changes 
to **Stylesheet.Current** should be done before the UI is created._