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
}
