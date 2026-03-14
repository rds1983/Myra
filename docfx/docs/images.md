### Overview
This entry describes in detail how Myra deals with images.

### IBrush
[IBrush](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/IBrush.cs) represents something that can draw itself in the specified rectangle with the specified color:
```c#
  public interface IBrush
  {
    void Draw(RenderContext context, Rectangle dest, Color color);
  }
```

Many widgets properties such as Widget.Background or Menu.SelectionBackground have IBrush type.
The most simple implementation of IBrush is [SolidBrush](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/Brushes/SolidBrush.cs).

I.e. following code sets SolidBrush as widget.Background:
```c#
  widget.Background = new SolidBrush(Color.Red); // SolidBrush from Color
  widget.Background = new SolidBrush("#808000FF"); // SolidBrush from RGBA string
  widget.Background = new SolidBrush("#FFA500"); // SolidBrush from RGB string
```
Also it could be set through [MML](MML.md).

I.e. following MML:
```xml
<Project>
  <Panel>
    <HorizontalStackPanel Spacing="8">
      <Panel Width="100" Height="50" Background="#FF0000FF" />
      <Label Text="Label" Background="#0000FF" />
      <Button Background="YellowGreen">
        <Label Text="Push Me" />
      </Button>
    </HorizontalStackPanel>
  </Panel>
</Project>
```
Would result in following image:

![alt text](~/images/images.png)

### IImage
[IImage](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/IImage.cs) extends IBrush with Size property:
```c#
  public interface IImage: IBrush
  {
    Point Size { get; }
  }
```
Widgets properties such as Image.Renderable or TextBox.Cursor have IImage type.

Myra provides 2 IImage implementation: TextureRegion and NinePatchRegion.

### TextureRegion
[TextureRegion](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/TextureAtlases/TextureRegion.cs) describes rectangle in the texture. TextureRegion implements IImage. 

Therefore following code will work:
```c#
// 'texture' is object of type Texture2D
image.Renderable = new TextureRegion(texture, new Rectangle(10, 10, 50, 50));
```

Also following:
```c#
// If 2nd parameter is omitted, then TextureRegion covers the whole texture
image.Renderable = new TextureRegion(texture);
```
  _**Note**. It's also possible to use TextureRegion as IBrush. However usually it wont make much sense, since it would result in the TextureRegion streched over the rectangle IBrush is drawn at._

### NinePatchRegion
[NinePatchRegion](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/TextureAtlases/NinePatchRegion.cs) represents region with unstrechable border and strechable center. 

It could be used following way:
```c#
widget.Background = new NinePatchRegion(texture, new Rectangle(10, 10, 50, 50), 
                                        new Thickness {Left = 2, Right = 2, 
                                                       Top = 2, Bottom = 2});
```
  _**Note**. Since NinePatchRegion is stretchable, it makes a lot of sense to use it as IBrush. In fact all backgrounds of the Myra widgets are NinePatchRegion._

### TextureRegionAtlas
[TextureRegionAtlas](https://github.com/rds1983/Myra/blob/master/src/Myra/Graphics2D/TextureAtlases/TextureRegionAtlas.cs) is collection of texture regions(each could be nine patch) accessible by string key.

It could be loaded from [MyraTexturePacker](https://github.com/rds1983/MyraTexturePacker) format(.xmat) using following code:
```c#
// 'data' is string containing contents of .atlas file
// 'textures' is dictionary that maps texture file names to actual textures
TextureRegionAtlas spriteSheet = TextureRegionAtlas.Load(data, name => textures[name]);
```