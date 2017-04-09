using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;

namespace Myra.Graphics2D.Tiled
{
	public class SimpleTiledRenderer
	{
		public void RenderLayer(SpriteBatch batch, TmxMap map, TmxLayer layer)
		{
			var viewPortRect = new Rectangle(batch.GraphicsDevice.Viewport.X,
				batch.GraphicsDevice.Viewport.Y,
				batch.GraphicsDevice.Viewport.Width,
				batch.GraphicsDevice.Viewport.Height);

			for (var x = viewPortRect.X/map.TileWidth; x < Math.Min(viewPortRect.Right/map.TileWidth, layer.Width); ++x)
			{
				for (var y = viewPortRect.Y/map.TileHeight; y < Math.Min(viewPortRect.Bottom/map.TileHeight, layer.Height); ++y)
				{
					var tile = layer[x, y];

					if (tile == null || tile.Region == null)
					{
						continue;
					}

					var destX = x*map.TileWidth + layer.Offset.X + tile.Offset.X;
					var destY = y*map.TileHeight + layer.Offset.Y + tile.Offset.Y;
					var destRect = new Rectangle(destX, destY, tile.Size.X, tile.Size.Y);

					batch.Draw(tile.Region, destRect, Color.White*layer.Opacity);
				}
			}

		}

		public void Render(SpriteBatch batch, TmxMap tileMap)
		{
			foreach (var layer in tileMap.Layers)
			{
				RenderLayer(batch, tileMap, layer);
			}
		}
	}
}