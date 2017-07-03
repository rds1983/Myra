using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Myra.Graphics2D.Text;

namespace Myra.Content.Pipeline.BitmapFonts
{
    [ContentImporter(".fnt", DefaultProcessor = "BitmapFontProcessor",
        DisplayName = "BMFont Importer - Myra")]
    public class BitmapFontImporter : ContentImporter<BitmapFontContent>
    {
        public override BitmapFontContent Import(string filename, ContentImporterContext context)
        {
            string s;
            using (var streamReader = new StreamReader(filename))
            {
                s = streamReader.ReadToEnd();
            }
            
            BitmapFontHelper.Validate(s);

            var result = new BitmapFontContent
            {
                Data = s
            };

            return result;
        }
    }
}