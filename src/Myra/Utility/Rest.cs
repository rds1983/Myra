using System;

namespace Myra.Utility
{
	internal static class Rest
	{
		public static object[][] CloneArray(this object[][] source)
		{
			var result = new object[source.Length][];

			for (var i = 0; i < source.Length; ++i)
			{
				result[i] = new object[source[i].Length];
				Array.Copy(source[i], result[i], source[i].Length);
			}

			return result;
		}

		public static void SortByColumn(this object[][] source, int columnIndex, bool ascending)
		{
			// Sort
			Array.Sort(source, (a, b) =>
			{
				var compare = ((IComparable)a[columnIndex]).CompareTo(b[columnIndex]);

				return ascending ? compare : -compare;
			});
		}
	}
}
