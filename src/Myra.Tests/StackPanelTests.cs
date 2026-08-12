using Myra.Graphics2D.UI;
using Xunit;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class StackPanelTests
	{
		[Fact]
		public void AddRemove()
		{
			var stackPanel = new VerticalStackPanel();
			var label1 = new Label();
			var label2 = new Label();
			var label3 = new Label();

			stackPanel.Widgets.Add(label1);
			stackPanel.Widgets.Add(label2);
			stackPanel.Widgets.Add(label3);

			stackPanel.Widgets.Remove(label2);

			Assert.Equal(2, stackPanel.Widgets.Count);
			Assert.Equal(label1, stackPanel.Widgets[0]);
			Assert.Equal(label3, stackPanel.Widgets[1]);
		}
	}
}


