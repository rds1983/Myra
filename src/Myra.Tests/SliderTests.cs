using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Xunit;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class SliderTests
	{
		[Fact]
		public void TestMinMax()
		{
			var slider = new HorizontalSlider
			{
				Minimum = 0.5f,
				Maximum = 2.0f,
				Left = 10,
				Top = 10,
				Height = 20,
				Width = 100,
			};

			var desktop = new Desktop
			{
				BoundsFetcher = () => new Rectangle(0, 0, 640, 480)
			};

			desktop.Root = slider;

			desktop.Render();

			slider.Value = 0.5f;
			Assert.Equal(0, slider.Hint);

			slider.Value = 2.0f;
			Assert.Equal(slider.Hint, slider.MaxHint);
		}
	}
}


