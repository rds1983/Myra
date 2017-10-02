using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Myra.Attributes;
using Myra.Graphics2D.UI;

namespace Myra.UIEditor.Plugin.LibGDX
{
    public class CustomWidget: Widget
    {
        [EditCategory("Appearance")]       
        public Color Color { get; set; }       
        
        public CustomWidget()
        {
            Color = Color.AliceBlue;
        }
        
        protected override Point InternalMeasure(Point availableSize)
        {
            return new Point(100, 100);
        }

        public override void InternalRender(RenderContext context)
        {
            base.InternalRender(context);

            var bounds = ActualBounds;
            context.Batch.FillRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height, Color);
        }
    }
}