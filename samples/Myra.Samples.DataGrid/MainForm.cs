using Myra.Graphics2D.UI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Myra.Samples
{
	public partial class MainForm
	{
		private readonly DataGrid _dataGrid;

		public MainForm()
		{
			BuildUI();

			_dataGrid = new DataGrid();

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

			_dataGrid.Columns = columns.ToArray();

			var csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "customers-10000.csv");
			var records = ParseCsv(csvPath);
			_dataGrid.Data = records;

			_dataGrid.SelectedIndexChanged += _dataGrid_SelectedIndexChanged;

			_panelDataGrid.Widgets.Add(_dataGrid);

			_splitPane.SetSplitterPosition(0, 0.8f);

			_checkShowGridLines.IsChecked = _dataGrid.ShowGridLines;
			_checkResizableColumns.IsChecked = _dataGrid.ResizableColumns;
			_checkHasHeader.IsChecked = _dataGrid.HasHeader;
			_checkHasIndexColumn.IsChecked = _dataGrid.HasIndexColumn;
			_checkSortableHeaders.IsChecked = _dataGrid.SortableHeaders;
			_checkHasFilter.IsChecked = _dataGrid.HasFilter;

			_checkShowGridLines.IsCheckedChanged += (s, a) => _dataGrid.ShowGridLines = _checkShowGridLines.IsChecked;
			_checkResizableColumns.IsCheckedChanged += (s, a) => _dataGrid.ResizableColumns = _checkResizableColumns.IsChecked;
			_checkHasHeader.IsCheckedChanged += (s, a) => _dataGrid.HasHeader = _checkHasHeader.IsChecked;
			_checkHasIndexColumn.IsCheckedChanged += (s, a) => _dataGrid.HasIndexColumn = _checkHasIndexColumn.IsChecked;
			_checkSortableHeaders.IsCheckedChanged += (s, a) => _dataGrid.SortableHeaders = _checkSortableHeaders.IsChecked;
			_checkHasFilter.IsCheckedChanged += (s, a) => _dataGrid.HasFilter = _checkHasFilter.IsChecked;

			_comboFillColumn.SelectedIndexChanged += (s, a) =>
			{
				if (_comboFillColumn.SelectedIndex == null || _comboFillColumn.SelectedIndex == 0)
				{
					_dataGrid.FillColumnIndex = null;
				}
				else
				{
					_dataGrid.FillColumnIndex = _comboFillColumn.SelectedIndex.Value - 1;
				}
			};

			UpdateSelection();
		}

		private void _dataGrid_SelectedIndexChanged(object sender, Events.MyraEventArgs e)
		{
			UpdateSelection();
		}

		private void UpdateSelection()
		{
			var record = (Record)_dataGrid.SelectedItem;

			if (record != null)
			{
				_labelIndex.Text = record.Index.ToString();
				_labelCustomerId.Text = record.CustomerId;
				_labelFirstName.Text = record.FirstName;
				_labelLastName.Text = record.LastName;
				_labelCompany.Text = record.Company;
				_labelCity.Text = record.City;
				_labelCountry.Text = record.Country;
				_labelPhone1.Text = record.Phone1;
				_labelPhone2.Text = record.Phone2;
				_labelEmail.Text = record.Email;
				_labelSubscriptionDate.Text = record.SubscriptionDate;
				_labelWebsite.Text = record.Website;
			}
			else
			{
				_labelIndex.Text = string.Empty;
				_labelCustomerId.Text = string.Empty;
				_labelFirstName.Text = string.Empty;
				_labelLastName.Text = string.Empty;
				_labelCompany.Text = string.Empty;
				_labelCity.Text = string.Empty;
				_labelCountry.Text = string.Empty;
				_labelPhone1.Text = string.Empty;
				_labelPhone2.Text = string.Empty;
				_labelEmail.Text = string.Empty;
				_labelSubscriptionDate.Text = string.Empty;
				_labelWebsite.Text = string.Empty;
			}
		}

		/// <summary>
		/// Parses a CSV file into a list of <see cref="Record"/> objects, skipping the header line.
		/// </summary>
		/// <param name="path">The full path to the CSV file.</param>
		/// <returns>A list of parsed customer records.</returns>
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

		/// <summary>
		/// Splits a single CSV line into fields, correctly handling quoted values with escaped double-quotes.
		/// </summary>
		/// <param name="line">A single line of CSV text.</param>
		/// <returns>A list of field values.</returns>
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
	}
}