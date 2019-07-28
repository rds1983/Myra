namespace Myra.Utility
{
	internal static class StringUtils
	{
		public static int Length(this string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0;
			}

			return s.Length;
		}
	}
}
