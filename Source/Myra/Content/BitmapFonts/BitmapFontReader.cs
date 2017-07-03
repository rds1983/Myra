using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Myra.Graphics2D.Text;

namespace Myra.Content.BitmapFonts
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader input, BitmapFont existingInstance)
        {
            var s = input.ReadString();

            var result = BitmapFontHelper.LoadFromFnt(input.AssetName, s, t =>
            {
                if (!t.Contains(":"))
                {
                    // Font backing image lies on separate texture
                    return new TextureRegion2D(input.ContentManager.Load<Texture2D>(t));
                }
                else
                {
                    // Font backing image is texture region on atlas
                    var parts = t.Split(':');
                    var textureAtlas = input.ContentManager.Load<TextureAtlas>(parts[0]);

                    return textureAtlas[parts[1]];
                }
            });
            

            return result;
        }
    }
}