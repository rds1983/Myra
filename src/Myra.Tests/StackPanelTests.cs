using Myra.Graphics2D.UI;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class StackPanelTests: BaseTests
	{
		[Test]
		public void AddRemove()
		{
			var stackPanel = new VerticalStackPanel();
			var label1 = new Label();
			var label2 = new Label();
			var label3 = new Label();

			stackPanel.AddChild(label1);
			stackPanel.AddChild(label2);
			stackPanel.AddChild(label3);

			stackPanel.RemoveChild(label2);

			Assert.AreEqual(2, stackPanel.Widgets.Count);
			Assert.AreEqual(label1, stackPanel.Widgets[0]);
			Assert.AreEqual(label3, stackPanel.Widgets[1]);
		}
	}
}
