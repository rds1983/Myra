using Microsoft.Xna.Framework.Content.Pipeline;
using Myra.Content.Pipeline.Stylesheets;
using Myra.Graphics2D;

namespace Myra.Content.Pipeline.Stylesheet
{
    [ContentProcessor(DisplayName = "JSON StylesheetContent Processor - Myra")]   
    public class StylesheetProcessor: ContentProcessor<StylesheetContent, StylesheetContent>
    {
        public override StylesheetContent Process(StylesheetContent input, ContentProcessorContext context)
        {
            return input;
        }
    }
}