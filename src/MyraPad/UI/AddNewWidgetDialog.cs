using Myra.Graphics2D.UI;
using System;

namespace MyraPad.UI
{
	public partial class AddNewWidgetDialog
	{
		private string[] _names;

		public int SelectedIndex { get; private set; }

		public AddNewWidgetDialog()
		{
			BuildUI();

			_textFilter.TextChanged += (s, a) => UpdateList();
			_listWidgets.SelectedIndexChanged += (s, a) => UpdateButtons();
		}

		internal void SetNames(string[] names)
		{
			if (names == null)
			{
				throw new ArgumentNullException(nameof(names));
			}

			_names = names;
			UpdateList();
		}

		private void UpdateList()
		{
			_listWidgets.Widgets.Clear();

			for(var i = 0; i < _names.Length; ++i)
			{
				var name = _names[i];
				if (!string.IsNullOrEmpty(_textFilter.Text) && !name.Contains(_textFilter.Text, StringComparison.InvariantCultureIgnoreCase))
				{
					// Apply filter
					continue;
				}

				var label = new Label
				{
					Text = name,
					Tag = i
				};

				_listWidgets.Widgets.Add(label);
			}
		}

		private void UpdateButtons()
		{
			ButtonOk.Enabled = _listWidgets.SelectedIndex != null;
		}

		protected internal override void OnOk()
		{
			SelectedIndex = (int)_listWidgets.SelectedItem.Tag;

			base.OnOk();
		}
	}
}