using System;
using Myra.Utility;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
{
    [PropertyEditor(typeof(EnumPropertyEditor<>), typeof(Enum))]
    public sealed class EnumPropertyEditor<TEnum> : StructPropertyEditor<TEnum> where TEnum : struct, Enum
    {
        private Array _values;
        private bool _nullable;
        
        public EnumPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
            if (!methodInfo.Type.IsEnum)
                throw new TypeLoadException($"Record is not an enum: {methodInfo.Type}");
        }
        
        protected override void Initialize()
        {
            Type type = typeof(TEnum);
            _nullable = TypeHelper.GetNullableTypeOrPassThrough(ref type);
            _values = Enum.GetValues(type);
        }

        protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
        {
            //TODO - support flags enums
            creatorDelegate = CreateComboView;
            return true;
        }

        private bool CreateComboView(out Widget widget)
        {
            if (_owner.SelectedField == null)
            {
                widget = null;
                return false;
            }
            
            var cv = new ComboView();
            
            if (_nullable)
            {
                cv.Widgets.Add(new Label
                {
                    Text = string.Empty,
                    Tag = null
                });
            }

            foreach (var v in _values)
            {
                cv.Widgets.Add(new Label
                {
                    Text = v.ToString(),
                    Tag = v
                });
            }

            object obj = _record.GetValue(_owner.SelectedField);
            
            int selectedIndex = Array.IndexOf(_values, obj);
            if (_nullable)
            {
                ++selectedIndex;
            }
            cv.SelectedIndex = selectedIndex;
            
            if (_record.HasSetter)
            {
                cv.SelectedIndexChanged += (sender, args) =>
                {
                    if (cv.SelectedIndex != -1)
                    { 
                        SetValue(_owner.SelectedField, cv.SelectedItem.Tag);
                    }
                };
            }
            else
            {
                cv.Enabled = false;
            }

            widget = cv;
            return true;
        }
        
        public override void SetWidgetValue(TEnum? value)
        {
            var cb = Widget as ComboView;
            int i = Array.IndexOf(_values, value);
            cb.SelectedIndex = i;
        }
    }
}