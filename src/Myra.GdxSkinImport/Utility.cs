using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace GdxSkinImport;

internal static class Utility
{
	public static string Version
	{
		get
		{
			var assembly = typeof(Utility).Assembly;
			var name = new AssemblyName(assembly.FullName);

			return name.Version.ToString();
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

	public static Color ParseColor(this Dictionary<string, object> data)
	{
		return new Color(float.Parse(data["r"].ToString()),
			float.Parse(data["g"].ToString()),
			float.Parse(data["b"].ToString()),
			float.Parse(data["a"].ToString()));
	}
}
