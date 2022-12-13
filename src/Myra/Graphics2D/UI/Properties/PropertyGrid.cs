using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Graphics2D.UI.File;
using Myra.Graphics2D.TextureAtlases;
using System.IO;
using Myra.Attributes;
using FontStashSharp;
using FontStashSharp.RichText;
using Myra.Graphics2D.Brushes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Properties
{
    public class PropertyGrid : SingleItemContainer<Grid>
    {
        private const string DefaultCategoryName = "Miscellaneous";

        private class SubGrid : SingleItemContainer<Grid>
        {
            private readonly ImageButton _mark;
            private readonly PropertyGrid _propertyGrid;

            public ImageButton Mark
            {
                get { return _mark; }
            }

            public PropertyGrid PropertyGrid
            {
                get { return _propertyGrid; }
            }

            public Rectangle HeaderBounds
            {
                get
                {
                    var headerBounds = new Rectangle(0, 0, ActualBounds.Width, InternalChild.GetRowHeight(0));

                    return headerBounds;
                }
            }

            [Browsable(false)]
            [XmlIgnore]
            public bool IsEmpty
            {
                get
                {
                    return _propertyGrid.IsEmpty;
                }
            }

            public SubGrid(PropertyGrid parent, object value, string header, string category, string filter, Record parentProperty)
            {
                InternalChild = new Grid
                {
                    ColumnSpacing = 4,
                    RowSpacing = 4
                };

                InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
                InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
                InternalChild.RowsProportions.Add(new Proportion(ProportionType.Auto));
                InternalChild.RowsProportions.Add(new Proportion(ProportionType.Auto));

                _propertyGrid = new PropertyGrid(parent.PropertyGridStyle, category, parentProperty, parent)
                {
                    Object = value,
                    Filter = filter,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    GridColumn = 1,
                    GridRow = 1
                };

                // Mark
                _mark = new ImageButton(null)
                {
                    Toggleable = true,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                _mark.ApplyImageButtonStyle(parent.PropertyGridStyle.MarkStyle);

                InternalChild.Widgets.Add(_mark);

                _mark.PressedChanged += (sender, args) =>
                {
                    if (_mark.IsPressed)
                    {
                        InternalChild.Widgets.Add(_propertyGrid);
                        parent._expandedCategories.Add(category);
                    }
                    else
                    {
                        InternalChild.Widgets.Remove(_propertyGrid);
                        parent._expandedCategories.Remove(category);
                    }
                };

                var expanded = true;
                if (parentProperty != null && parentProperty.FindAttribute<DesignerFoldedAttribute>() != null)
                {
                    expanded = false;
                }

                if (expanded)
                {
                    _mark.IsPressed = true;
                }

                var label = new Label(null)
                {
                    Text = header,
                    GridColumn = 1
                };
                label.ApplyLabelStyle(parent.PropertyGridStyle.LabelStyle);

                InternalChild.Widgets.Add(label);

                HorizontalAlignment = HorizontalAlignment.Stretch;
                VerticalAlignment = VerticalAlignment.Stretch;
            }

            public override void OnTouchDoubleClick()
            {
                base.OnTouchDoubleClick();

                var mousePosition = ToLocal(Desktop.MousePosition);
                if (!HeaderBounds.Contains(mousePosition) || _mark.Bounds.Contains(mousePosition))
                {
                    return;
                }

                _mark.IsPressed = !_mark.IsPressed;
            }

            public override void InternalRender(RenderContext context)
            {
                if (_propertyGrid.PropertyGridStyle.SelectionHoverBackground != null && UseHoverRenderable)
                {
                    var headerBounds = HeaderBounds;
                    if (headerBounds.Contains(ToLocal(Desktop.MousePosition)))
                    {
                        _propertyGrid.PropertyGridStyle.SelectionHoverBackground.Draw(context, headerBounds);
                    }
                }

                base.InternalRender(context);
            }
        }

        private readonly PropertyGrid _parentGrid;
        private Record _parentProperty;
        private readonly Dictionary<string, List<Record>> _records = new Dictionary<string, List<Record>>();
        private readonly HashSet<string> _expandedCategories = new HashSet<string>();
        private object _object;
        private bool _ignoreCollections;
        private readonly PropertyGridSettings _settings = new PropertyGridSettings();
        private string _filter;

        [Browsable(false)]
        [XmlIgnore]
        public TreeStyle PropertyGridStyle
        {
            get; private set;
        }

        [Browsable(false)]
        [XmlIgnore]
        public object Object
        {
            get { return _object; }

            set
            {
                if (value == _object)
                {
                    return;
                }

                _object = value;
                Rebuild();
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public string Category { get; private set; }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IgnoreCollections
        {
            get
            {
                if (_parentGrid != null)
                {
                    return _parentGrid.IgnoreCollections;
                }

                return _ignoreCollections;
            }

            set
            {
                _ignoreCollections = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public bool IsEmpty
        {
            get
            {
                return InternalChild.Widgets.Count == 0;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public PropertyGridSettings Settings
        {
            get
            {
                if (_parentGrid != null)
                {
                    return _parentGrid.Settings;
                }

                return _settings;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public int FirstColumnWidth
        {
            get
            {
                return (int)InternalChild.ColumnsProportions[0].Value;
            }

            set
            {
                InternalChild.ColumnsProportions[0].Value = value;
            }
        }

        [DefaultValue(HorizontalAlignment.Stretch)]
        public override HorizontalAlignment HorizontalAlignment
        {
            get { return base.HorizontalAlignment; }
            set { base.HorizontalAlignment = value; }
        }

        [DefaultValue(VerticalAlignment.Stretch)]
        public override VerticalAlignment VerticalAlignment
        {
            get { return base.VerticalAlignment; }
            set { base.VerticalAlignment = value; }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string Filter
        {
            get => _filter;
            set
            {
                if (_filter == value)
                {
                    return;
                }

                _filter = value;
                Rebuild();
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public Func<Record, object[]> CustomValuesProvider;

        [Browsable(false)]
        [XmlIgnore]
        public Func<Record, object, object, bool> CustomSetter;

        [Browsable(false)]
        [XmlIgnore]
        public Func<Record, object, Widget> CustomWidgetProvider;

        public event EventHandler<GenericEventArgs<string>> PropertyChanged;

        private PropertyGrid(TreeStyle style, string category, Record parentProperty, PropertyGrid parentGrid = null)
        {
            _parentGrid = parentGrid;
            InternalChild = new Grid();

            _parentProperty = parentProperty;
            InternalChild.ColumnSpacing = 8;
            InternalChild.RowSpacing = 8;
            InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
            InternalChild.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));

            Category = category;

            if (style != null)
            {
                ApplyPropertyGridStyle(style);
            }

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Filter = string.Empty;

            this.CustomWidgetProvider = parentGrid?.CustomWidgetProvider;
            this.CustomSetter = parentGrid?.CustomSetter;
            this.CustomValuesProvider = parentGrid?.CustomValuesProvider;
        }

        public PropertyGrid(TreeStyle style, string category) : this(style, category, null)
        {
        }

        public PropertyGrid(string category) : this(Stylesheet.Current.TreeStyle, category)
        {
        }

        public PropertyGrid() : this(DefaultCategoryName)
        {
        }

        private void FireChanged(string name)
        {
            var ev = PropertyChanged;

            var p = _parentGrid;
            while (p != null)
            {
                ev = p.PropertyChanged;
                p = p._parentGrid;
            }

            if (ev != null)
            {
                ev(this, new GenericEventArgs<string>(name));
            }
        }

        private static void UpdateLabelCount(Label textBlock, int count)
        {
            textBlock.Text = string.Format("{0} Items", count);
        }

        private void SetValue(Record record, object obj, object value)
        {
            if (CustomSetter != null && CustomSetter(record, obj, value))
            {
                return;
            }

            record.SetValue(obj, value);
        }

        private ComboBox CreateCustomValuesEditor(Record record, object[] customValues, bool hasSetter)
        {
            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var cb = new ComboBox();
            foreach (var v in customValues)
            {
                cb.Items.Add(new ListItem(v.ToString(), null, v));
            }

            cb.SelectedIndex = Array.IndexOf(customValues, value);
            if (hasSetter)
            {
                cb.SelectedIndexChanged += (sender, args) =>
                {
                    var item = cb.SelectedIndex != null ? customValues[cb.SelectedIndex.Value] : null;
                    SetValue(record, _object, item);
                    FireChanged(propertyType.Name);
                };
            }
            else
            {
                cb.Enabled = false;
            }

            return cb;
        }

        private CheckBox CreateBooleanEditor(Record record, bool hasSetter)
        {
            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var isChecked = (bool)value;
            var cb = new CheckBox
            {
                IsPressed = isChecked
            };

            if (hasSetter)
            {
                cb.Click += (sender, args) =>
                {
                    SetValue(record, _object, cb.IsPressed);
                    FireChanged(propertyType.Name);
                };
            }
            else
            {
                cb.Enabled = false;
            }

            return cb;
        }

        private Grid CreateColorEditor(Record record, bool hasSetter)
        {
            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var subGrid = new Grid
            {
                ColumnSpacing = 8,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            var isColor = propertyType == typeof(Color);

            subGrid.ColumnsProportions.Add(new Proportion());
            subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

            var color = Color.Transparent;
            if (isColor)
            {
                color = (Color)value;
            }
            else if (value != null)
            {
                color = ((Color?)value).Value;
            }

            var image = new Image
            {
                Renderable = DefaultAssets.WhiteRegion,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 32,
                Height = 16,
                Color = color
            };

            subGrid.Widgets.Add(image);

            var button = new ImageTextButton
            {
                Text = "Change...",
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                Tag = value,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                GridColumn = 1
            };

            subGrid.Widgets.Add(button);

            if (hasSetter)
            {
                button.Click += (sender, args) =>
                {
                    var dlg = new ColorPickerDialog()
                    {
                        Color = image.Color
                    };

                    dlg.Closed += (s, a) =>
                    {
                        if (!dlg.Result)
                        {
                            return;
                        }

                        image.Color = dlg.Color;
                        SetValue(record, _object, dlg.Color);

                        FireChanged(propertyType.Name);
                    };

                    dlg.ShowModal(Desktop);
                };
            }
            else
            {
                button.Enabled = false;
            }

            return subGrid;
        }

        private Grid CreateBrushEditor(Record record, bool hasSetter)
        {
            var propertyType = record.Type;

            var value = record.GetValue(_object) as SolidBrush;

            var subGrid = new Grid
            {
                ColumnSpacing = 8,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            subGrid.ColumnsProportions.Add(new Proportion());
            subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

            var color = Color.Transparent;
            if (value != null)
            {
                color = value.Color;
            }

            var image = new Image
            {
                Renderable = DefaultAssets.WhiteRegion,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 32,
                Height = 16,
                Color = color
            };

            subGrid.Widgets.Add(image);

            var button = new ImageTextButton
            {
                Text = "Change...",
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                Tag = value,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                GridColumn = 1
            };

            subGrid.Widgets.Add(button);

            if (hasSetter)
            {
                button.Click += (sender, args) =>
                {
                    var dlg = new ColorPickerDialog()
                    {
                        Color = image.Color
                    };

                    dlg.Closed += (s, a) =>
                    {
                        if (!dlg.Result)
                        {
                            return;
                        }

                        image.Color = dlg.Color;
                        SetValue(record, _object, new SolidBrush(dlg.Color));
                        var baseObject = _object as BaseObject;
                        if (baseObject != null)
                        {
                            baseObject.Resources[record.Name] = dlg.Color.ToHexString();
                        }
                        FireChanged(propertyType.Name);
                    };

                    dlg.ShowModal(Desktop);
                };
            }
            else
            {
                button.Enabled = false;
            }

            return subGrid;
        }

        private ComboBox CreateEnumEditor(Record record, bool hasSetter)
        {
            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var isNullable = propertyType.IsNullableEnum();
            var enumType = isNullable ? propertyType.GetNullableType() : propertyType;
            var values = Enum.GetValues(enumType);

            var cb = new ComboBox();

            if (isNullable)
            {
                cb.Items.Add(new ListItem(string.Empty, null, null));
            }

            foreach (var v in values)
            {
                cb.Items.Add(new ListItem(v.ToString(), null, v));
            }

            var selectedIndex = Array.IndexOf(values, value);
            if (isNullable)
            {
                ++selectedIndex;
            }
            cb.SelectedIndex = selectedIndex;

            if (hasSetter)
            {
                cb.SelectedIndexChanged += (sender, args) =>
                {
                    if (cb.SelectedIndex != -1)
                    {
                        SetValue(record, _object, cb.SelectedItem.Tag);
                        FireChanged(enumType.Name);
                    }
                };
            }
            else
            {
                cb.Enabled = false;
            }

            return cb;
        }

        private SpinButton CreateNumericEditor(Record record, bool hasSetter)
        {
            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var numericType = propertyType;
            if (propertyType.IsNullablePrimitive())
            {
                numericType = propertyType.GetNullableType();
            }

            var spinButton = new SpinButton
            {
                Integer = numericType.IsNumericInteger(),
                Nullable = propertyType.IsNullablePrimitive(),
                Value = value != null ? (float)Convert.ChangeType(value, typeof(float)) : default(float?)
            };

            if (hasSetter)
            {
                spinButton.ValueChanged += (sender, args) =>
                {
                    try
                    {
                        object result;

                        if (spinButton.Value != null)
                        {
                            result = Convert.ChangeType(spinButton.Value.Value, numericType);
                        }
                        else
                        {
                            result = null;
                        }

                        SetValue(record, _object, result);

                        if (record.Type.IsValueType)
                        {
                            // Handle structs
                            var tg = this;
                            var pg = tg._parentGrid;
                            while (pg != null && tg._parentProperty != null && tg._parentProperty.Type.IsValueType)
                            {
                                tg._parentProperty.SetValue(pg._object, tg._object);

                                if (!tg._parentProperty.Type.IsValueType)
                                {
                                    break;
                                }

                                tg = pg;
                                pg = tg._parentGrid;
                            }
                        }

                        FireChanged(record.Name);
                    }
                    catch (InvalidCastException)
                    {
                        // TODO: Rework this ugly type conversion solution
                    }
                    catch (Exception ex)
                    {
                        spinButton.Value = args.OldValue;
                        var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
                        dialog.ShowModal(Desktop);
                    }
                };
            }
            else
            {
                spinButton.Enabled = false;
            }

            return spinButton;
        }

        private TextBox CreateStringEditor(Record record, bool hasSetter)
        {
            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var tf = new TextBox
            {
                Text = value != null ? value.ToString() : string.Empty
            };

            if (hasSetter)
            {
                tf.TextChanged += (sender, args) =>
                {
                    try
                    {
                        object result;

                        if (propertyType.IsNullablePrimitive())
                        {
                            if (string.IsNullOrEmpty(tf.Text))
                            {
                                result = null;
                            }
                            else
                            {
                                result = Convert.ChangeType(tf.Text, record.Type.GetNullableType());
                            }
                        }
                        else
                        {
                            result = Convert.ChangeType(tf.Text, record.Type);
                        }

                        SetValue(record, _object, result);

                        if (record.Type.IsValueType)
                        {
                            var tg = this;
                            var pg = tg._parentGrid;
                            while (pg != null && tg._parentProperty != null)
                            {
                                tg._parentProperty.SetValue(pg._object, tg._object);

                                if (!tg._parentProperty.Type.IsValueType)
                                {
                                    break;
                                }

                                tg = pg;
                                pg = tg._parentGrid;
                            }
                        }

                        FireChanged(record.Name);
                    }
                    catch (Exception)
                    {
                        // TODO: Rework this ugly type conversion solution
                    }
                };
            }
            else
            {
                tf.Enabled = false;
            }

            return tf;
        }

        private Grid CreateCollectionEditor(Record record, Type itemType)
        {
            var value = record.GetValue(_object);

            var items = (IList)value;

            var subGrid = new Grid
            {
                ColumnSpacing = 8,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            subGrid.ColumnsProportions.Add(new Proportion());
            subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

            var label = new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
            };
            UpdateLabelCount(label, items.Count);

            subGrid.Widgets.Add(label);

            var button = new ImageTextButton
            {
                Text = "Change...",
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                Tag = value,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                GridColumn = 1
            };

            button.Click += (sender, args) =>
            {
                var collectionEditor = new CollectionEditor(items, itemType);

                var dialog = Dialog.CreateMessageBox("Edit", collectionEditor);

                dialog.ButtonOk.Click += (o, eventArgs) =>
                {
                    collectionEditor.SaveChanges();
                    UpdateLabelCount(label, items.Count);
                };

                dialog.ShowModal(Desktop);
            };

            subGrid.Widgets.Add(button);

            return subGrid;
        }

        private Grid CreateFileEditor<T>(Record record, bool hasSetter, string filter)
        {
            if (Settings.AssetManager == null)
            {
                return null;
            }

            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var subGrid = new Grid
            {
                ColumnSpacing = 8,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));
            subGrid.ColumnsProportions.Add(new Proportion());

            var baseObject = _object as BaseObject;
            var path = string.Empty;
            if (baseObject != null)
            {
                baseObject.Resources.TryGetValue(record.Name, out path);
            }

            var textBox = new TextBox
            {
                Text = path
            };

            subGrid.Widgets.Add(textBox);

            var button = new ImageTextButton
            {
                Text = "Change...",
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                Tag = value,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                GridColumn = 1
            };

            subGrid.Widgets.Add(button);

            if (hasSetter)
            {
                button.Click += (sender, args) =>
                {
                    var dlg = new FileDialog(FileDialogMode.OpenFile)
                    {
                        Filter = filter
                    };

                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        var filePath = textBox.Text;
                        if (!Path.IsPathRooted(filePath) && !string.IsNullOrEmpty(Settings.BasePath))
                        {
                            filePath = Path.Combine(Settings.BasePath, filePath);
                        }
                        dlg.FilePath = filePath;
                    }
                    else if (!string.IsNullOrEmpty(Settings.BasePath))
                    {
                        dlg.Folder = Settings.BasePath;
                    }

                    dlg.Closed += (s, a) =>
                    {
                        if (!dlg.Result)
                        {
                            return;
                        }

                        try
                        {
                            var newValue = Settings.AssetManager.Load<T>(dlg.FilePath);

                            var filePath = dlg.FilePath;
                            if (!string.IsNullOrEmpty(Settings.BasePath))
                            {
                                filePath = PathUtils.TryToMakePathRelativeTo(filePath, Settings.BasePath);
                            }

                            textBox.Text = filePath;
                            SetValue(record, _object, newValue);
                            if (baseObject != null)
                            {
                                baseObject.Resources[record.Name] = filePath;
                            }

                            FireChanged(propertyType.Name);
                        }
                        catch (Exception)
                        {

                        }
                    };

                    dlg.ShowModal(Desktop);
                };
            }
            else
            {
                button.Enabled = false;
            }

            return subGrid;
        }

        private Widget CreateAttributeFileEditor(Record record, bool hasSetter, FilePathAttribute attribute)
        {
            var propertyType = record.Type;
            var value = record.GetValue(_object);

            var result = new HorizontalStackPanel
            {
                Spacing = 8
            };

            TextBox path = null;
            if (attribute.ShowPath)
            {
                result.Proportions.Add(Proportion.Fill);
                path = new TextBox
                {
                    Readonly = true,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                if (value != null)
                {
                    path.Text = value.ToString();
                }

                result.AddChild(path);
            }

            result.Proportions.Add(Proportion.Auto);

            var button = new TextButton
            {
                Text = "Change...",
                ContentHorizontalAlignment = HorizontalAlignment.Center,
                Tag = value,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                GridColumn = 1
            };

            if (hasSetter)
            {
                button.Click += (sender, args) =>
                {
                    var dlg = new FileDialog(attribute.DialogMode)
                    {
                        Filter = attribute.Filter
                    };

                    if (value != null)
                    {
                        var filePath = value.ToString();
                        if (!Path.IsPathRooted(filePath) && !string.IsNullOrEmpty(Settings.BasePath))
                        {
                            filePath = Path.Combine(Settings.BasePath, filePath);
                        }
                        dlg.FilePath = filePath;
                    }
                    else if (!string.IsNullOrEmpty(Settings.BasePath))
                    {
                        dlg.Folder = Settings.BasePath;
                    }

                    dlg.Closed += (s, a) =>
                    {
                        if (!dlg.Result)
                        {
                            return;
                        }

                        try
                        {
                            var filePath = dlg.FilePath;
                            if (!string.IsNullOrEmpty(Settings.BasePath))
                            {
                                filePath = PathUtils.TryToMakePathRelativeTo(filePath, Settings.BasePath);
                            }

                            if (path != null)
                            {
                                path.Text = filePath;
                            }

                            SetValue(record, _object, filePath);

                            FireChanged(propertyType.Name);
                        }
                        catch (Exception)
                        {
                        }
                    };

                    dlg.ShowModal(Desktop);
                };
            }
            else
            {
                button.Enabled = false;
            }

            result.AddChild(button);

            return result;
        }

        private void FillSubGrid(ref int y, IReadOnlyList<Record> records)
        {
            for (var i = 0; i < records.Count; ++i)
            {
                var record = records[i];

                var hasSetter = record.HasSetter;
                if (_parentProperty != null && _parentProperty.Type.IsValueType && !_parentProperty.HasSetter)
                {
                    hasSetter = false;
                }

                var value = record.GetValue(_object);
                Widget valueWidget = null;

                var oldY = y;

                var propertyType = record.Type;

                Proportion rowProportion;
                object[] customValues = null;

                if ((valueWidget = CustomWidgetProvider?.Invoke(record, _object)) != null)
                {

                }
                else if (CustomValuesProvider != null && (customValues = CustomValuesProvider(record)) != null)
                {
                    if (customValues.Length == 0)
                    {
                        continue;
                    }

                    valueWidget = CreateCustomValuesEditor(record, customValues, hasSetter);
                }
                else if (propertyType == typeof(bool))
                {
                    valueWidget = CreateBooleanEditor(record, hasSetter);
                }
                else if (propertyType == typeof(Color) || propertyType == typeof(Color?))
                {
                    valueWidget = CreateColorEditor(record, hasSetter);
                }
                else if (propertyType.IsEnum || propertyType.IsNullableEnum())
                {
                    valueWidget = CreateEnumEditor(record, hasSetter);
                }
                else if (propertyType.IsNumericType() ||
                         (propertyType.IsNullablePrimitive() && propertyType.GetNullableType().IsNumericType()))
                {

                    valueWidget = CreateNumericEditor(record, hasSetter);
                }
                else if (propertyType == typeof(string) && record.FindAttribute<FilePathAttribute>() != null)
                {
                    var filePathAttr = record.FindAttribute<FilePathAttribute>();
                    valueWidget = CreateAttributeFileEditor(record, hasSetter, filePathAttr);
                }
                else if (propertyType == typeof(string) || propertyType.IsPrimitive || propertyType.IsNullablePrimitive())
                {
                    valueWidget = CreateStringEditor(record, hasSetter);
                }
                else if (typeof(IList).IsAssignableFrom(propertyType))
                {
                    if (!IgnoreCollections)
                    {
                        var it = propertyType.FindGenericType(typeof(ICollection<>));
                        if (it != null)
                        {
                            var itemType = it.GenericTypeArguments[0];
                            if (value != null)
                            {
                                valueWidget = CreateCollectionEditor(record, itemType);
                            }
                        }
                    }
                }
                else if (propertyType == typeof(SpriteFontBase))
                {
                    valueWidget = CreateFileEditor<SpriteFontBase>(record, hasSetter, "*.fnt");
                }
                else if (propertyType == typeof(IBrush))
                {
                    valueWidget = CreateBrushEditor(record, hasSetter);
                }
                else if (propertyType == typeof(IImage))
                {
                    valueWidget = CreateFileEditor<TextureRegion>(record, hasSetter, "*.png|*.jpg|*.bmp|*.gif");
                }
                else
                {
                    // Subgrid
                    if (value != null)
                    {
                        if (PassesFilter(record.Name))
                        {
                            var subGrid = new SubGrid(this, value, record.Name, DefaultCategoryName, string.Empty, record)
                            {
                                GridColumnSpan = 2,
                                GridRow = y
                            };

                            InternalChild.Widgets.Add(subGrid);

                            rowProportion = new Proportion(ProportionType.Auto);
                            InternalChild.RowsProportions.Add(rowProportion);
                            ++y;
                        }

                        continue;
                    }

                    var tb = new Label();
                    tb.ApplyLabelStyle(PropertyGridStyle.LabelStyle);
                    tb.Text = "null";

                    valueWidget = tb;
                }

                if (valueWidget == null)
                {
                    continue;
                }

                var name = record.Name;
                var dn = record.FindAttribute<DisplayNameAttribute>();

                if (dn != null)
                {
                    name = dn.DisplayName;
                }

                if (!PassesFilter(name))
                {
                    continue;
                }

                var nameLabel = new Label
                {
                    Text = name,
                    VerticalAlignment = VerticalAlignment.Center,
                    GridColumn = 0,
                    GridRow = oldY
                };

                InternalChild.Widgets.Add(nameLabel);

                valueWidget.GridColumn = 1;
                valueWidget.GridRow = oldY;
                valueWidget.HorizontalAlignment = HorizontalAlignment.Stretch;
                valueWidget.VerticalAlignment = VerticalAlignment.Top;

                InternalChild.Widgets.Add(valueWidget);

                rowProportion = new Proportion(ProportionType.Auto);
                InternalChild.RowsProportions.Add(rowProportion);
                ++y;
            }
        }

        public bool PassesFilter(string name)
        {
            if (string.IsNullOrEmpty(Filter) || string.IsNullOrEmpty(name))
            {
                return true;
            }

            return name.ToLower().Contains(_filter.ToLower());
        }

        public void Rebuild()
        {
            InternalChild.RowsProportions.Clear();
            InternalChild.Widgets.Clear();
            _records.Clear();
            _expandedCategories.Clear();

            if (_object == null)
            {
                return;
            }

            var properties = from p in _object.GetType().GetProperties() select p;
            var records = new List<Record>();
            foreach (var property in properties)
            {
                if (property.GetGetMethod() == null ||
                    !property.GetGetMethod().IsPublic ||
                    property.GetGetMethod().IsStatic)
                {
                    continue;
                }

                var hasSetter = property.GetSetMethod() != null &&
                                property.GetSetMethod().IsPublic;

                var browsableAttr = property.FindAttribute<BrowsableAttribute>();
                if (browsableAttr != null && !browsableAttr.Browsable)
                {
                    continue;
                }

                var readOnlyAttr = property.FindAttribute<ReadOnlyAttribute>();
                if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
                {
                    hasSetter = false;
                }

                var record = new PropertyRecord(property)
                {
                    HasSetter = hasSetter
                };

                var categoryAttr = property.FindAttribute<CategoryAttribute>();
                record.Category = categoryAttr != null ? categoryAttr.Category : DefaultCategoryName;

                records.Add(record);
            }

            var fields = from f in _object.GetType().GetFields() select f;
            foreach (var field in fields)
            {
                if (!field.IsPublic || field.IsStatic)
                {
                    continue;
                }

                var browsableAttr = field.FindAttribute<BrowsableAttribute>();
                if (browsableAttr != null && !browsableAttr.Browsable)
                {
                    continue;
                }

                var categoryAttr = field.FindAttribute<CategoryAttribute>();

                var hasSetter = true;
                var readOnlyAttr = field.FindAttribute<ReadOnlyAttribute>();
                if (readOnlyAttr != null && readOnlyAttr.IsReadOnly)
                {
                    hasSetter = false;
                }


                var record = new FieldRecord(field)
                {
                    HasSetter = hasSetter,
                    Category = categoryAttr != null ? categoryAttr.Category : DefaultCategoryName
                };

                records.Add(record);
            }

            // Sort by categories
            for (var i = 0; i < records.Count; ++i)
            {
                var record = records[i];

                List<Record> categoryRecords;
                if (!_records.TryGetValue(record.Category, out categoryRecords))
                {
                    categoryRecords = new List<Record>();
                    _records[record.Category] = categoryRecords;
                }

                categoryRecords.Add(record);
            }

            // Sort by names within categories
            foreach (var category in _records)
            {
                category.Value.Sort((a, b) => Comparer<string>.Default.Compare(a.Name, b.Name));
            }

            var ordered = _records.OrderBy(key => key.Key);

            var y = 0;
            List<Record> defaultCategoryRecords;
            if (_records.TryGetValue(Category, out defaultCategoryRecords))
            {
                FillSubGrid(ref y, defaultCategoryRecords);
            }

            if (Category != DefaultCategoryName)
            {
                return;
            }

            foreach (var category in ordered)
            {
                if (category.Key == DefaultCategoryName)
                {
                    continue;
                }

                var subGrid = new SubGrid(this, Object, category.Key, category.Key, Filter, null)
                {
                    GridColumnSpan = 2,
                    GridRow = y
                };

                if (subGrid.IsEmpty)
                {
                    continue;
                }

                InternalChild.Widgets.Add(subGrid);

                if (_expandedCategories.Contains(category.Key))
                {
                    subGrid.Mark.IsPressed = true;
                }

                var rp = new Proportion(ProportionType.Auto);
                InternalChild.RowsProportions.Add(rp);

                y++;
            }
        }

        public void ApplyPropertyGridStyle(TreeStyle style)
        {
            ApplyWidgetStyle(style);

            PropertyGridStyle = style;
        }
    }
}