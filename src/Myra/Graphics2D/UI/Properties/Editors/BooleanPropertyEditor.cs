using System;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
{
    [PropertyEditor(typeof(BooleanPropertyEditor), typeof(bool))]
    public sealed class BooleanPropertyEditor : StructPropertyEditor<bool>, IStructTypeRef<bool>
    {
        private readonly bool _hasSetter;
        private readonly bool _nullable;
        public bool IsNullable => _nullable;
        
        public BooleanPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
            Type type = methodInfo.Type;
            _nullable = TypeHelper.GetNullableTypeOrPassThrough(ref type);
            _hasSetter = methodInfo.HasSetter;
        }
        
        protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
        {
            creatorDelegate = CreateCheckBox;
            return true;
        }

        private bool CreateCheckBox(out Widget widget)
        {
            if (_owner.SelectedField == null)
            {
                widget = null;
                return false;
            }
            
            var cb = new CheckButton
            {
                IsChecked = GetValue(_owner.SelectedField)
            };
            
            if (_record.HasSetter)
            {
                cb.Click += (sender, args) =>
                {
                    SetValue(_owner.SelectedField, !cb.IsChecked);
                };
            }
            else
            {
                cb.Enabled = false;
            }

            Widget = widget = cb;
            return true;
        }
        
        public override void SetWidgetValue(bool? value)
        {
            var cb = Widget as CheckButton;
            if (!_nullable && !value.HasValue)
                value = false;
            cb.IsChecked = value.Value;
        }
    }
}