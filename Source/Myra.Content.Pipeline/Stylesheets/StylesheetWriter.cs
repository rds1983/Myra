using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Myra.Content.Pipeline.Stylesheets
{
    [ContentTypeWriter]  
    public class StylesheetWriter : ContentTypeWriter<StylesheetContent>
    {
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Myra.Graphics2D.UI.Styles.Stylesheet, Myra";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Myra.Content.Stylesheets.StylesheetReader, Myra";
        }

        protected override void Write(ContentWriter writer, StylesheetContent data)
        {
            var s = data.Root.ToString();
            writer.Write(s);
        }
    }
}