using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using NUnit.Framework;
using System;

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
			var button = new TextButton();
			var oldTexture = ((TextureRegion)button.Background).Texture;

			MyraEnvironment.Reset();

			var newStylesheet = Stylesheet.Current;
			button = new TextButton();
			var newTexture = ((TextureRegion)button.Background).Texture;

			Assert.AreNotEqual(oldStylesheet, newStylesheet);
			Assert.AreNotEqual(oldTexture, newTexture);
		}
	}
}
