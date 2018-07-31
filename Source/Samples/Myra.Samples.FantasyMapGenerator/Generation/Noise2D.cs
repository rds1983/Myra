using System;

namespace Myra.Samples.FantasyMapGenerator.Generation
{
	public abstract class Noise2D
	{
		public abstract void Generate();

		/// <summary>
		///  Returns value in range [0, 1]
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract float GetValue(float x, float y);

		protected float LerpCoord(float c)
		{
			if (c < 0)
			{
				c = Math.Abs(c);

				// Retrieve fractional part
				int i = (int)c;
				float f = c - i;

				return 1.0f - f;
			}

			if (c > 1.0f)
			{
				// Retrieve fractional part
				int i = (int)c;
				float f = c - i;

				return f;
			}

			return c;
		}
	}
}
