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

	public static float ParseFloat(this Dictionary<string, object> data, string key, float def = 0.0f)
	{
		object obj;
		if (!data.TryGetValue(key, out obj))
		{
			return def;
		}

		return float.Parse(obj.ToString());
	}

	public static Color ParseColor(this Dictionary<string, object> data)
	{
		return new Color(data.ParseFloat("r"),
			data.ParseFloat("g"),
			data.ParseFloat("b"),
			data.ParseFloat("a"));
	}

	public static string GetString(this Dictionary<string, object> data, string key) => data[key].ToString();
}
