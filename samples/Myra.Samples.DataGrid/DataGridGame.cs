using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Myra.Samples;

public class DataGridGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private Desktop _desktop;

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

	protected override void LoadContent()
	{
		base.LoadContent();
		MyraEnvironment.Game = this;

		var dataGrid = new DataGrid();

		var columns = new List<DataGridColumnBase>
		{
			new DataGridTextColumn { Header = "First Name", Property = "FirstName", Width = 100 },
			new DataGridTextColumn { Header = "Last Name", Property = "LastName", Width = 100 },
			new DataGridTextColumn { Header = "Company", Property = "Company", Width = 200 },
			new DataGridTextColumn { Header = "City", Property = "City", Width = 200 },
			new DataGridTextColumn { Header = "Country", Property = "Country", Width = 200 },
			new DataGridTextColumn { Header = "Email", Property = "Email", Width = 200 },
			new DataGridTextColumn { Header = "Phone 1", Property = "Phone1", Width = 200 },
		};

		dataGrid.Columns = columns.ToArray();

		var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers-10000.csv");
		var records = ParseCsv(csvPath);
		dataGrid.Data = records;

		_desktop = new Desktop { Root = dataGrid };
	}

	private static List<Record> ParseCsv(string path)
	{
		var records = new List<Record>();
		var lines = File.ReadAllLines(path);

		foreach (var line in lines.Skip(1))
		{
			var fields = ParseCsvLine(line);
			if (fields.Count < 12)
				continue;

			records.Add(new Record
			{
				Index = int.Parse(fields[0]),
				CustomerId = fields[1],
				FirstName = fields[2],
				LastName = fields[3],
				Company = fields[4],
				City = fields[5],
				Country = fields[6],
				Phone1 = fields[7],
				Phone2 = fields[8],
				Email = fields[9],
				SubscriptionDate = fields[10],
				Website = fields[11]
			});
		}

		return records;
	}

	private static List<string> ParseCsvLine(string line)
	{
		var fields = new List<string>();
		var current = new System.Text.StringBuilder();
		bool inQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];

			if (inQuotes)
			{
				if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
				{
					current.Append('"');
					i++;
				}
				else if (c == '"')
				{
					inQuotes = false;
				}
				else
				{
					current.Append(c);
				}
			}
			else
			{
				if (c == '"')
				{
					inQuotes = true;
				}
				else if (c == ',')
				{
					fields.Add(current.ToString());
					current.Clear();
				}
				else
				{
					current.Append(c);
				}
			}
		}

		fields.Add(current.ToString());
		return fields;
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
		GraphicsDevice.Clear(Color.Black);
		_desktop.Render();
	}
}