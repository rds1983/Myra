using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using NUnit.Framework;
using System;

namespace Myra.Tests
{
	[TestFixture]
	public class CustomTests: BaseTests
	{
		/// <summary>
		/// Ensures exception is thrown if a label is created without default style set in the stylesheet
		/// </summary>
		[Test]
		public void NoDefaultStyleLabel()
		{
			// Remove all styles, including default one from the stylesheet
			Stylesheet.Current.LabelStyles.Clear();

			Assert.Throws<Exception>(() =>
			{
				var label = new Label("blue");
			});
		}
	}
}
