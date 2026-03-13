using System;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties.Editors
{
    /// <summary>
    /// Attribute that ties a concrete <see cref="PropertyEditor"/> to one or more property types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PropertyEditorAttribute : Attribute
    {
        private readonly Type attached;
        private readonly Type[] editTypes;
        
        public PropertyEditorAttribute(Type attached, params Type[] editTypes)
        {
            this.attached = attached;
            this.editTypes = editTypes;
        }
        internal PropertyEditor.Registry GetRegistry() => new PropertyEditor.Registry(attached, editTypes);
    }
    
    /// <summary>
    /// Encapsulates a <see cref="Widget"/> (UI element) and <see cref="Record"/> (.Net property or field), for the purposes of display or editing by the user.
    /// </summary>
    public abstract partial class PropertyEditor : IRecordReference
    {
        protected readonly IInspector _owner;
        protected readonly Record _record;
        
        public Type Type => _record.Type;
        public Widget Widget { get; protected set; }
        
        /// <summary>
        /// Creates a new widget attached to the given Record
        /// </summary>
        protected PropertyEditor(IInspector owner, Record methodInfo)
        {
            _owner = owner;
            _record = methodInfo ?? throw new NullReferenceException(nameof(methodInfo));
            _DoInit();
            if (_TryCreateWidget(out Widget editor))
                Widget = editor;
        }
        
        private void _DoInit() => Initialize();
        protected virtual void Initialize() { }

        private bool _TryCreateWidget(out Widget widget) => TryCreateWidget(out widget);
        /// <summary>
        /// Attempt to create a widget with the current setup.
        /// </summary>
        protected abstract bool TryCreateWidget(out Widget widget);
        /// <summary>
        /// Tell the widget to update the displayed data to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">data to set</param>
        public abstract void SetWidgetValue(object value);
        
        /// <summary>
        /// Changes the data in the record
        /// </summary>
        public void SetValue(object field, object value)
        {
            _record.SetValue(field, value);
            _owner.FireChanged(_record.Name);
        }
        
        /// <summary>
        /// Updates the widget's displayed value to be what's in the record
        /// </summary>
        public void UpdateDisplay()
        {
            if(Widget.Desktop.FocusedKeyboardWidget == Widget)
                return;
            SetWidgetValue(_record.GetValue(_owner.SelectedField));
        }

        object IRecordReference.GetValue(object field) => _record.GetValue(field);
        Record IRecordReference.Record => _record;
        bool IRecordReference.IsReadOnly => !_record.HasSetter;
    }
    
    /// <inheritdoc cref="PropertyEditor"/>
    public abstract class PropertyEditor<T> : PropertyEditor, IRecordReference<T>
    {
        protected PropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
        }
        
        protected sealed override bool TryCreateWidget(out Widget widget)
        {
            if (CreatorPicker(out var func))
                return func.Invoke(out widget);
            widget = null;
            return false;
        }
        
        protected abstract bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate);
        
        /// <summary>
        /// Gets the value from the field
        /// </summary>
        public virtual T GetValue(object field)
        {
            object o = _record.GetValue(field);
            if (o is T ot)
                return ot;
            return default;
        }
        /// <summary>
        /// Tries to set the value to to the field
        /// </summary>
        public virtual void SetValue(object field, T value)
        {
            base.SetValue(field, value);
        }
        
        /// <inheritdoc />
        public override void SetWidgetValue(object value)
        {
            if(value is T kind)
                SetWidgetValue(kind);
        }

        /// <inheritdoc cref="SetWidgetValue(object)"/>
        public abstract void SetWidgetValue(T value);
    }

    public abstract class StructPropertyEditor<T> : PropertyEditor<T>, IStructRecordReference<T> where T : struct
    {
        public bool IsNullable { get; }
        
        protected StructPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
            Type type = methodInfo.Type;
            IsNullable = TypeHelper.GetNullableTypeOrPassThrough(ref type);
        }
        
        /// <inheritdoc />
        public sealed override void SetWidgetValue(object value)
        {
            if(value == null)
                SetWidgetValue(null);
            if(value is T kind)
                SetWidgetValue(kind);
        }
        
        /// <inheritdoc />
        public sealed override void SetWidgetValue(T value) => SetWidgetValue(value);

        /// <inheritdoc cref="SetWidgetValue(object)"/>
        public abstract void SetWidgetValue(T? value);
    }
}