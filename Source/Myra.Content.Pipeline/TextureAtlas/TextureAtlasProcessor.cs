using Microsoft.Xna.Framework.Content.Pipeline;
using Myra.Content.TextureAtlases;
using Myra.Graphics2D;

namespace Myra.Content.Pipeline.TextureAtlas
{
    [ContentProcessor(DisplayName = "JSON StylesheetContent Processor - Myra")]   
    public class TextureAtlasProcessor: ContentProcessor<TextureAtlasContent, TextureAtlasContent>
    {
        public override TextureAtlasContent Process(TextureAtlasContent input, ContentProcessorContext context)
        {
            return input;
        }
    }
}