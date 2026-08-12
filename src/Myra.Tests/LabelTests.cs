using Xunit;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class LabelTests
	{
		[Fact]
		public void LoadLabelFromXmmp_LoadsProperties()
		{
			var root = Utility.LoadProjectRootClone("labelWithPaddings.xmmp");

			Assert.IsType<Panel>(root);
			var panel = (Panel)root;

			Assert.Single(panel.Widgets);
			Assert.IsType<Label>(panel.Widgets[0]);
			var label = (Label)panel.Widgets[0];

			Assert.Equal("StbImageSharp", label.Text);
			Assert.Equal(new Thickness(8), label.Margin);
			Utility.AssertSolidBrush("#808000FF", label.Border);
			Assert.Equal(new Thickness(8), label.BorderThickness);
			Assert.Equal(new Thickness(16), label.Padding);
			Utility.AssertSolidBrush("#008000FF", label.Background);
		}
	}
}




