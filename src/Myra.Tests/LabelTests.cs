using NUnit.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D;

namespace Myra.Tests
{
	[TestFixture]
	public class LabelTests
	{
		[Test]
		public void LoadLabelFromXmmp_LoadsProperties()
		{
			var root = Utility.LoadFromResourceRootClone("labelWithPaddings.xmmp");

			Assert.IsInstanceOf<Panel>(root);
			var panel = (Panel)root;

			Assert.AreEqual(1, panel.Widgets.Count);
			Assert.IsInstanceOf<Label>(panel.Widgets[0]);
			var label = (Label)panel.Widgets[0];

			Assert.AreEqual("StbImageSharp", label.Text);
			Assert.AreEqual(new Thickness(8), label.Margin);
			Utility.AssertSolidBrush("#808000FF", label.Border);
			Assert.AreEqual(new Thickness(8), label.BorderThickness);
			Assert.AreEqual(new Thickness(16), label.Padding);
			Utility.AssertSolidBrush("#008000FF", label.Background);
		}
	}
}
