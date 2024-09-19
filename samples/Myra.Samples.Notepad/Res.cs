using System.IO;
using System.Reflection;
using System;

namespace Myra.Samples.Notepad
{
	internal static class Res
	{
		/// <summary>
		/// Open assembly resource stream by relative name
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Stream OpenResourceStream(this Assembly assembly, string path)
		{
			// Once you figure out the name, pass it in as the argument here.
			var stream = assembly.GetManifestResourceStream(path);
			if (stream == null)
			{
				throw new Exception($"Could not find resource at path '{path}'");
			}

			return stream;
		}

		/// <summary>
		/// Reads assembly resource as byte array by relative name
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static byte[] ReadResourceAsBytes(this Assembly assembly, string path)
		{
			var ms = new MemoryStream();
			using (var input = assembly.OpenResourceStream(path))
			{
				input.CopyTo(ms);

				return ms.ToArray();
			}
		}

		/// <summary>
		/// Reads assembly resource as string by relative name
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ReadResourceAsString(this Assembly assembly, string path)
		{
			string result;
			using (var input = assembly.OpenResourceStream(path))
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
