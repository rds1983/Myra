using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Data;
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
}
