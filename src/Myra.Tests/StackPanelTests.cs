using Myra.Graphics2D.UI;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class StackPanelTests
	{
		[Test]
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

			Assert.AreEqual(2, stackPanel.Widgets.Count);
			Assert.AreEqual(label1, stackPanel.Widgets[0]);
			Assert.AreEqual(label3, stackPanel.Widgets[1]);
		}
	}
}
