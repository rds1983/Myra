using System;
using System.Collections.Generic;
using System.Linq;
using Myra.Graphics2D.UI;

namespace Myra.Editor.Plugin
{
    public class WidgetTypesSet
    {
        private readonly HashSet<Type> _widgets = new HashSet<Type>();

        public Type[] Types
        {
            get { return _widgets.ToArray(); }
        }

        public void AddWidgetType<T>() where T : Widget
        {
            _widgets.Add(typeof(T));
        }
        
        public void RemoveWidgetType<T>() where T : Widget
        {
            _widgets.Remove(typeof(T));
        }
    }
}