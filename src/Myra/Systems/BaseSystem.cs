using Myra.Graphics2D.UI;

namespace Myra.Systems
{
    public abstract class BaseSystem : ISystem
    {
        public Desktop Desktop { get; set; }
        
        public virtual void OnWidgetAddedToDesktop(Widget widget)
        {
            
        }

        public abstract void Update();
        
    }
}