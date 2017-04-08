using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Tiled;

namespace Myra.Assets
{
	public class TmxTilesetLoader : IAssetLoader<TmxTileset>
	{
		public TmxTileset Load(AssetLoaderContext context, string assetName)
		{
			var text = context.ReadAsText(assetName);
			var xDocument = XDocument.Parse(text);
			var result = new TmxTileset(xDocument, context.Load<Texture2D>);

			return result;
		}
	}
}