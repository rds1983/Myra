using System.Collections.Generic;
using Myra.Assets;
using Myra.Attributes;

namespace Myra.Graphics2D.Tiles
{
	[AssetLoader(typeof(TileModelLoader))]
	public class TileMap
	{
		private readonly List<TileLayer> _layers = new List<TileLayer>();

		public List<TileLayer> Layers
		{
			get { return _layers; }
		}
	}
}
