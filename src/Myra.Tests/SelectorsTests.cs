using AssetManagementBase;
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
			Assert.AreEqual(6, listView.Widgets.Count);

			Assert.IsInstanceOf<Label>(listView.Widgets[0]);
			var label1 = (Label)listView.Widgets[0];
			Assert.AreEqual("test", label1.Text);

			Assert.IsInstanceOf<HorizontalSeparator>(listView.Widgets[1]);

			Assert.IsInstanceOf<Label>(listView.Widgets[2]);
			var label2 = (Label)listView.Widgets[2];
			Assert.AreEqual("test2", label2.Text);

			Assert.IsInstanceOf<VerticalStackPanel>(listView.Widgets[3]);
			var verticalStackPanel1 = (VerticalStackPanel)listView.Widgets[3];
			Assert.AreEqual(2, verticalStackPanel1.Widgets.Count);

			Assert.IsInstanceOf<Label>(verticalStackPanel1.Widgets[0]);
			var label3 = (Label)verticalStackPanel1.Widgets[0];
			Assert.AreEqual("test3", label3.Text);

			Assert.IsInstanceOf<Label>(verticalStackPanel1.Widgets[1]);
			var label4 = (Label)verticalStackPanel1.Widgets[1];
			Assert.AreEqual("test4", label4.Text);

			Assert.IsInstanceOf<HorizontalSeparator>(listView.Widgets[4]);

			Assert.IsInstanceOf<VerticalStackPanel>(listView.Widgets[5]);
			var verticalStackPanel2 = (VerticalStackPanel)listView.Widgets[5];
			Assert.AreEqual(3, verticalStackPanel2.Widgets.Count);

			Assert.IsInstanceOf<Label>(verticalStackPanel2.Widgets[0]);
			var label5 = (Label)verticalStackPanel2.Widgets[0];
			Assert.AreEqual("test5", label5.Text);

			Assert.IsInstanceOf<Label>(verticalStackPanel2.Widgets[1]);
			var label6 = (Label)verticalStackPanel2.Widgets[1];
			Assert.AreEqual("test6", label6.Text);

			Assert.IsInstanceOf<Label>(verticalStackPanel2.Widgets[2]);
			var label7 = (Label)verticalStackPanel2.Widgets[2];
			Assert.AreEqual("test7", label7.Text);
		}

		[Test]
		public void ComboViewTest()
		{
			var assetManager = AssetManager.CreateResourceAssetManager(Utility.Assembly, "Resources.");
			var mml = assetManager.ReadAsString("comboView.xmmp");
			var project = Project.LoadFromXml(mml, assetManager);
			var root = project.Root;

			Assert.IsInstanceOf<Panel>(root);
			var panel = (Panel)root;

			Assert.AreEqual(1, panel.Widgets.Count);
			Assert.IsInstanceOf<ComboView>(panel.Widgets[0]);
			var comboView = (ComboView)panel.Widgets[0];

			Assert.AreEqual(4, comboView.Widgets.Count);

			Assert.IsInstanceOf<Label>(comboView.Widgets[0]);
			var label1 = (Label)comboView.Widgets[0];
			Assert.AreEqual("Test", label1.Text);

			Assert.IsInstanceOf<HorizontalStackPanel>(comboView.Widgets[1]);
			var horizontalStackPanel = (HorizontalStackPanel)comboView.Widgets[1];
			Assert.AreEqual(2, horizontalStackPanel.Widgets.Count);

			Assert.IsInstanceOf<Image>(horizontalStackPanel.Widgets[0]);
			var image1 = (Image)horizontalStackPanel.Widgets[0];
			Assert.AreEqual(16, image1.Width);
			Assert.AreEqual(16, image1.Height);
			Assert.IsNotNull(image1.Renderable);
			Assert.AreEqual(64, image1.Renderable.Size.X);
			Assert.AreEqual(64, image1.Renderable.Size.Y);

			Assert.IsInstanceOf<Label>(horizontalStackPanel.Widgets[1]);
			var label2 = (Label)horizontalStackPanel.Widgets[1];
			Assert.AreEqual("Test2", label2.Text);

			Assert.IsInstanceOf<HorizontalSeparator>(comboView.Widgets[2]);

			Assert.IsInstanceOf<Label>(comboView.Widgets[3]);
			var label3 = (Label)comboView.Widgets[3];
			Assert.AreEqual("Test3", label3.Text);
		}
	}
}
