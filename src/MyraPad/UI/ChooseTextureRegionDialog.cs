using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI.Data;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyraPad.UI
{
	public partial class ChooseTextureRegionDialog
	{
		private class Record
		{
			private readonly TextureRegion _region;

			public TextureRegion Image => _region;

			public string Size => $"{_region.Size.X}x{_region.Size.Y}";

			public bool NP => _region is NinePatchRegion;

			public string Name => _region.Name;

			/// <summary>
			/// Initializes a new instance of the <see cref="Record"/> class wrapping the specified region.
			/// </summary>
			/// <param name="region">The texture atlas region to display.</param>
			public Record(TextureRegion region)
			{
				_region = region ?? throw new ArgumentNullException(nameof(region));
			}
		}

		private readonly DataGrid _dataGrid;

		private Stylesheet Stylesheet => Studio.Instance.Project.Stylesheet;

		public TextureRegion Image => ((Record)_dataGrid.SelectedItem).Image;


		public ChooseTextureRegionDialog()
		{
			BuildUI();

			// Build the grid with an image thumbnail, size text, nine-patch check box, and name columns
			_dataGrid = new DataGrid();
			var columns = new DataGridColumnBase[]
			{
				new DataGridImageColumn("Image"),
				new DataGridTextColumn("Size"),
				new DataGridCheckBoxColumn("NP"),
				new DataGridTextColumn("Name")
			};

			_dataGrid.Columns = columns.ToArray();

			// Make the name column fill the remaining space
			_dataGrid.FillColumnIndex = 3;

			var data = new List<Record>();
			foreach (var pair in Stylesheet.Atlas.Regions)
			{
				data.Add(new Record(pair.Value));
			}

			_dataGrid.Data = data;

			_dataGrid.SelectedIndexChanged += (s, a) => UpdateEnabled();

			Content = _dataGrid;

			UpdateEnabled();
		}

		private void UpdateEnabled()
		{
			ButtonOk.Enabled = _dataGrid.SelectedRowIndex != null;
		}
	}
}