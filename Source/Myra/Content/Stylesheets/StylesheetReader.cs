using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json.Linq;

namespace Myra.Content.Stylesheets
{
    public class StylesheetReader : ContentTypeReader<Stylesheet>
    {
        protected override Stylesheet Read(ContentReader input, Stylesheet existingInstance)
        {
            var s = input.ReadString();
            
            var root = JObject.Parse(s);
            
            string atlasName;
            if (!root.GetStyle("textureAtlas", out atlasName))
            {
                throw new ContentLoadException("Could not determine texture atlas name");
            }

            var atlas = input.ContentManager.Load<TextureAtlas>(atlasName);

            var loader = new StylesheetLoader(root, t =>
                {
                    if (string.IsNullOrEmpty(t))
                    {
                        return null;
                    }

                    return atlas[t];
                },
                f =>
                {
                    if (string.IsNullOrEmpty(f))
                    {
                        return null;
                    }

                    return input.ContentManager.Load<BitmapFont>(f);
                });

            var result = loader.Load();

            return result;
        }
    }
}