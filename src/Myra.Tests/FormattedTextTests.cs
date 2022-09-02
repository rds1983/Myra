using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System.Linq;

namespace Myra.Tests
{
	[TestFixture]
	public class FormattedTextTests
	{
		private SpriteFontBase _font;

		private SpriteFontBase Font
		{
			get
			{
				if (_font == null)
				{
					_font = DefaultAssets.UIStylesheet.Fonts.Values.First();
				}

				return _font;
			}
		}

		[Test]
		public void MeasureUtf32DoesNotThrow()
		{
			var formattedText = new RichTextLayout
			{
				Font = Font,
				Text = "🙌h📦e l👏a👏zy"
			};

			var size = Point.Zero;
			Assert.DoesNotThrow(() =>
			{
				size = formattedText.Size;
			});

			Assert.GreaterOrEqual(size.X, 0);
			Assert.GreaterOrEqual(size.Y, 0);
		}
	}
}
