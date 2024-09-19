using System.IO;
using System.Reflection;
using System;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using Myra.Graphics2D.Brushes;
using FontStashSharp.RichText;

namespace Myra.Tests
{
	internal static class Utility
	{
		public static readonly Assembly Assembly = typeof(Utility).Assembly;

		/// <summary>
		/// Open assembly resource stream by relative name
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static Stream OpenResourceStream(string path)
		{
			// Once you figure out the name, pass it in as the argument here.
			path = Assembly.GetName().Name + "." + path;
			var stream = Assembly.GetManifestResourceStream(path);
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
		public static byte[] ReadResourceAsBytes(string path)
		{
			var ms = new MemoryStream();
			using (var input = OpenResourceStream(path))
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
		public static string ReadResourceAsString(string path)
		{
			string result;
			using (var input = OpenResourceStream(path))
			{
				using (var textReader = new StreamReader(input))
				{
					result = textReader.ReadToEnd();
				}
			}

			return result;
		}

		public static Project LoadFromResource(string name)
		{
			var xml = Utility.ReadResourceAsString("Resources." + name);

			return Project.LoadFromXml(xml);
		}

		public static Widget LoadFromResourceRootClone(string name)
		{
			var project = LoadFromResource(name);

			return project.Root.Clone();
		}

		public static void AssertSolidBrush(Color color, IBrush brush)
		{
			Assert.IsInstanceOf<SolidBrush>(brush);
			var solidBrush = (SolidBrush)brush;

			Assert.AreEqual(color, solidBrush.Color);
		}

		public static void AssertSolidBrush(string colorName, IBrush brush)
		{
			var color = ColorStorage.FromName(colorName);
			AssertSolidBrush(color.Value, brush);
		}

		public static void AssertColor(string colorName, Color color)
		{
			var color2 = ColorStorage.FromName(colorName);
			Assert.AreEqual(color2.Value, color);
		}
	}
}
