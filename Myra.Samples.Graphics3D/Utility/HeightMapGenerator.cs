using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Graphics3D.Terrain;
using Myra.Utility;

namespace Myra.Samples.Graphics3D.Utility
{
	public class HeightMapGenerator
	{
		private const float Roughness = 4.0f;

		private readonly int _columns, _rows;
		private readonly float _minimumDepth, _maximumDepth;
		private readonly Random _random = new Random();

		private int _stepsCount, _currentStep;

		public Action<ProgressInfo> ProgressReporter;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="columns"></param>
		/// <param name="rows"></param>
		/// <param name="minimumDepth"></param>
		/// <param name="maximumDepth"></param>
		public HeightMapGenerator(int columns, int rows, float minimumDepth, float maximumDepth)
		{
			if (columns <= 0)
			{
				throw new ArgumentOutOfRangeException("columns");
			}

			if (rows <= 0)
			{
				throw new ArgumentOutOfRangeException("rows");
			}

			if (minimumDepth > maximumDepth)
			{
				throw new ArgumentException("minimumDepth > maximumDepth");
			}

			_columns = columns;
			_rows = rows;
			_minimumDepth = minimumDepth;
			_maximumDepth = maximumDepth;
		}

		private float Displace(float smallSize)
		{
			var Max = smallSize/_columns*Roughness;
			var r = (float) _random.NextDouble();
			return (r - 0.5f)*Max;
		}

		private static float Saturate(float value)
		{
			var result = value;
			if (result < 0.0f)
			{
				result = 0.0f;
			}

			if (result > 1.0f)
			{
				result = 1.0f;
			}

			return result;
		}

		private void DiamondSquare(HeightMap heightMap)
		{
			// Add top process rect
			var firstRect = new Rectangle(0, 0, heightMap.Columns - 1, heightMap.Rows - 1);
			var rects = new Queue<Rectangle>();

			rects.Enqueue(firstRect);
			while (rects.Count > 0)
			{
				var rect = rects.Dequeue();


				if (rect.Width > 1 || rect.Height > 1)
				{
					// Get edges heights
					var topLeft = heightMap[rect.X, rect.Y];
					var topRight = heightMap[rect.X + rect.Width, rect.Y];
					var bottomLeft = heightMap[rect.X, rect.Y + rect.Height];
					var bottomRight = heightMap[rect.X + rect.Width, rect.Y + rect.Height];

					// Average
					var average = topLeft +
					              topRight +
					              bottomLeft +
					              bottomRight;
					average /= 4.0f;

					var displace = Displace(rect.Width);

					var halfWidth = rect.Width >> 1;
					var halfHeight = rect.Height >> 1;
					var centerX = rect.X + halfWidth;
					var centerY = rect.Y + halfHeight;

					heightMap[centerX, centerY] = Saturate(average + displace);

					// Diamond
					// Left
					average = (topLeft + bottomLeft)/2.0f;
					displace = Displace(rect.Width);
					heightMap[rect.X, centerY] = Saturate(average + displace);
					// Top
					average = (topLeft + topRight)/2.0f;
					displace = Displace(rect.Width);
					heightMap[centerX, rect.Y] = Saturate(average + displace);
					// Right
					average = (topRight + bottomRight)/2.0f;
					displace = Displace(rect.Width);
					heightMap[rect.X + rect.Width, centerY] = Saturate(average + displace);
					// Bottom
					average = (bottomLeft + bottomRight)/2.0f;
					displace = Displace(rect.Width);
					heightMap[centerX, rect.Y + rect.Height] = Saturate(average + displace);

					// Add rects to process
					// Left-top
					rects.Enqueue(new Rectangle(rect.X, rect.Y, halfWidth, halfHeight));
					// Right-top
					rects.Enqueue(new Rectangle(rect.X + halfWidth, rect.Y, rect.Width - halfWidth, halfHeight));
					// Left-bottom
					rects.Enqueue(new Rectangle(rect.X, rect.Y + halfHeight, halfWidth, rect.Height - halfHeight));
					// Right-bottom
					rects.Enqueue(new Rectangle(rect.X + halfWidth, rect.Y + halfHeight, rect.Width - halfWidth,
						rect.Height - halfHeight));
				}
				else
				{
					++_currentStep;
					if (_stepsCount <= 0) continue;

					var a = ProgressReporter;
					if (a != null)
					{
						a(new ProgressInfo
						{
							Finished = false,
							Progress = (float) _currentStep/_stepsCount
						});
					}
				}
			}
		}

		public HeightMap Generate()
		{
			var heightMap = new HeightMap(_columns + 1, _rows + 1);

			heightMap[0, 0] = 0;
			heightMap[heightMap.Columns - 1, 0] = 0;
			heightMap[0, heightMap.Rows - 1] = 0;
			heightMap[heightMap.Columns - 1, heightMap.Rows - 1] = 0;

			_stepsCount = heightMap.Columns*heightMap.Rows;

			DiamondSquare(heightMap);

			// Apply heights
			for (var y = 0; y < heightMap.Rows; ++y)
			{
				for (var x = 0; x < heightMap.Columns; ++x)
				{
					var h = heightMap[x, y];
					heightMap[x, y] = _minimumDepth + h*(_maximumDepth - _minimumDepth);
				}
			}

			var a = ProgressReporter;
			if (a != null)
			{
				a(new ProgressInfo
				{
					Finished = true,
					Progress = 1.0f
				});
			}

			return heightMap;
		}
	}
}
