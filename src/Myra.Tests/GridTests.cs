using AssetManagementBase;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Xunit;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class GridTests
	{
		private static Project LoadFromResource(string name)
		{
			var assetManager = Utility.CreateAssetManager();
			return assetManager.LoadProject("GridTests/" + name);
		}

		[Fact]
		public static void TestSimpleProportionsPart()
		{
			var project = LoadFromResource("SimpleProportionsPart.xmmp");
			var grid = (Grid)project.Root;

			grid.Arrange(new Rectangle(0, 0, 400, 400));

			Assert.Equal(100, grid.Widgets[0].ContainerBounds.Width);
			Assert.Equal(200, grid.Widgets[0].ContainerBounds.Height);
			Assert.Equal(300, grid.Widgets[1].ContainerBounds.Width);
			Assert.Equal(200, grid.Widgets[1].ContainerBounds.Height);
			Assert.Equal(100, grid.Widgets[2].ContainerBounds.Width);
			Assert.Equal(200, grid.Widgets[2].ContainerBounds.Height);
			Assert.Equal(300, grid.Widgets[3].ContainerBounds.Width);
			Assert.Equal(200, grid.Widgets[3].ContainerBounds.Height);
		}

		[Fact]
		public static void TestSimpleAutoFill()
		{
			var project = LoadFromResource("SimpleAutoFill.xmmp");
			var grid = (Grid)project.Root;

			grid.Arrange(new Rectangle(0, 0, 400, 500));

			Assert.Equal(100, grid.Widgets[0].ContainerBounds.Width);
			Assert.Equal(450, grid.Widgets[0].ContainerBounds.Height);
			Assert.Equal(300, grid.Widgets[1].ContainerBounds.Width);
			Assert.Equal(450, grid.Widgets[1].ContainerBounds.Height);
			Assert.Equal(100, grid.Widgets[2].ContainerBounds.Width);
			Assert.Equal(50, grid.Widgets[2].ContainerBounds.Height);
			Assert.Equal(300, grid.Widgets[3].ContainerBounds.Width);
			Assert.Equal(50, grid.Widgets[3].ContainerBounds.Height);
		}
	}
}



