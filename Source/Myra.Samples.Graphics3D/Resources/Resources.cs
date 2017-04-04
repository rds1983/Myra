using System.IO;
using System.Reflection;

namespace Myra.Samples.Graphics3D.Resources
{
	internal static class Resources
	{
		private const string Prefix = "Myra.Samples.Graphics3D.Resources.";

		internal static Stream GetBinaryResourceStream(string name)
		{
			var assembly = typeof(Resources).GetTypeInfo().Assembly;

			// Once you figure out the name, pass it in as the argument here.
			var stream = assembly.GetManifestResourceStream(Prefix + name);

			return stream;
		}
	}
}
