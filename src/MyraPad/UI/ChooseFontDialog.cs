using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using System.Collections.Generic;

namespace MyraPad.UI
{
	public partial class ChooseFontDialog
	{
		private readonly Dictionary<int, string> _keys = new Dictionary<int, string>();

		private Stylesheet Stylesheet => Studio.Instance.Project.Stylesheet;

		public StylesheetFont Font => Stylesheet.Fonts[_keys[_gridData.SelectedRowIndex.Value]];


		public ChooseFontDialog()
		{
			BuildUI();

			RebuildList();

			_textFilter.TextChanged += (s, a) => RebuildList();
			_gridData.SelectedIndexChanged += (s, a) => UpdateEnabled();

			UpdateEnabled();
		}

		private void RebuildList()
		{
			_gridData.Widgets.Clear();
			_keys.Clear();

			var row = 0;
			foreach (var font in Stylesheet.Fonts)
			{
				if (!string.IsNullOrEmpty(_textFilter.Text) && !font.Id.Contains(_textFilter.Text))
				{
					continue;
				}

				var labelFontFile = new Label
				{
					Text = font.File
				};

				Grid.SetRow(labelFontFile, row);
				_gridData.Widgets.Add(labelFontFile);

				var labelSize = new Label
				{
					Text = $"{(int)font.Font.FontSize}",
					VerticalAlignment = VerticalAlignment.Center
				};

				Grid.SetColumn(labelSize, 1);
				Grid.SetRow(labelSize, row);
				_gridData.Widgets.Add(labelSize);

				var labelName = new Label
				{
					Text = font.Id,
					VerticalAlignment = VerticalAlignment.Center
				};

				Grid.SetColumn(labelName, 2);
				Grid.SetRow(labelName, row);
				_gridData.Widgets.Add(labelName);

				_keys[row] = font.Id;

				++row;
			}
		}

		private void UpdateEnabled()
		{
			ButtonOk.Enabled = _gridData.SelectedRowIndex != null;
		}
	}
}