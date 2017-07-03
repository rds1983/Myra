using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Content.TextureAtlases
{
    public class TextureAtlasContent
    {
        public string Name { get; set; }
        public TextureAtlasRegionContent[] Regions { get; set; }

        public TextureAtlas Create(Texture2D texture)
        {
            var result = new TextureAtlas(Name, texture);

            foreach (var sd in Regions)
            {
                var bounds = sd.Bounds;

                if (!sd.NinePatchInfo.HasValue)
                {
                    result.CreateRegion(sd.Name, bounds.X, bounds.Y, bounds.Width, bounds.Height);
                }
                else
                {
                    result.CreateNinePatchRegion(sd.Name, bounds.X, bounds.Y, bounds.Width, bounds.Height, sd.NinePatchInfo.Value);
                }
            }

            return result;            
        }
    }
}