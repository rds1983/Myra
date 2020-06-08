using Myra.Graphics2D.UI;

namespace Myra.Systems
{
    public interface ISystem
    {

        Desktop Desktop { get; set; }
        void OnWidgetAddedToDesktop(Widget widget);
        void Update();
        
    }
}