using XNAssets;
using System.Xml.Linq;
using System;
using Myra.Tools.ToMyraAtlasConverter;
using System.IO;

#if !STRIDE
using Microsoft.Xna.Framework.Graphics;
#else
using Texture2D = Stride.Graphics.Texture;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	internal class TextureRegionAtlasLoader : IAssetLoader<TextureRegionAtlas>
	{
		public TextureRegionAtlas Load(AssetLoaderContext context, string assetName)
		{
			var data = context.Load<string>(assetName);
			bool isXml;
			try
			{
				var xDoc = XDocument.Parse(data);
				isXml = true;
			}
			catch(Exception)
			{
				isXml = false;
			}

			if (isXml)
			{
				return TextureRegionAtlas.FromXml(data, name => context.Load<Texture2D>(name));
			}

			return Gdx.FromGDX(data, name => context.Load<Texture2D>(name));
		}
	}
}