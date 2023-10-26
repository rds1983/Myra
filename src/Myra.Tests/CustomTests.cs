using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Myra.Tests
{
	[TestFixture]
	public class CustomTests
	{
		/// <summary>
		/// Ensures exception is thrown if a label is created without default style set in the stylesheet
		/// </summary>
		[Test]
		public void NoDefaultStyleLabel()
		{
			// Store current stylesheet
			var oldStylesheet = Stylesheet.Current.Clone();

			// Remove all styles, including default one from the stylesheet
			Stylesheet.Current.LabelStyles.Clear();

			Assert.Throws<Exception>(() =>
			{
				var label = new Label("blue");
			});

			// Restore the stylesheet for other tests to work
			Stylesheet.Current = oldStylesheet;
		}

		/// <summary>
		/// Tests that stylesheet is correctly recreated after MyraEnvironment.Reset() call
		/// </summary>
		[Test]
		public void MyraEnvironmentReset()
		{
			var oldStylesheet = Stylesheet.Current;
			var button = new Button();
			var oldTexture = ((TextureRegion)button.Background).Texture;

			MyraEnvironment.Reset();

			var newStylesheet = Stylesheet.Current;
			button = new Button();
			var newTexture = ((TextureRegion)button.Background).Texture;

			Assert.AreNotEqual(oldStylesheet, newStylesheet);
			Assert.AreNotEqual(oldTexture, newTexture);
		}

		[Test]
		public void TestZIndexSort()
		{
			var widgets = new List<Widget>();
			var button1 = new Button
			{
				ZIndex = 10
			};
			var button2 = new Button
			{
				ZIndex = 6
			};
			var button3 = new Button
			{
				ZIndex = 4
			};
			var button4 = new Button
			{
				ZIndex = 2
			};

			widgets.Add(button1);
			widgets.Add(button2);
			widgets.Add(button3);
			widgets.Add(button4);

			widgets.SortWidgetsByZIndex();

			// The list should become reversed
			Assert.AreEqual(widgets[0], button4);
			Assert.AreEqual(widgets[1], button3);
			Assert.AreEqual(widgets[2], button2);
			Assert.AreEqual(widgets[3], button1);
		}
	}
}
