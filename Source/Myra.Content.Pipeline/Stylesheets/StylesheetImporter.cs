using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Newtonsoft.Json.Linq;

namespace Myra.Content.Pipeline.Stylesheets
{
    [ContentImporter(".json", DefaultProcessor = "StylesheetProcessor",
        DisplayName = "Myra UI Stylesheet - Myra")]
    public class StylesheetImporter : ContentImporter<StylesheetContent>
    {
        public override StylesheetContent Import(string filename, ContentImporterContext context)
        {
            string data;
            using (var streamReader = new StreamReader(filename))
            {
                data = streamReader.ReadToEnd();
            }

            var content = new StylesheetContent
            {

                Root = JObject.Parse(data)
            };

            return content;
        }
    }
}