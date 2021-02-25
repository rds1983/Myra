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
	}
}
