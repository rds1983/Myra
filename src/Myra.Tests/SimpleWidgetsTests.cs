using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using NUnit.Framework;

namespace Myra.Tests
{
	[TestFixture]
	public class SimpleWidgetsTests
	{
		[Test]
		public void LabelTest()
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

		[Test]
		public void LabelTest2()
		{
			var root = Utility.LoadFromResourceRootClone("marginBorderPadding.xmmp");

			Assert.IsInstanceOf<Panel>(root);
			var panel1 = (Panel)root;

			Utility.AssertSolidBrush("#ADD8E6FF", panel1.Background);

			Assert.AreEqual(1, panel1.Widgets.Count);
			Assert.IsInstanceOf<Panel>(panel1.Widgets[0]);
			var panel2 = (Panel)panel1.Widgets[0];

			Assert.AreEqual(new Thickness(36), panel2.Margin);
			Utility.AssertSolidBrush("#008000", panel2.Border);
			Assert.AreEqual(new Thickness(12, 0), panel2.BorderThickness);
			Assert.AreEqual(new Thickness(20, 20, 0, 0), panel2.Padding);
			Utility.AssertSolidBrush("#FFA500FF", panel2.Background);
			Utility.AssertSolidBrush("#808000FF", panel2.OverBorder);

			Assert.AreEqual(1, panel2.Widgets.Count);
			Assert.IsInstanceOf<Label>(panel2.Widgets[0]);
			var label1 = (Label)panel2.Widgets[0];

			Assert.AreEqual("Some Text", label1.Text);
			Utility.AssertColor("#FF0000FF", label1.TextColor);
		}

		[Test]
		public void CheckButtonTest()
		{
			var root = Utility.LoadFromResourceRootClone("checkButton.xmmp");

			Assert.IsInstanceOf<Panel>(root);
			var panel = (Panel)root;

			Assert.AreEqual(1, panel.Widgets.Count);
			Assert.IsInstanceOf<CheckButton>(panel.Widgets[0]);
			var checkButton = (CheckButton)panel.Widgets[0];

			Assert.IsInstanceOf<VerticalStackPanel>(checkButton.Content);
			var stackPanel = (VerticalStackPanel)checkButton.Content;
			Assert.AreEqual(3, stackPanel.Widgets.Count);

			for(var i = 1; i <= 3; ++i)
			{
				Assert.IsInstanceOf<Label>(stackPanel.Widgets[i - 1]);
				var label = (Label)stackPanel.Widgets[i - 1];
				Assert.AreEqual("Text " + i, label.Text);
			}
		}

		[Test]
		public void NewButtonsTest()
		{
			var root = Utility.LoadFromResourceRootClone("newButtons.xmmp");

			Assert.IsInstanceOf<HorizontalStackPanel>(root);
			var rootPanel = (HorizontalStackPanel)root;

			Assert.AreEqual(2, rootPanel.Widgets.Count);
			Assert.IsInstanceOf<Button>(rootPanel.Widgets[0]);
			var button = (Button)rootPanel.Widgets[0];
			Assert.AreEqual(new Thickness(4), button.Margin);
			Utility.AssertSolidBrush("#4BD961FF", button.Border);
			Assert.AreEqual(new Thickness(1), button.BorderThickness);
			Assert.AreEqual(new Thickness(4), button.Padding);

			Assert.IsInstanceOf<VerticalStackPanel>(button.Content);
			var buttonStackPanel = (VerticalStackPanel)button.Content;
			Assert.AreEqual(4, buttonStackPanel.Widgets.Count);
			Assert.AreEqual(8, buttonStackPanel.Spacing);

			for (var i = 1; i <= buttonStackPanel.Widgets.Count; ++i)
			{
				Assert.IsInstanceOf<Label>(buttonStackPanel.Widgets[i - 1]);
				var label = (Label)buttonStackPanel.Widgets[i - 1];
				Assert.AreEqual("Test" + i, label.Text);
			}

			Assert.IsInstanceOf<ToggleButton>(rootPanel.Widgets[1]);
			var toggleButton = (ToggleButton)rootPanel.Widgets[1];
			Assert.AreEqual(new Thickness(4), toggleButton.Margin);
			Utility.AssertSolidBrush("#4BD961FF", toggleButton.Border);
			Assert.AreEqual(new Thickness(1), toggleButton.BorderThickness);
			Assert.AreEqual(new Thickness(4), toggleButton.Padding);

			Assert.IsInstanceOf<VerticalStackPanel>(toggleButton.Content);
			var toggleButtonStackPanel = (VerticalStackPanel)toggleButton.Content;

			Assert.AreEqual(8, toggleButtonStackPanel.Spacing);
			Assert.AreEqual(4, toggleButtonStackPanel.Widgets.Count);
			for (var i = 1; i <= toggleButtonStackPanel.Widgets.Count; ++i)
			{
				Assert.IsInstanceOf<Label>(toggleButtonStackPanel.Widgets[i - 1]);
				var label = (Label)toggleButtonStackPanel.Widgets[i - 1];
				Assert.AreEqual("Test" + i, label.Text);
			}
		}

		[Test]
		public void TextBoxTest()
		{
			var root = Utility.LoadFromResourceRootClone("scrolledTextField.xmmp");

			Assert.IsInstanceOf<Panel>(root);
			var panel = (Panel)root;

			Assert.AreEqual(1, panel.Widgets.Count);
			Assert.IsInstanceOf<TextBox>(panel.Widgets[0]);
			var textBox = (TextBox)panel.Widgets[0];

			Assert.AreEqual(HorizontalAlignment.Center, textBox.HorizontalAlignment);
			Assert.AreEqual(VerticalAlignment.Center, textBox.VerticalAlignment);
			Assert.AreEqual(100, textBox.Width);
		}
	}
}
