using System;
using System.IO;
using System.Reflection;

namespace Myra.Utility
{
	public static class PathUtils
	{
		public static string ExecutingAssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().Location;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		public static string TryToMakePathRelativeTo(string path, string pathRelativeTo)
		{
			try
			{
				var fullPathUri = new Uri(path, UriKind.Absolute);

				if (!pathRelativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()))
				{
					pathRelativeTo += Path.DirectorySeparatorChar;
				}
				var folderPathUri = new Uri(pathRelativeTo, UriKind.Absolute);

				path = folderPathUri.MakeRelativeUri(fullPathUri).ToString();
			}
			catch (Exception)
			{
			}

			return path;
		}
	}
}
