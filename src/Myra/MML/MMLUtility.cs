using Myra.Graphics2D.UI.Styles;
using System;

namespace Myra.MML
{
	internal static class MMLUtility
	{
		public static bool IsStyleable(this Type type)
		{
			// Check if widget constructor accepts a style name parameter (string)
			foreach (var c in type.GetConstructors())
			{
				var p = c.GetParameters();
				if (p != null && p.Length == 2)
				{
					if (p[0].ParameterType == typeof(Stylesheet) && p[1].ParameterType == typeof(string))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
