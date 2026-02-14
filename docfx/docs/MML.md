MML(Myra Markup Language) is XML based declarative language to describe UI.

I.e. following MML is equivalent to the UI from [[Quick Start Tutorial]]:
```xml
<Project>
  <Project.ExportOptions />
  <Grid ColumnSpacing="8" RowSpacing="8">
    <Grid.ColumnsProportions>
      <Proportion Type="Auto" />
      <Proportion Type="Auto" />
    </Grid.ColumnsProportions>
    <Grid.RowsProportions>
      <Proportion Type="Auto" />
      <Proportion Type="Auto" />
    </Grid.RowsProportions>
    <Label Text="Hello, World!" Id="label" />
    <ComboBox Grid.Column="1">
      <ListItem Text="Red" Color="#FF0000FF" />
      <ListItem Text="Green" Color="#008000FF" />
      <ListItem Text="Blue" Color="#0000FFFF" />
    </ComboBox>
    <Button Grid.Row="1"><Label Text="Show"/></Button>
    <SpinButton Nullable="True" Width="100" Grid.Column="1" Grid.Row="1" />
  </Grid>
</Project>
```

".xmmp" is preferred extension for files with MML.

Following code loads Project from MML:
```c#
string data = File.ReadAllText(filePath);
Project project = Project.LoadFromXml(data);
```

Following code saves it:	
```c#	
string data = project.Save();	
File.WriteAllText(filePath, data);	
```

Following code is used to obtain a reference to the element by id (used to attach event handlers):
```c#
Label label = (Label) project.Root.FindChildById("label");
```