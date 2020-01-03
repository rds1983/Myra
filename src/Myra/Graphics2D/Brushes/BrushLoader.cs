using Myra.Assets;
using System;

namespace Myra.Graphics2D.Brushes
{
	internal class BrushLoader : IAssetLoader<IBrush>
	{
		public IBrush Load(AssetLoaderContext context, string assetName)
		{
			var color = ColorStorage.FromName(assetName);
			if (color == null)
			{
				throw new Exception(string.Format("Unable to resolve color '{0}'", assetName));
			}

			return new SolidBrush(color.Value);
		}
	}
}