using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class GridTests
	{
		private static Project LoadFromResource(string name)
		{
			var xml = Utility.ReadResourceAsString("Resources.GridTests." + name);

			return Project.LoadFromXml(xml);
		}

		[Test]
		public static void TestSimpleProportionsPart()
		{
			var project = LoadFromResource("SimpleProportionsPart.xmmp");
			var grid = (Grid)project.Root;

			grid.Arrange(new Rectangle(0, 0, 400, 400));

			Assert.AreEqual(100, grid.Widgets[0].ContainerBounds.Width);
			Assert.AreEqual(200, grid.Widgets[0].ContainerBounds.Height);
			Assert.AreEqual(300, grid.Widgets[1].ContainerBounds.Width);
			Assert.AreEqual(200, grid.Widgets[1].ContainerBounds.Height);
			Assert.AreEqual(100, grid.Widgets[2].ContainerBounds.Width);
			Assert.AreEqual(200, grid.Widgets[2].ContainerBounds.Height);
			Assert.AreEqual(300, grid.Widgets[3].ContainerBounds.Width);
			Assert.AreEqual(200, grid.Widgets[3].ContainerBounds.Height);
		}

		[Test]
		public static void TestSimpleAutoFill()
		{
			var project = LoadFromResource("SimpleAutoFill.xmmp");
			var grid = (Grid)project.Root;

			grid.Arrange(new Rectangle(0, 0, 400, 500));

			Assert.AreEqual(100, grid.Widgets[0].ContainerBounds.Width);
			Assert.AreEqual(450, grid.Widgets[0].ContainerBounds.Height);
			Assert.AreEqual(300, grid.Widgets[1].ContainerBounds.Width);
			Assert.AreEqual(450, grid.Widgets[1].ContainerBounds.Height);
			Assert.AreEqual(100, grid.Widgets[2].ContainerBounds.Width);
			Assert.AreEqual(50, grid.Widgets[2].ContainerBounds.Height);
			Assert.AreEqual(300, grid.Widgets[3].ContainerBounds.Width);
			Assert.AreEqual(50, grid.Widgets[3].ContainerBounds.Height);
		}
	}
}
