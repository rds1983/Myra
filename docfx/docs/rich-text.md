All widgets except TextBox support [FontStashSharp rich text syntax](https://fontstashsharp.github.io/FontStashSharp/docs/rich-text.html).

For example, if we take the following MML:
```xml
<Project>
  <Panel>
    <Label Text="/c[red]First /cd/tuline/n/c[blue]/tdSecond /cd/tsLine" HorizontalAlignment="Center" VerticalAlignment="Center" />
  </Panel>
</Project>
```
It would be rendered:

![alt text](~/images/rich-text-1.png)

Or if we take the following MML:
```xml
<Project>
  <Panel>
    <Button HorizontalAlignment="Center" VerticalAlignment="Center">
      <Label Text="E=mc/v[-8]2/n/vdMass–energy equivalence." />
    </Button>
  </Panel>
</Project>
```

It would result in the following:

![alt text](~/images/rich-text-2.png)

If you want to use commands that require external resources ('/f' and '/i'), you must set RichTextDefaults.FontResolver and RichTextDefaults.ImageResolver.

However, there are special rules for RTF resource resolving when using MyraPad. Resources must be either available in the same folder as the edited xmmp file, or the resource folder path must be specified in the DesignerRtfAssetsPath attribute of the root Project tag. The resource folder path can be either absolute or relative to the location of the edited xmmp file.

For example, if we load the following MML in MyraPad:
```xml
<Project DesignerRtfAssetsPath="../../../tiles/releases/Nov-2015/dngn/trees">
  <Project.ExportOptions />
  <Panel>
    <TabControl HorizontalAlignment="Center" VerticalAlignment="Center" Height="260" GridRow="1">
      <TabItem Text="/i[mangrove1.png] /v[8]First Tab" />
      <TabItem Text="/i[mangrove2.png] /v[8]/tsSecond Tab" />
      <TabItem Text="/i[mangrove3.png] /v[8]/tuThird Tab" />
    </TabControl>
  </Panel>
</Project>
```

It would render the following(assuming we have corresponding images mangrove*.png in the corresponding folder):

![alt text](~/images/rich-text-3.png)

The '/f' command parameter must be passed in the form 'fontFileName, size'.

For example, if we load the following MML:
```xml
<Project DesignerRtfAssetsPath="C:\Windows\Fonts">
  <Project.ExportOptions />
  <Panel>
    <Label Text="Text in default font./n/f[arialbd.ttf, 24]Bold and smaller font. /f[ariali.ttf, 48]Italic and larger font./n/fdBack to the default font." HorizontalAlignment="Center" VerticalAlignment="Center" />
  </Panel>
</Project>
```

It would render the following:

![alt text](~/images/rich-text-4.png)

  **Note:** The above resource resolving rules only work in MyraPad. The property DesignerRtfAssetsPath is ignored at runtime. It is the developer's responsibility to set RichTextDefaults.FontResolver and RichTextDefaults.ImageResolver if you want to use external resources in rich text at runtime.
