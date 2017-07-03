using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Myra.Content.Pipeline.BitmapFonts
{
    [ContentTypeWriter]     
    public class BitmapFontWriter : ContentTypeWriter<BitmapFontContent>
    {
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.BitmapFonts.BitmapFont, Myra";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Myra.Content.BitmapFonts.BitmapFontReader, Myra";
        }

        protected override void Write(ContentWriter writer, BitmapFontContent data)
        {
            writer.Write(data.Data);
        }
    }
}