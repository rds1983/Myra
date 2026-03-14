All widgets except TextBox do support [FontStashSharp rich text syntax](https://fontstashsharp.github.io/FontStashSharp/docs/rich-text.html).

I.e. if we take following MML:
```xml
<Project>
  <Panel>
    <Label Text="/c[red]First /cd/tuline/n/c[blue]/tdSecond /cd/tsLine" HorizontalAlignment="Center" VerticalAlignment="Center" />
  </Panel>
</Project>
```
It would be rendered:

![alt text](~/images/rich-text-1.png)

Or if we take MML:
```xml
<Project>
  <Panel>
    <Button HorizontalAlignment="Center" VerticalAlignment="Center">
      <Label Text="E=mc/v[-8]2/n/vdMass–energy equivalence." />
    </Button>
  </Panel>
</Project>
```

It would result in:

![alt text](~/images/rich-text-2.png)

If we want to use commands that require external resources('/f' and '/i'), then we would need to set RichTextDefaults.FontResolver and RichTextDefaults.ImageResolver.

However there are special rules for the rtf resource resolving when using MyraPad. It requires resources to be either available in the same folder as the edited xmmp file. Or the resource folder path must be specified in the attribute DesignerRtfAssetsPath of the root tag Project. The resource folder path could be either absolute or relative to the location of the edited xmmp.

I.e. if we load following MML in the MyraPad:
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

It would render following(assuming we have corresponding images mangrove*.png in the corresponding folder):

![alt text](~/images/rich-text-3.png)

The '/f' command parameter must be passed in form 'fontFileName, size'.

I.e. if we load following MML:
```xml
<Project DesignerRtfAssetsPath="C:\Windows\Fonts">
  <Project.ExportOptions />
  <Panel>
    <Label Text="Text in default font./n/f[arialbd.ttf, 24]Bold and smaller font. /f[ariali.ttf, 48]Italic and larger font./n/fdBack to the default font." HorizontalAlignment="Center" VerticalAlignment="Center" />
  </Panel>
</Project>
```

It would render following:

![alt text](~/images/rich-text-4.png)

  **Note**. Above resource resolving rules work only in the MyraPad. The property DesignerRtfAssetsPath is ignored in the run-time. Thus its developer responsibility to set RichTextDefaults.FontResolver and RichTextDefaults.ImageResolver, if they want to use external resources in the rich text during the run-time.
