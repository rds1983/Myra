using AssetManagementBase;
using Microsoft.Xna.Framework;
using Myra.Events;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Data;
using System.Collections.Generic;
using System.Linq;

namespace Myra.Samples;

/// <summary>
/// The main game class. Loads a texture atlas, displays its regions in a <see cref="DataGrid"/>,
/// and shows the selected region in a detail panel on the right.
/// </summary>
public class TextureAtlasViewerGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private Desktop _desktop;
	private readonly string _textureAtlasPath;
	private DataGrid _dataGrid;
	private Image _image;

	/// <summary>
	/// Initializes a new instance of the <see cref="TextureAtlasViewerGame"/> class for the specified texture atlas.
	/// </summary>
	/// <param name="textureAtlasPath">Path to the texture atlas (<c>.xmat</c>) file to display.</param>
	public TextureAtlasViewerGame(string textureAtlasPath)
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1800,
			PreferredBackBufferHeight = 1200
		};
		Window.AllowUserResizing = true;
		IsMouseVisible = true;
		_textureAtlasPath = textureAtlasPath;
	}

	/// <summary>
	/// Creates the <see cref="DataGrid"/>, defines columns, loads the texture atlas regions,
	/// builds the split layout, and assigns it as the desktop root.
	/// </summary>
	protected override void LoadContent()
	{
		base.LoadContent();
		MyraEnvironment.Game = this;

		var manager = AssetManager.CreateFileAssetManager(".");
		var textureAtlas = manager.LoadTextureRegionAtlas(_textureAtlasPath);

		// Build the grid with an image thumbnail, size text, nine-patch check box, and name columns
		_dataGrid = new DataGrid();

		var columns = new DataGridColumnBase[]
		{
			new DataGridImageColumn("Image"),
			new DataGridTextColumn("Size")
			{
				HasFilter = false,
				HasSorting = false,
			},
			new DataGridCheckBoxColumn("NP"),
			new DataGridTextColumn("Name", 300)
		};

		_dataGrid.Columns = columns.ToArray();

		// Wrap each region in a Record so it can be bound to the grid by property name
		var data = new List<Record>();
		foreach (var pair in textureAtlas.Regions)
		{
			data.Add(new Record(pair.Value));
		}

		_dataGrid.Data = data;

		// Lay the grid out on the left with a fixed width and the detail panel on the right
		var rootContainer = new HorizontalStackPanel();

		rootContainer.Widgets.Add(_dataGrid);

		rootContainer.Widgets.Add(new VerticalSeparator());

		_image = new Image
		{
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};

		var panel = new Panel();
		panel.Widgets.Add(_image);

		StackPanel.SetProportionType(panel, ProportionType.Fill);
		rootContainer.Widgets.Add(panel);

		// Show the selected region in the detail panel
		_dataGrid.SelectedIndexChanged += DataGrid_SelectedIndexChanged;

		_desktop = new Desktop
		{
			Root = rootContainer
		};
	}

	/// <summary>
	/// Updates the detail panel to display the image of the currently selected grid row.
	/// </summary>
	/// <param name="sender">The event sender.</param>
	/// <param name="e">The event arguments.</param>
	private void DataGrid_SelectedIndexChanged(object sender, MyraEventArgs e)
	{
		var record = (Record)_dataGrid.SelectedItem;

		if (record != null)
		{
			_image.Renderable = record.Image;
		}
		else
		{
			_image.Renderable = null;
		}
	}

	/// <summary>
	/// Clears the screen and renders the <see cref="Desktop"/>.
	/// </summary>
	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		GraphicsDevice.Clear(Color.Black);
		_desktop.Render();
	}
}
