using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.Tiles
{
	public class Tile
	{
		public TextureRegion2D Region { get; set; }
		public Point Size { get; set; }
		public Point Offset { get; set; }
	}
}
