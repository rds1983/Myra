using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class SliderTests
	{
		[Test]
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
			Assert.AreEqual(slider.Hint, 0);

			slider.Value = 2.0f;
			Assert.AreEqual(slider.Hint, slider.MaxHint);
		}
	}
}
