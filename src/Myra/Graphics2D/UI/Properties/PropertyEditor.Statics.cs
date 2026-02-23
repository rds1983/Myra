using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Myra.Utility;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
{
    public partial class PropertyEditor
    {
        // Statics/internals for PropertyEditor types
        
        private static readonly Type[] ActivatorTypeArgs = { typeof(IInspector), typeof(Record) };
        private static ReadOnlyCollection<Registry> _registry;
        private static bool _init;
        
        public static bool TryCreate(IInspector inspector, Record bindProperty, out PropertyEditor result)
        {
            if (TryGetEditorTypeForType(bindProperty.Type, out Type editorType))
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
        
        private static bool TryGetEditorTypeForType(Type propertyKind, out Type editorType)
        {
            if (!_init)
                InitializeRegistry();

            foreach (Registry reg in _registry)
            {
                if (reg.CanEditType(propertyKind))
                {
                    if (reg.IsOpenGenericType)
                    {
                        TypeHelper.GetNullableTypeOrPassThrough(ref propertyKind);
                        editorType = reg.EditorType.MakeGenericType(propertyKind);
                    }
                    else
                    {
                        editorType = reg.EditorType;
                    }
                    return true;
                }
            }

            editorType = null;
            return false;
        }
        
        /// <summary>
        /// Initialize the <see cref="PropertyEditor"/>-<see cref="Type"/> relationship registry.
        /// </summary>
        /// <param name="predictedCount">Internal editor-array alloc size.</param>
        /// <param name="fromAssemblies">The assemblies which are scanned for concrete inheritors of <see cref="PropertyEditor"/>. Any <see cref="PropertyEditor"/> without <see cref="PropertyEditorAttribute"/> are ignored. Null will scan all assemblies in the current <see cref="AppDomain"/>.</param>
        public static void InitializeRegistry(int predictedCount = 16, params Assembly[] fromAssemblies)
        {
            Assembly[] scanAsm;
            if (fromAssemblies == null || fromAssemblies.Length <= 0)
                scanAsm = AppDomain.CurrentDomain.GetAssemblies(); //this scans way more assemblies than needed, but works
            else
            {
                scanAsm = fromAssemblies;
                if(!fromAssemblies.Contains(typeof(PropertyEditor).Assembly))
                    Console.WriteLine("PropertyEditor.InitializeRegistry() warning: Myra's Assembly was not included.");
                //maybe ensure fromAssemblies contains Myra (this) assembly?
            }
            
            Reflective_LoadTypeRegistry(predictedCount, scanAsm, out List<PropertyEditor.Registry> registry);

            _registry = registry.AsReadOnly();
            _init = true;
        }
        
        /// <summary>
        /// Generate <see cref="PropertyEditor.Registry"/> objects for each concrete <see cref="PropertyEditor"/> implementation found.
        /// That implementation must also have a <see cref="PropertyEditorAttribute"/>.
        /// </summary>
        private static void Reflective_LoadTypeRegistry(int predictedCount, IEnumerable<Assembly> assemblies, out List<Registry> registry)
        {
            registry = new List<Registry>(predictedCount);
            foreach (Assembly asm in assemblies)
            {
                foreach (Type type in asm.GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(PropertyEditor)) ))
                {
                    PropertyEditorAttribute att = type.FindAttribute<PropertyEditorAttribute>();
                    if (att == null)
                        continue;
                    Registry reg = att.GetRegistry();
                    if(reg == null)
                        continue;
                    registry.Add(reg);
                }
            }
        }
    }
}