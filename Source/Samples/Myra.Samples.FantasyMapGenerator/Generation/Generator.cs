using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Myra.Samples.FantasyMapGenerator.Generation
{
	public class Generator
	{
		private const int MinimumIslandSize = 1000;
		private const int MinimumLakeSize = 300;

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

		private int _size = 0;
		private float _landMinimum = 0;
		private readonly Random _random = new Random();
		private bool[,] _isSet;
		private float[,] _data;
		private bool _firstDisplace = true;

		private bool[,] _islandMask;

		public float LandMinimum
		{
			get
			{
				return _landMinimum;
			}
		}

		public int Size
		{
			get
			{
				return _size;
			}
		}

		public float Variability { get; set; }
		public bool IsSurroundedByWater { get; set; }

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
						top.X >= _size ||
						top.Y < 0 ||
						top.Y >= _size ||
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
			_islandMask.Fill(false);
		}

		private float DetermineLevel(float minimumLevel, float requiredProp)
		{
			float result = minimumLevel + 0.05f;

			while (result < 1.0f)
			{
				int c = 0;
				for (int y = 0; y < _size; ++y)
				{
					for (int x = 0; x < _size; ++x)
					{
						float n = GetData(x, y);

						if (minimumLevel <= n && n < result)
						{
							++c;
						}
					}
				}

				float prop = (float)c / (_size * _size);
				if (prop >= requiredProp)
				{
					break;
				}

				result += 0.05f;
			}

			return result;
		}

		private float Displace(float average, float d)
		{
			if (IsSurroundedByWater && _firstDisplace)
			{
				_firstDisplace = false;
				return 1.0f;
			}

			float p = (float)_random.NextDouble() - 0.5f;
			float result = (average + d * p);

			return result;
		}

		private float GetData(int x, int y)
		{
			return _data[y, x];
		}

		private void SetDataIfNotSet(int x, int y, float value)
		{
			if (_isSet[y, x])
			{
				return;
			}

			_data[y, x] = value;

			_isSet[y, x] = true;
		}

		private void MiddlePointDisplacement(int left, int top, int right, int bottom, float d)
		{
			int localWidth = right - left + 1;
			int localHeight = bottom - top + 1;

			if (localWidth <= 2 && localHeight <= 2)
			{
				return;
			}

			// Retrieve corner heights
			float heightTopLeft = GetData(left, top);
			float heightTopRight = GetData(right, top);
			float heightBottomLeft = GetData(left, bottom);
			float heightBottomRight = GetData(right, bottom);
			float average = (heightTopLeft + heightTopRight + heightBottomLeft + heightBottomRight) / 4;

			// Calculate center
			int centerX = left + localWidth / 2;
			int centerY = top + localHeight / 2;

			// Square step
			float centerHeight = Displace(average, d);
			SetDataIfNotSet(centerX, centerY, centerHeight);

			// Diamond step
			SetDataIfNotSet(left, centerY, (heightTopLeft + heightBottomLeft + centerHeight) / 3);
			SetDataIfNotSet(centerX, top, (heightTopLeft + heightTopRight + centerHeight) / 3);
			SetDataIfNotSet(right, centerY, (heightTopRight + heightBottomRight + centerHeight) / 3);
			SetDataIfNotSet(centerX, bottom, (heightBottomLeft + heightBottomRight + centerHeight) / 3);

			// Sub-recursion
			float div = 1.0f + (10.0f - Variability) / 10.0f;

			d /= div;

			MiddlePointDisplacement(left, top, centerX, centerY, d);
			MiddlePointDisplacement(centerX, top, right, centerY, d);
			MiddlePointDisplacement(left, centerY, centerX, bottom, d);
			MiddlePointDisplacement(centerX, centerY, right, bottom, d);
		}

		public void GenerateHeightMap()
		{
			// Set initial values
			if (!IsSurroundedByWater)
			{
				SetDataIfNotSet(0, 0, (float)_random.NextDouble());
				SetDataIfNotSet(Size - 1, 0, (float)_random.NextDouble());
				SetDataIfNotSet(0, Size - 1, (float)_random.NextDouble());
				SetDataIfNotSet(Size - 1, Size - 1, (float)_random.NextDouble());
			}
			else
			{
				SetDataIfNotSet(0, 0, 0.0f);
				SetDataIfNotSet(Size - 1, 0, 0.0f);
				SetDataIfNotSet(0, Size - 1, 0.0f);
				SetDataIfNotSet(Size - 1, Size - 1, 0.0f);
			}

			// Plasma
			MiddlePointDisplacement(0, 0, Size - 1, Size - 1, 1.0f);

			// Determine min & max
			float? min = null, max = null;
			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					float v = GetData(x, y);

					if (min == null || v < min)
					{
						min = v;
					}

					if (max == null || v > max)
					{
						max = v;
					}
				}
			}

			// Normalize
			float delta = max.Value - min.Value;
			for (int y = 0; y < Size; ++y)
			{
				for (int x = 0; x < Size; ++x)
				{
					float v = GetData(x, y);

					v -= min.Value;

					if (delta > 1.0f)
					{
						v /= delta;
					}

					_data[y, x] = v;
				}
			}
		}

		public float[,] Generate(int size,
								int variability,
								float waterPart,
								bool surroundedByWater,
								bool smooth,
								bool removeSmallIslands,
								bool removeSmallLakes)
		{

			_size = size;

			_islandMask = new bool[size, size];

			Variability = variability;
			IsSurroundedByWater = surroundedByWater;

			_data = new float[Size, Size];
			_isSet = new bool[Size, Size];
			_data.Fill(0.0f);
			_isSet.Fill(false);

			_firstDisplace = true;

			GenerateHeightMap();

			float waterLevel = DetermineLevel(0.0f, waterPart);
			float landLevel = DetermineLevel(waterLevel, (1.0f - waterPart));

			_landMinimum = waterLevel;

			if (_landMinimum < 0)
			{
				_landMinimum = 0;
			}

			// Calculate amount of water and land tiles
			int w = 0, l = 0;
			for (int y = 0; y < _size; ++y)
			{
				for (int x = 0; x < _size; ++x)
				{
					float n = GetData(x, y);

					if (n < _landMinimum)
					{
						++w;
					}
					else
					{
						++l;
					}
				}
			}

			int tiles = _size * _size;

			Console.WriteLine("{0}% water, {1}% land",
					w * 100 / tiles,
					l * 100 / tiles);

			// Smooth
			if (smooth)
			{
				var oldHeightMap = new float[_size, _size];
				for (int y = 0; y < _size; ++y)
				{
					for (int x = 0; x < _size; ++x)
					{
						oldHeightMap[y, x] = _data[y, x];
					}
				}

				for (int y = 0; y < _size; ++y)
				{
					for (int x = 0; x < _size; ++x)
					{
						float newValue = 0;

						for (int k = 0; k < _deltas.Length; ++k)
						{
							int dx = x + _deltas[k].X;
							int dy = y + _deltas[k].Y;

							if (dx < 0 || dx >= _size ||

									dy < 0 || dy >= _size)
							{
								continue;
							}

							float value = _smoothMatrix[_deltas[k].Y + 1, _deltas[k].X + 1] * oldHeightMap[dy, dx];
							newValue += value;
						}

						newValue += _smoothMatrix[1, 1] * oldHeightMap[y, x];
						_data[y, x] = newValue;
					}
				}
			}

			if (removeSmallIslands)
			{
				ClearMask();

				// Next run remove small islands
				for (int y = 0; y < _size; ++y)
				{
					for (int x = 0; x < _size; ++x)
					{
						if (!_islandMask[y, x] && _data[y, x] >= _landMinimum)
						{
							List<Point> island = Build(x, y, p => _data[p.Y, p.X] >= _landMinimum);

							if (island.Count < MinimumIslandSize)
							{
								// Remove small island
								foreach (var p in island)
								{
									_data[p.Y, p.X] = _landMinimum - 0.001f;
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
				for (int y = 0; y < _size; ++y)
				{
					for (int x = 0; x < _size; ++x)
					{
						if (!_islandMask[y, x] && _data[y, x] < _landMinimum)
						{
							List<Point> lake = Build(x, y, p => _data[p.Y, p.X] < _landMinimum);

							if (lake.Count < MinimumLakeSize)
							{
								// Remove small lake
								foreach (var p in lake)
								{
									_data[p.Y, p.X] = _landMinimum + 0.001f;
								}
							}
						}
					}

				}
			}

			return _data;
		}
	}
}