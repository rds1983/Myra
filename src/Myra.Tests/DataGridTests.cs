using System;
using AssetManagementBase;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Data;
using Myra.Graphics2D.UI.Styles;
using Xunit;

namespace Myra.Tests;

[Collection("Myra Tests")]
public class DataGridTests
{
	[Fact]
	public void DataNullNoException()
	{
		var dataGrid = new DataGrid();

		var desktop = new Desktop
		{
			BoundsFetcher = () => new Rectangle(0, 0, 640, 480)
		};

		desktop.Root = dataGrid;

		var exception = Record.Exception(() => desktop.Render());

		Assert.Null(exception);
	}

	[Theory]
	[InlineData("not a bool")]
	[InlineData(42)]
	[InlineData(3.14)]
	public void CheckBoxColumnThrowsOnNonBooleanValue(object value)
	{
		var column = new DataGridCheckBoxColumn("Property", "Header");

		var exception = Assert.Throws<Exception>(() => column.CreateWidget(value, null));
		Assert.Contains("boolean", exception.Message);
	}

	[Theory]
	[InlineData("not an image")]
	[InlineData(42)]
	[InlineData(true)]
	public void ImageColumnThrowsOnNonImageValue(object value)
	{
		var column = new DataGridImageColumn("Property", "Header");

		var exception = Assert.Throws<Exception>(() => column.CreateWidget(value, null));
		Assert.Contains("image", exception.Message);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void CheckBoxColumnNoExceptionOnBooleanValue(bool value)
	{
		var style = new DataGridStyle { CheckCellStyle = new ImageStyle() };
		var column = new DataGridCheckBoxColumn("Property", "Header");

		var widget = column.CreateWidget(value, style);
		Assert.NotNull(widget);
	}

	[Theory]
	[InlineData(true, true)]
	[InlineData(false, true)]
	[InlineData(null, false)]
	public void CheckBoxColumnNoExceptionOnNullableBooleanValue(bool? value, bool widgetNotNull)
	{
		var style = new DataGridStyle { CheckCellStyle = new ImageStyle() };
		var column = new DataGridCheckBoxColumn("Property", "Header");

		var widget = column.CreateWidget(value, style);
		if (widgetNotNull)
		{
			Assert.NotNull(widget);
		}
		else
		{
			Assert.Null(widget);
		}
	}

	[Fact]
	public void ImageColumnNoExceptionOnImageValue()
	{
		var image = Utility.CreateAssetManager().LoadTextureRegion("Stylesheets/Default/default_ui_skin.xmat:button");
		var column = new DataGridImageColumn("Property", "Header");

		var widget = column.CreateWidget(image, new DataGridStyle());
		Assert.NotNull(widget);
	}
}
