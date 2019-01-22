using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Attributes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Samples.RogueEditor.Data;
using Newtonsoft.Json;

namespace Myra.Samples.RogueEditor.UI
{
	public class MapEditor : Widget
	{
		public static readonly Point TileSize = new Point(32, 32);

		private Map _map;
		private Point _gridSize;
		private bool _isMouseDown = false;

		[JsonIgnore]
		[HiddenInEditor]
		public SpriteFont Font { get; set; }

		public Vector2 TopLeft { get; set; }

		[JsonIgnore]
		[HiddenInEditor]
		public Point? MarkPosition { get; set; }

		[JsonIgnore]
		[HiddenInEditor]
		public Point GridSize { get; private set; }

		[JsonIgnore]
		[HiddenInEditor]
		public Map Map
		{
			get { return _map; }

			set
			{
				if (value == _map)
				{
					return;
				}

				_map = value;
				InvalidateMeasure();
			}
		}

		public MapEditor()
		{
			Font = DefaultAssets.Font;

			CanFocus = false;
			ClipToBounds = true;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Map == null)
			{
				return;
			}

			Draw(context, Map, true);
		}

		public void Draw(RenderContext context, Map map, bool drawMark)
		{
			if (Font == null)
			{
				return;
			}

			var tileSize = TileSize;
			var gridSize = new Point(context.View.Width / tileSize.X,
									 context.View.Height / tileSize.Y);
			GridSize = gridSize;

			var mapViewPort = new Rectangle((int)TopLeft.X,
				(int)TopLeft.Y,
				gridSize.X + 1,
				gridSize.Y + 1);

			for (var mapY = mapViewPort.Y; mapY < mapViewPort.Bottom; ++mapY)
			{
				for (var mapX = mapViewPort.X; mapX < mapViewPort.Right; ++mapX)
				{
					if (mapX < 0 || mapX >= map.Size.X || mapY < 0 || mapY >= map.Size.Y)
						continue;

					var pos = new Point(mapX, mapY);
					var tile = map.GetTileAt(pos);

					var screen = GameToScreen(pos);

					var rect = new Rectangle(screen.X, screen.Y, TileSize.X, TileSize.Y);
					if (pos == MarkPosition)
					{
						context.Batch.FillRectangle(rect, Color.Blue);
					}

					if (tile != null)
					{
						var s = tile.Image.ToString();
						var sz = Font.MeasureString(s);

						var offset = new Vector2((TileSize.X - sz.X) / 2,
							(TileSize.Y - sz.Y) / 2);
						context.Batch.DrawString(Font,
							tile.Image.ToString(),
							screen.ToVector2() + offset,
							tile.Color);
					}
				}
			}
		}

		private void DrawAppearance(RenderContext context, Color color, TextureRegion image, Rectangle rect)
		{
			var pos = new Point(rect.X + (rect.Width - image.Bounds.Width) / 2,
				rect.Y + (rect.Height - image.Bounds.Height) / 2);

			context.Draw(image, pos, color);
		}

		private Point GameToScreen(Vector2 gamePosition)
		{
			return new Point((int)((gamePosition.X - TopLeft.X) * TileSize.X),
				(int)((gamePosition.Y - TopLeft.Y) * TileSize.Y));
		}

		private Point GameToScreen(Point gamePosition)
		{
			return new Point((int)((gamePosition.X - TopLeft.X) * TileSize.X),
				(int)((gamePosition.Y - TopLeft.Y) * TileSize.Y));
		}

		private Vector2 ScreenToGame(Point position)
		{
			var tileSize = TileSize;
			var tilePosition = new Vector2
			{
				X = TopLeft.X + position.X / tileSize.X,
				Y = TopLeft.Y + position.Y / tileSize.Y
			};

			return tilePosition;
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			if (Map == null)
			{
				return;
			}

			var gameCoords = ScreenToGame(Desktop.MousePosition);
			MarkPosition = new Point((int)gameCoords.X, (int)gameCoords.Y);

/*			if (Desktop.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				ProcessMouseDown();
			}*/
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			if (mb == MouseButtons.Left)
			{
				ProcessMouseDown();
			}
		}

		private void ProcessMouseDown()
		{
			var asTileInfo = Studio.Instance.Explorer.SelectedObject as TileInfo;
			if (asTileInfo == null)
			{
				return;
			}

			var pos = MarkPosition.Value;
			Map.Tiles[pos.X, pos.Y] = asTileInfo;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Map == null)
			{
				return Point.Zero;
			}

			var tileSize = TileSize;
			return new Point(Map.Size.X * tileSize.X,
				Map.Size.Y * tileSize.Y);
		}
	}
}