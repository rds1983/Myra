using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class SelectorsTests
	{
		[Test]
		public void ListViewTest()
		{
			var root = Utility.LoadFromResourceRootClone("listView.xmmp");

			Assert.IsInstanceOf<Panel>(root);
			var panel = (Panel)root;

			Assert.AreEqual(1, panel.Widgets.Count);
			Assert.IsInstanceOf<ListView>(panel.Widgets[0]);
			var listView = (ListView)panel.Widgets[0];

			Assert.AreEqual(200, listView.Width);
			Assert.AreEqual(200, listView.Height);
			Assert.AreEqual(HorizontalAlignment.Center, listView.HorizontalAlignment);
			Assert.AreEqual(VerticalAlignment.Center, listView.VerticalAlignment);
			Assert.AreEqual(9, listView.Widgets.Count);

			Assert.IsInstanceOf<Label>(listView.Widgets[0]);
			var label1 = (Label)listView.Widgets[0];
			Assert.AreEqual("test", label1.Text);

			Assert.IsInstanceOf<HorizontalSeparator>(listView.Widgets[1]);
		}

		[Test]
		public void ComboViewTest()
		{

		}
	}
}
