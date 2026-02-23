using System;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
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
        internal EditorTypeRegistry GetRegistry() => new EditorTypeRegistry(attached, editTypes);
    }
    
    /// <summary>
    /// Encapsulates a <see cref="Widget"/> (UI element) and <see cref="Record"/> (.Net property or field), for the purposes of display or editing by the user.
    /// </summary>
    public abstract class PropertyEditor : IRecordReference
    {
#region Statics
        private static readonly Type[] ActivatorTypeArgs = { typeof(IInspector), typeof(Record) };
        /// <summary>
        /// Initialize the <see cref="PropertyEditor"/>-<see cref="Type"/> relationship registry.
        /// </summary>
        /// <param name="predictedCount">Internal editor-array alloc size.</param>
        /// <param name="fromAssemblies">The assemblies which are scanned for concrete inheritors of <see cref="PropertyEditor"/>. Any <see cref="PropertyEditor"/> without <see cref="PropertyEditorAttribute"/> are ignored. Null will scan all assemblies in the current <see cref="AppDomain"/>.</param>
        public static void InitializeRegistry(int predictedCount = 16, params System.Reflection.Assembly[] fromAssemblies)
            => Editors.InitializeRegistry(predictedCount, fromAssemblies);
        public static bool TryCreate(IInspector inspector, Record bindProperty, out PropertyEditor result)
        {
            if (Editors.TryGetEditorTypeForType(bindProperty.Type, out Type editorType))
            {
                // TODO mayble cache a lookup of known constructor-info objects?
                var ctor = editorType.GetConstructor(ActivatorTypeArgs);
                if (ctor != null)
                {
                    try
                    {
                        //This also creates the widget
                        object obj = Activator.CreateInstance(editorType, inspector, bindProperty);
                        result = obj as PropertyEditor;
                        return result != null;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Activator Reflection Error: {e}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Could not find property editor for type: "+bindProperty.Type);
            }
            result = null;
            return false;
        }
#endregion
        protected delegate bool WidgetCreatorDelegate(out Widget widget);
        
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
            DoInit();
            if (TryCreateWidget(out Widget editor))
                Widget = editor;
        }
        
        private void DoInit() => Initialize();
        protected virtual void Initialize() { }

        private bool TryCreateWidget(out Widget widget) => TryCreateEditorWidget(out widget);
        protected abstract bool TryCreateEditorWidget(out Widget widget);
        
        public void SetValue(object field, object value)
        {
            _record.SetValue(field, value);
            _owner.FireChanged(_record.Name);
        }
        public abstract void SetWidgetValue(object value);
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
        
        protected sealed override bool TryCreateEditorWidget(out Widget widget)
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
            _record.SetValue(field, value);
            _owner.FireChanged(_record.Name);
        }
    }

    public abstract class StructPropertyEditor<T> : PropertyEditor<T>, IStructTypeRef<T> where T : struct
    {
        private bool _nullable;
        public bool IsNullable => _nullable;
        
        protected StructPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
        {
        }
        
        public sealed override void SetWidgetValue(object value)
        {
            if(value is T kind)
                SetWidgetValue(kind);
        }

        public abstract void SetWidgetValue(T? value);
    }
}