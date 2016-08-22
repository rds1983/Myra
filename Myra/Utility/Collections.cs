using System.Collections.Generic;

namespace Myra.Utility
{
	public static class Collections
	{
		public static T2 SafeGet<T1, T2>(this IDictionary<T1, T2> dictionary, T1 key)
		{
			T2 result;

			if (!dictionary.TryGetValue(key, out result))
			{
				result = default(T2);
			}

			return result;
		}
	}
}
