namespace Myra.Samples.FantasyMapGenerator.Generation
{
	public static class Utils
	{
		public static void Fill<T>(this T[] array, T value)
		{
			for (var i = 0; i < array.Length; ++i)
			{
				array[i] = value;
			}
		}

		public static void Fill<T>(this T[,] array, T value)
		{
			for (var i = 0; i < array.GetLength(0); ++i)
			{
				for (var j = 0; j < array.GetLength(1); ++j)
				{
					array[i, j] = value;
				}
			}
		}
	}
}