using System.IO;
using System.Reflection;
using System;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;
using Microsoft.Xna.Framework;
using Xunit;
using Myra.Graphics2D.Brushes;
using FontStashSharp.RichText;
using AssetManagementBase;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace Myra.Tests
{
	internal static class Utility
	{
		public static readonly Assembly Assembly = typeof(Utility).Assembly;
		private const float FloatEpsilon = 1e-6f;

		public static Widget LoadProjectRootClone(string name)
		{
			var assetManager = CreateAssetManager();
			var project = assetManager.LoadProject(name);

			return project.Root.Clone();
		}

		/// <summary>
		/// Creates an AssetManager that loads files from the Assets folder in the test output directory.
		/// </summary>
		/// <returns>A file-based AssetManager pointing to the Assets folder.</returns>
		public static AssetManager CreateAssetManager()
		{
			var assetPath = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");
			return AssetManager.CreateFileAssetManager(assetPath);
		}

		public static void AssertSolidBrush(Color color, IBrush brush)
		{
			Assert.IsType<SolidBrush>(brush);
			var solidBrush = (SolidBrush)brush;

			Assert.Equal(color, solidBrush.Color);
		}

		public static void AssertSolidBrush(string colorName, IBrush brush)
		{
			var color = ColorStorage.FromName(colorName);
			AssertSolidBrush(color.Value, brush);
		}

		public static void AssertColor(string colorName, Color color)
		{
			var color2 = ColorStorage.FromName(colorName);
			Assert.Equal(color2.Value, color);
		}

		public static void AssertEqualEpsilon(float expected, float actual)
		{
			Assert.Equal(expected, actual, FloatEpsilon);
		}

		/// <summary>
		/// Normalizes XML by sorting attributes to ensure consistent ordering regardless of serialization order.
		/// </summary>
		public static XElement NormalizeXml(XElement element)
		{
			return new XElement(element.Name,
				element.Attributes().OrderBy(a => a.Name.LocalName),
				element.Elements().Select(NormalizeXml));
		}

		/// <summary>
		/// Asserts that two XML documents are semantically equal, ignoring attribute order and formatting.
		/// </summary>
		public static void AssertXmlEqual(XDocument a, XDocument b)
		{
			var normalizedA = NormalizeXml(a.Root).ToString();
			var normalizedB = NormalizeXml(b.Root).ToString();
			Assert.Equal(normalizedA, normalizedB);
		}

		/// <summary>
		/// Recursively collects all widgets of a specific type from a widget hierarchy.
		/// </summary>
		public static List<T> CollectWidgetsByType<T>(Widget parent) where T : Widget
		{
			var results = new List<T>();
			CollectWidgetsByTypeRecursive(parent, results);
			return results;
		}

		private static void CollectWidgetsByTypeRecursive<T>(Widget parent, List<T> results) where T : Widget
		{
			if (parent is T widget)
			{
				results.Add(widget);
			}

			foreach (var child in parent.Children)
			{
				CollectWidgetsByTypeRecursive(child, results);
			}
		}
	}
}
