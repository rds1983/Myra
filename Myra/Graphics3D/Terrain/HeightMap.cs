namespace Myra.Graphics3D.Terrain
{
	public class HeightMap
	{
		private readonly float[,] _heights;

		public float[,] Heights
		{
			get { return _heights; }
		}

		public int Columns
		{
			get { return _heights.GetLength(0); }
		}

		public int Rows
		{
			get { return _heights.GetLength(1); }
		}

		public float this[int col, int row]
		{
			get { return _heights[col, row]; }

			set { _heights[col, row] = value; }
		}

		public HeightMap(int columns, int rows)
		{
			_heights = new float[columns, rows];
		}

		public float GetHeightAt(int column, int row)
		{
			if (column < 0)
			{
				column = 0;
			}
			if (column >= Columns)
			{
				column = Columns - 1;
			}

			if (row < 0)
			{
				row = 0;
			}
			if (row >= Rows)
			{
				row = Rows - 1;
			}

			return _heights[column, row];
		}

		public void SetHeightAt(int column, int row, float height)
		{
			_heights[column, row] = height;
		}
	}
}