using FontStashSharp;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
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
			var formattedText = new FormattedText
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
