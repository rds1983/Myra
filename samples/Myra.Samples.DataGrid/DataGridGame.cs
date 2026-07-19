using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Myra.Samples;

/// <summary>
/// Sample game demonstrating the <see cref="DataGrid"/> widget with a large CSV data set.
/// Loads customer records from a CSV file and displays them in a scrollable, sortable grid.
/// </summary>
public class DataGridGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private Desktop _desktop;
	private MainForm _mainForm;

	public DataGridGame()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1200,
			PreferredBackBufferHeight = 800
		};
		Window.AllowUserResizing = true;
		IsMouseVisible = true;
	}

	/// <summary>
	/// Creates the <see cref="DataGrid"/>, defines columns, loads CSV data, and assigns the grid as the desktop root.
	/// </summary>
	protected override void LoadContent()
	{
		base.LoadContent();
		MyraEnvironment.Game = this;

		_mainForm = new MainForm();
		_desktop = new Desktop
		{
			Root = _mainForm
		};
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