using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.Styles;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Properties
{
    [PropertyEditor(typeof(ColorPropertyEditor), typeof(Color))]
    public sealed class ColorPropertyEditor : StructPropertyEditor<Color>
    {
        private Image _colorDisplay;
        public ColorPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {

        }

        protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
        {
            creatorDelegate = CreateEditor;
            return true;
        }

        private bool CreateEditor(out Widget widget)
        {
            if (_owner.SelectedField == null)
            {
                widget = null;
                return false;
            }
            
            var propertyType = _record.Type;
            var value = _record.GetValue(_owner.SelectedField);
            
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
                Renderable = Stylesheet.Current.WhiteRegion,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 32,
                Height = 16,
                Color = color
            };
            _colorDisplay = image;
            subGrid.Widgets.Add(image);

            var button = new Button
            {
                Tag = value,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Content = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = "Change..."
                }
            };
            Grid.SetColumn(button, 1);

            subGrid.Widgets.Add(button);
            widget = subGrid;
            
            if (_record.HasSetter)
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
                        SetValue(_owner.SelectedField, dlg.Color);

                        _owner.FireChanged(propertyType.Name);
                    };

                    dlg.ShowModal(_owner.Desktop);
                };
            }
            else
            {
                button.Enabled = false;
            }

            return true;
        }
        
        public override void SetWidgetValue(Color? value)
        {
            if (!value.HasValue)
                value = Color.Magenta;
            _colorDisplay.Color = value.Value;
        }
    }
}