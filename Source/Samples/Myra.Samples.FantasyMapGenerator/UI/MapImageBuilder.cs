using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.TextureAtlases;
using Myra.Samples.FantasyMapGenerator.Generation;
using System;

namespace Myra.Samples.FantasyMapGenerator.UI
{
	public enum ViewMode
	{
		Normal,
		Siluette
	}

	public class MapImageBuilder
	{
		private static readonly Color SiluetteBackground = Color.White;
		private static readonly Color SiluetteBorder = Color.Black;

		private int[,] _heightMap;
		private ViewMode _viewMode = ViewMode.Normal;
		private int _landMinimum = 0;

		public ViewMode ViewMode
		{
			get
			{
				return _viewMode;
			}

			set
			{
				if (_viewMode == value)
				{
					return;
				}

				_viewMode = value;
			}
		}

		public int Height
		{
			get
			{
				return _heightMap.GetLength(0);
			}
		}

		public int Width
		{
			get
			{
				return _heightMap.GetLength(1);
			}
		}

		public void SetData(int[,] heightMap, int landMinimum)
		{
			if (heightMap == null)
			{
				throw new ArgumentNullException("heightMap");
			}

			this._heightMap = heightMap;
			this._landMinimum = landMinimum;
		}

		private static int Lerp(int val)
		{
			if (val < 0)
			{
				val = 0;
			}

			if (val > 255)
			{
				val = 255;
			}

			return val;
		}

		private Color ColorByHeight(int height)
		{
			Color result;

			if (height < _landMinimum)
			{
				float val = 1.0f - (height - _landMinimum) * 0.5f / (Generator.MinimumHeight - _landMinimum);

				// Water
				result = new Color((int)(30 * val), (int)(70 * val), (int)(200 * val));
			}
			else
			{
				// Land
				float val = 0.5f + (height - _landMinimum) * 0.5f / (Generator.MaximumHeight - _landMinimum);

				result = new Color((int)(160 * val), (int)(255 * val), (int)(102 * val));
			}

			return result;
		}

		private void BuildNormalImage(Color[] result)
		{
			for (int y = 0; y < Height; ++y)
			{
				for (int x = 0; x < Width; ++x)
				{
					int h = _heightMap[y, x];
					Color c = ColorByHeight(h);
					result[y * Width + x] = c;
				}
			}
		}

		private void SafeSetColor(Color[] image, int x, int y, Color color)
		{
			if (x < 0 || x >= Width ||
					y < 0 || y >= Height)
			{
				return;
			}

			image[y * Width + x] = color;
		}

		private void BuildSiluette(Color[] result)
		{
			// Fill with white
			result.Fill(SiluetteBackground);

			// Horizontal run
			for (int y = 0; y < Height; ++y)
			{
				bool isLand = false;
				for (int x = 0; x < Width; ++x)
				{
					int h = _heightMap[y, x];

					bool doSet = false;
					if (!isLand && h > _landMinimum)
					{
						doSet = true;
						isLand = true;
					}
					else if (isLand && h <= _landMinimum)
					{
						doSet = true;
						isLand = false;
					}

					if (doSet)
					{
						SafeSetColor(result, x, y, SiluetteBorder);
						SafeSetColor(result, x + 1, y, SiluetteBorder);
					}
				}
			}

			for (int x = 0; x < Width; ++x)
			{
				bool isLand = false;
				for (int y = 0; y < Height; ++y)
				{
					int h = _heightMap[y, x];

					bool doSet = false;
					if (!isLand && h > _landMinimum)
					{
						doSet = true;
						isLand = true;
					}
					else if (isLand && h <= _landMinimum)
					{
						doSet = true;
						isLand = false;
					}

					if (doSet)
					{
						SafeSetColor(result, x, y, SiluetteBorder);
						SafeSetColor(result, x, y + 1, SiluetteBorder);
					}
				}
			}
		}

		public TextureRegion BuildImage()
		{
			var result = new Color[Width * Height];

			switch (_viewMode)
			{
				case ViewMode.Normal:
					BuildNormalImage(result);
					break;
				case ViewMode.Siluette:
					BuildSiluette(result);
					break;
			}

			var texture = new Texture2D(MyraEnvironment.GraphicsDevice, Width, Height);
			texture.SetData(result);

			return new TextureRegion(texture);
		}
	}
}