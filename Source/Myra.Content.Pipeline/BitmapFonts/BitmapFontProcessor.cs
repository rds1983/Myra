using Microsoft.Xna.Framework.Content.Pipeline;

namespace Myra.Content.Pipeline.BitmapFonts
{
    [ContentProcessor(DisplayName = "BitmapFontContent Processor - Myra")]   
    public class BitmapFontProcessor: ContentProcessor<BitmapFontContent, BitmapFontContent>
    {
        public override BitmapFontContent Process(BitmapFontContent input, ContentProcessorContext context)
        {
            return input;
        }
    }
}