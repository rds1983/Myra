using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Myra.Content.TextureAtlases
{
    public class TextureAtlasRegionContent
    {
        public string Name { get; set; }
        public Rectangle Bounds { get; set; }
        public Thickness? NinePatchInfo { get; set; }
    }
}