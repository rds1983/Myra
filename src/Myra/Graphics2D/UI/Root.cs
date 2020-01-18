using System;
using System.Collections.Generic;
using System.Linq;

namespace Myra.Graphics2D.UI
{
    internal class Root : Grid
    {
        public Widget FindWidget(Func<Widget, bool> Filter)
        {
            return FindInCollection(Widgets.ToList(), Filter);
        }
        private Widget FindInCollection(List<Widget> collection, Func<Widget, bool> Filter) {

            foreach (var item in collection)
            {
                if (Filter(item))
                {
                    return item;
                }
                if (item is IMultipleItemsContainer)
                {
                    var i2 = FindInCollection((item as IMultipleItemsContainer).Widgets.ToList(),Filter);
                    if (i2 != null)
                        return i2;
                }
            }
            return null;
        }

        public Widget GetWidgetByID(string id)
        {
            return FindWidget(w => w.Id == id);
        }
    }
}
