using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Myra.Content.TextureAtlases;
using Myra.Graphics2D;

namespace Myra.Content.Pipeline.TextureAtlas
{
    [ContentTypeWriter]  
    public class TextureAtlasWriter : ContentTypeWriter<TextureAtlasContent>
    {
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "MonoGame.Extended.TextureAtlases.TextureAtlas, MonoGame.Extended";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
			return "Myra.Content.TextureAtlases.TextureAtlasReader, Myra";
        }

        protected override void Write(ContentWriter writer, TextureAtlasContent data)
        {
            var assetName = Path.GetFileNameWithoutExtension(data.Name);
            Debug.Assert(assetName != null, "assetName != null");

            writer.Write(assetName);
            writer.Write(data.Regions.Length);

            foreach (var region in data.Regions)
            {
                writer.Write(region.Name);
                writer.Write(region.Bounds.X);
                writer.Write(region.Bounds.Y);
                writer.Write(region.Bounds.Width);
                writer.Write(region.Bounds.Height);

                if (region.NinePatchInfo.HasValue)
                {
                    writer.Write(true);
                    var ninePatchInfo = region.NinePatchInfo.Value;
                    writer.Write(ninePatchInfo.Left);
                    writer.Write(ninePatchInfo.Right);
                    writer.Write(ninePatchInfo.Top);
                    writer.Write(ninePatchInfo.Bottom);
                }
                else
                {
                    writer.Write(false);                    
                }
            }
        }
    }
}