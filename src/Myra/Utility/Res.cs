using System.IO;
using System.Reflection;

namespace Myra.Utility
{
	/// <summary>
	/// Resource utility
	/// </summary>
	public static class Res
	{
		/// <summary>
		/// Open assembly resource stream by relative name
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public static Stream OpenResourceStream(this Assembly assembly, string assetName)
		{
			var path = assembly.GetName().Name + "." + assetName;

			// Once you figure out the name, pass it in as the argument here.
			var stream = assembly.GetManifestResourceStream(path);

			return stream;
		}

		/// <summary>
		/// Reads assembly resource as byte array by relative name
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public static byte[] ReadResourceAsBytes(this Assembly assembly, string assetName)
		{
			var ms = new MemoryStream();
			using (var input = assembly.OpenResourceStream(assetName))
			{
				input.CopyTo(ms);

				return ms.ToArray();
			}
		}

		/// <summary>
		/// Reads assembly resource as string by relative name
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public static string ReadResourceAsString(this Assembly assembly, string assetName)
		{
			string result;
			using (var input = assembly.OpenResourceStream(assetName))
			{
				using (var textReader = new StreamReader(input))
				{
					result = textReader.ReadToEnd();
				}
			}

			return result;
		}
	}
}
