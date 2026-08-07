using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.MML;
using System.Xml.Linq;
using Xunit;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class DimensionTests
	{
		[Fact]
		public void DefinesExpectedDimensionTypes()
		{
			Assert.Equal(DimensionType.Auto, Dimension.Auto.Type);
			Assert.Equal(DimensionType.Fill, Dimension.Fill.Type);
			Assert.Equal(new Dimension(DimensionType.Pixel, 32), Dimension.Pixel(32));
			Assert.Equal(new Dimension(DimensionType.Percent, 0.5f), Dimension.Percent(0.5f));
		}

		[Theory]
		[InlineData("Auto", DimensionType.Auto, 0.0f)]
		[InlineData("Fill", DimensionType.Fill, 0.0f)]
		[InlineData("24px", DimensionType.Pixel, 24.0f)]
		[InlineData("24", DimensionType.Pixel, 24.0f)]
		[InlineData("50%", DimensionType.Percent, 0.5f)]
		public void ParsesCommonDimensionStrings(string value, DimensionType expectedType, float expectedValue)
		{
			var dimension = Dimension.Parse(value);

			Assert.Equal(expectedType, dimension.Type);
			Assert.Equal(expectedValue, dimension.Value, 3);
		}

		[Fact]
		public void RegistersDimensionSerializerForMML()
		{
			var serializer = BaseContext.FindSerializer(typeof(Dimension));

			Assert.NotNull(serializer);
			Assert.Equal(Dimension.Pixel(32), serializer.Deserialize("32px"));
			Assert.Equal("25%", serializer.Serialize(Dimension.Percent(0.25f)));
		}

		[Fact]
		public void PixelDimensionsMeasureLikeLegacyWidthAndHeight()
		{
			var panel = new Panel
			{
				WidthDimension = Dimension.Pixel(80),
				HeightDimension = Dimension.Pixel(24)
			};

			Assert.Null(panel.Width);
			Assert.Null(panel.Height);
			Assert.Equal(new Point(80, 24), panel.Measure(new Point(500, 500)));
		}

		[Fact]
		public void LegacyWidthAndHeightStillMeasureAsPixels()
		{
			var panel = new Panel
			{
				Width = 80,
				Height = 24
			};

			Assert.Equal(Dimension.Auto, panel.WidthDimension);
			Assert.Equal(Dimension.Auto, panel.HeightDimension);
			Assert.Equal(new Point(80, 24), panel.Measure(new Point(500, 500)));
		}

		[Fact]
		public void LegacyWidthAndHeightClearExplicitDimensions()
		{
			var panel = new Panel
			{
				WidthDimension = Dimension.Pixel(80),
				HeightDimension = Dimension.Pixel(24)
			};

			panel.Width = 120;
			panel.Height = 36;

			Assert.Equal(Dimension.Auto, panel.WidthDimension);
			Assert.Equal(Dimension.Auto, panel.HeightDimension);
			Assert.Equal(new Point(120, 36), panel.Measure(new Point(500, 500)));
		}

		[Fact]
		public void AutoDimensionsRestoreContentMeasurement()
		{
			var panel = new Panel
			{
				Width = 120,
				Height = 36
			};
			panel.Widgets.Add(new Panel
			{
				Width = 80,
				Height = 24
			});

			panel.WidthDimension = Dimension.Auto;
			panel.HeightDimension = Dimension.Auto;

			Assert.Null(panel.Width);
			Assert.Null(panel.Height);
			Assert.Equal(new Point(80, 24), panel.Measure(new Point(500, 500)));
		}

		[Fact]
		public void FillDimensionsUseAvailableSize()
		{
			var panel = new Panel
			{
				WidthDimension = Dimension.Fill,
				HeightDimension = Dimension.Fill
			};

			Assert.Equal(new Point(200, 100), panel.Measure(new Point(200, 100)));

			panel.Arrange(new Rectangle(0, 0, 200, 100));

			Assert.Equal(new Rectangle(0, 0, 200, 100), panel.Bounds);
		}

		[Fact]
		public void PercentDimensionsUseAvailableSize()
		{
			var panel = new Panel
			{
				WidthDimension = Dimension.Percent(0.5f),
				HeightDimension = Dimension.Percent(0.25f)
			};

			Assert.Equal(new Point(100, 25), panel.Measure(new Point(200, 100)));

			panel.Arrange(new Rectangle(0, 0, 200, 100));

			Assert.Equal(new Rectangle(0, 0, 100, 25), panel.Bounds);
		}

		[Fact]
		public void CanLoadPixelDimensionsFromMML()
		{
			var project = Project.LoadFromXml("<Project><Panel WidthDimension=\"80px\" HeightDimension=\"24px\" /></Project>");
			var panel = Assert.IsType<Panel>(project.Root);

			Assert.Equal(Dimension.Pixel(80), panel.WidthDimension);
			Assert.Equal(Dimension.Pixel(24), panel.HeightDimension);
			Assert.Null(panel.Width);
			Assert.Null(panel.Height);
			Assert.Equal(new Point(80, 24), panel.Measure(new Point(500, 500)));
		}

		[Fact]
		public void CanLoadFillAndPercentDimensionsFromMML()
		{
			var project = Project.LoadFromXml("<Project><Panel WidthDimension=\"Fill\" HeightDimension=\"25%\" /></Project>");
			var panel = Assert.IsType<Panel>(project.Root);

			Assert.Equal(Dimension.Fill, panel.WidthDimension);
			Assert.Equal(Dimension.Percent(0.25f), panel.HeightDimension);
			Assert.Equal(new Point(200, 25), panel.Measure(new Point(200, 100)));
		}

		[Fact]
		public void SavesImplementedDimensionAttributes()
		{
			var project = Project.LoadFromXml("<Project><Panel WidthDimension=\"Fill\" HeightDimension=\"25%\" /></Project>");

			var element = XDocument.Parse(project.ToXml()).Root.Element("Panel");

			Assert.Equal("Fill", element.Attribute("WidthDimension").Value);
			Assert.Equal("25%", element.Attribute("HeightDimension").Value);
			Assert.Null(element.Attribute("Width"));
			Assert.Null(element.Attribute("Height"));
		}

		[Fact]
		public void LegacyWidthMMLDoesNotSaveDimensionAttributes()
		{
			var project = Project.LoadFromXml("<Project><Panel Width=\"80\" Height=\"24\" /></Project>");

			var element = XDocument.Parse(project.ToXml()).Root.Element("Panel");

			Assert.Equal("80", element.Attribute("Width").Value);
			Assert.Equal("24", element.Attribute("Height").Value);
			Assert.Null(element.Attribute("WidthDimension"));
			Assert.Null(element.Attribute("HeightDimension"));
		}

		[Fact]
		public void WidgetStyleCanApplyDimensions()
		{
			var panel = new Panel();
			panel.ApplyWidgetStyle(new WidgetStyle
			{
				WidthDimension = Dimension.Percent(0.5f),
				HeightDimension = Dimension.Fill
			});

			Assert.Equal(new Point(100, 100), panel.Measure(new Point(200, 100)));
		}
	}
}
