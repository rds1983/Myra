using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Myra.Samples.FantasyMapGenerator.Generation
{
	public class Generator
	{
		public const int MinimumHeight = -10000;
		public const int MaximumHeight = 10000;
		private const int MinimumIslandSize = 1000;
		private const int MinimumLakeSize = 300;
		private const int ThresholdTest = 1000;

		private int _landMinimum = 0;

		private static readonly Point[] _deltas = new Point[]{
			new Point(0, -1),
			new Point(-1, 0),
			new Point(1, 0),
			new Point(0, 1),
			new Point(-1, -1),
			new Point(1, -1),
			new Point(-1, 1),
			new Point(1, 1),
		};

		private static readonly float[,] _smoothMatrix = new float[,]{
			{0.1f, 0.1f, 0.1f},
			{0.1f, 0.2f, 0.1f},
			{0.1f, 0.1f, 0.1f}
		};

		private int _width, _height;
		private int[,] _heightMap;
		private bool[,] _islandMask;

		public int LandMinimum
		{
			get
			{
				return _landMinimum;
			}
		}

		private List<Point> Build(int x, int y, Func<Point, bool> addCondition)
		{
			// Clear mask
			List<Point> result = new List<Point>();

			Stack<Point> toProcess = new Stack<Point>();

			toProcess.Push(new Point(x, y));

			while (toProcess.Count > 0)
			{
				Point top = toProcess.Pop();

				if (top.X < 0 ||
						top.X >= _width ||
						top.Y < 0 ||
						top.Y >= _height ||
						_islandMask[top.Y, top.X] ||
						!addCondition(top))
				{
					continue;
				}

				result.Add(top);
				_islandMask[top.Y, top.X] = true;

				// Add adjancement tiles
				toProcess.Push(new Point(top.X - 1, top.Y));
				toProcess.Push(new Point(top.X, top.Y - 1));
				toProcess.Push(new Point(top.X + 1, top.Y));
				toProcess.Push(new Point(top.X, top.Y + 1));
			}

			return result;
		}

		private void ClearMask()
		{
			for (int y = 0; y < _height; ++y)
			{
				for (int x = 0; x < _width; ++x)
				{
					_islandMask[y, x] = false;
				}
			}
		}

		private static float DetermineLevel(Noise2D noise, float minimumLevel, float requiredProp)
		{
			float result = minimumLevel + 0.05f;

			while (result < 1.0f)
			{
				int c = 0;
				for (int y = 0; y < ThresholdTest; ++y)
				{
					for (int x = 0; x < ThresholdTest; ++x)
					{
						float n = noise.GetValue((float)x / (ThresholdTest - 1), (float)y / (ThresholdTest - 1));

						if (minimumLevel <= n && n < result)
						{
							++c;
						}
					}
				}

				float prop = (float)c / (ThresholdTest * ThresholdTest);
				if (prop >= requiredProp)
				{
					break;
				}

				result += 0.05f;
			}

			return result;
		}

		private static int NoiseToHeight(float n)
		{
			int h = MinimumHeight + (int)((MaximumHeight - MinimumHeight) * n);

			return h;
		}

		public int[,] Generate(int width, int height,
								int variability,
								float waterPercent,
								float landPercent,
								bool surroundedByWater,
								bool smooth,
								bool removeSmallIslands,
								bool removeSmallLakes)
		{
			this._width = width;
			this._height = height;

			_heightMap = new int[height, width];
			for (int y = 0; y < height; ++y)
			{
				for (var x = 0; x < width; ++x)
				{
					_heightMap[y, x] = MinimumHeight;
				}
			}

			_islandMask = new bool[height, width];

			var noise = new MidPointDisplacementNoise2D();

			// Generate
			noise.Variability = variability;
			noise.IsSurroundedByWater = surroundedByWater;
			noise.Generate();

			float waterLevel = DetermineLevel(noise, 0.0f, waterPercent);
			float landLevel = DetermineLevel(noise, waterLevel, landPercent);

			_landMinimum = NoiseToHeight(waterLevel);

			if (_landMinimum < MinimumHeight + 1)
			{
				_landMinimum = MinimumHeight + 1;
			}

			Console.WriteLine("landMinimum = {0}", _landMinimum);

			int w = 0, l = 0;
			for (int y = 0; y < height; ++y)
			{
				for (int x = 0; x < width; ++x)
				{
					float n = noise.GetValue((float)x / (width - 1), (float)y / (height - 1));

					int h = NoiseToHeight(n);

					if (h < _landMinimum)
					{
						++w;
					}
					else
					{
						++l;
					}


					_heightMap[y, x] = h;
				}
			}

			int tiles = width * height;

			Console.WriteLine("{0}% water, {1}% land",
					w * 100 / tiles,
					l * 100 / tiles);

			// Smooth
			if (smooth)
			{
				var oldHeightMap = new int[height, width];
				for (int y = 0; y < height; ++y)
				{
					for (int x = 0; x < width; ++x)
					{
						oldHeightMap[y, x] = _heightMap[y, x];
					}
				}

				for (int y = 0; y < height; ++y)
				{
					for (int x = 0; x < width; ++x)
					{
						float newValue = 0;

						for (int k = 0; k < _deltas.Length; ++k)
						{
							int dx = x + _deltas[k].X;
							int dy = y + _deltas[k].Y;

							if (dx < 0 || dx >= width ||

									dy < 0 || dy >= height)
							{
								continue;
							}

							float value = _smoothMatrix[_deltas[k].Y + 1, _deltas[k].X + 1] * oldHeightMap[dy, dx];
							newValue += value;
						}

						newValue += _smoothMatrix[1, 1] * oldHeightMap[y, x];
						_heightMap[y, x] = (int)newValue;
					}
				}
			}

			if (removeSmallIslands)
			{
				ClearMask();

				// Next run remove small islands
				for (int y = 0; y < height; ++y)
				{
					for (int x = 0; x < width; ++x)
					{
						if (!_islandMask[y, x] && _heightMap[y, x] >= _landMinimum)
						{
							List<Point> island = Build(x, y, p => _heightMap[p.Y, p.X] >= _landMinimum);

							if (island.Count < MinimumIslandSize)
							{
								// Remove small island
								foreach (var p in island)
								{
									_heightMap[p.Y, p.X] = _landMinimum - 1;
								}
							}
						}
					}
				}
			}

			if (removeSmallLakes)
			{
				ClearMask();

				// Remove small lakes
				for (int y = 0; y < height; ++y)
				{
					for (int x = 0; x < width; ++x)
					{
						if (!_islandMask[y, x] && _heightMap[y, x] < _landMinimum)
						{
							List<Point> lake = Build(x, y, p => _heightMap[p.Y, p.X] < _landMinimum);

							if (lake.Count < MinimumLakeSize)
							{
								// Remove small lake
								foreach (var p in lake)
								{
									_heightMap[p.Y, p.X] = _landMinimum + 1;
								}
							}
						}
					}

				}
			}

			return _heightMap;
		}
	}
}