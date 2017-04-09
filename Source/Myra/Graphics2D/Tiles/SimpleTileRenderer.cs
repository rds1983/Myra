using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.Tiles
{
	public class SimpleTileRenderer
	{
		public void RenderLayer(SpriteBatch batch, TileLayer layer)
		{
			var tileSize = layer.TileSize;
			if (tileSize.X == 0 || tileSize.Y == 0)
			{
				throw new Exception("TileSize couldn't have zero dimension");
			}

			var viewPortRect = new Rectangle(batch.GraphicsDevice.Viewport.X,
				batch.GraphicsDevice.Viewport.Y,
				batch.GraphicsDevice.Viewport.Width,
				batch.GraphicsDevice.Viewport.Height);

			for (var x = viewPortRect.X/tileSize.X; x < Math.Min(viewPortRect.Right/tileSize.X, layer.Size.X); ++x)
			{
				for (var y = viewPortRect.Y/tileSize.Y; y < Math.Min(viewPortRect.Bottom/tileSize.Y, layer.Size.Y); ++y)
				{
					var tiles = layer.GetTilesAt(x, y);
					foreach (var tile in tiles)
					{
						if (tile == null)
						{
							continue;
						}

						var destX = x*tileSize.X + layer.Offset.X + tile.Offset.X;
						var destY = y*tileSize.Y + layer.Offset.Y + tile.Offset.Y;
						var destRect = new Rectangle(destX, destY, tile.Size.X, tile.Size.Y);

						batch.Draw(tile.Region, destRect, Color.White*layer.Opacity);
					}
				}
			}
		}

		public void Render(SpriteBatch batch, TileMap tileMap)
		{
			foreach (var layer in tileMap.Layers)
			{
				RenderLayer(batch, layer);
			}
		}
	}
}