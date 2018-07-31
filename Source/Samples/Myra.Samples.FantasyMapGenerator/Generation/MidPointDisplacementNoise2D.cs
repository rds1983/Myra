using System;

namespace Myra.Samples.FantasyMapGenerator.Generation
{
	public class MidPointDisplacementNoise2D : Noise2D
	{
		private const int Edge = 2048;

		private readonly Random _random = new Random();
		private readonly float[,] _data;
		private readonly bool[,] _isSet;

		public float Variability { get; set; }
		public bool IsSurroundedByWater { get; set; }
		private bool _firstDisplace = true;

		public MidPointDisplacementNoise2D()
		{
			Variability = 1.0f;
			IsSurroundedByWater = false;

			_data = new float[Edge, Edge];
			_isSet = new bool[Edge, Edge];
			Clear();
		}

		private void Clear()
		{
			_firstDisplace = true;

			_data.Fill(0.0f);
			_isSet.Fill(false);
		}

		public override float GetValue(float x, float y)
		{
			x = LerpCoord(x);
			y = LerpCoord(y);

			x *= (Edge - 1);
			y *= (Edge - 1);

			return _data[(int)y, (int)x];
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

		public override void Generate()
		{
			Clear();

			// Set initial values
			if (!IsSurroundedByWater)
			{
				SetDataIfNotSet(0, 0, (float)_random.NextDouble());
				SetDataIfNotSet(Edge - 1, 0, (float)_random.NextDouble());
				SetDataIfNotSet(0, Edge - 1, (float)_random.NextDouble());
				SetDataIfNotSet(Edge - 1, Edge - 1, (float)_random.NextDouble());
			}
			else
			{
				SetDataIfNotSet(0, 0, 0.0f);
				SetDataIfNotSet(Edge - 1, 0, 0.0f);
				SetDataIfNotSet(0, Edge - 1, 0.0f);
				SetDataIfNotSet(Edge - 1, Edge - 1, 0.0f);
			}

			// Plasma
			MiddlePointDisplacement(0, 0, Edge - 1, Edge - 1, 1.0f);

			// Determine min & max
			float? min = null, max = null;
			for (int y = 0; y < Edge; ++y)
			{
				for (int x = 0; x < Edge; ++x)
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
			for (int y = 0; y < Edge; ++y)
			{
				for (int x = 0; x < Edge; ++x)
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
	}
}