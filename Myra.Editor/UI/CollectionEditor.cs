using System;
using System.Collections;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace Myra.Editor.UI
{
	public class CollectionEditor : SingleItemContainer<Grid>
	{
		private readonly IList _collection;
		private readonly Type _type;
		private readonly ListBox _listItems;
		private readonly PropertyGrid _propertyGrid;
		private readonly Button _buttonDelete, _buttonMoveUp, _buttonMoveDown;

		public CollectionEditor(IList collection, Type type)
		{
			WidthHint = 400;
			HeightHint = 400;

			_collection = collection;
			_type = type;

			Widget = new Grid
			{
				Padding = new PaddingInfo
				{
					Left = 8,
					Right = 8,
					Bottom = 8,
					Top = 8
				},
				ColumnSpacing = 8,
				RowSpacing = 8
			};

			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));
			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_listItems = new ListBox();
			_listItems.HorizontalAlignment = HorizontalAlignment.Stretch;
			_listItems.VerticalAlignment = VerticalAlignment.Stretch;

			_listItems.SelectedIndexChanged += ListItemsOnSelectedIndexChanged;

			// Add initial items
			foreach (var item in _collection)
			{
				_listItems.Items.Add(new ListItem(BuildItemText(item), null, item));
			}

			Widget.Widgets.Add(_listItems);

			_propertyGrid = new PropertyGrid {GridPosition = {X = 1}};
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;

			Widget.Widgets.Add(_propertyGrid);

			var buttonsGrid = new Grid
			{
				GridPosition = {Y = 1},
				ColumnSpacing = 4
			};

			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			var buttonNew = new Button {Text = "New"};
			buttonNew.Up += ButtonNewOnUp;
			buttonsGrid.Widgets.Add(buttonNew);

			_buttonDelete = new Button
			{
				Text = "Delete",
				GridPosition = {X = 1}
			};
			_buttonDelete.Up += ButtonDeleteOnUp;
			buttonsGrid.Widgets.Add(_buttonDelete);

			_buttonMoveUp = new Button
			{
				Text = "Up",
				GridPosition = {X = 2}
			};
			_buttonMoveUp.Up += ButtonMoveUpOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveUp);

			_buttonMoveDown = new Button
			{
				Text = "Down",
				GridPosition = {X = 3}
			};
			_buttonMoveDown.Up += ButtonMoveDownOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveDown);

			Widget.Widgets.Add(buttonsGrid);

			UpdateButtonsEnabled();
		}

		private string BuildItemText(object item)
		{
			var result = item.ToString();
			if (string.IsNullOrEmpty(result))
			{
				result = item.GetType().ToString();
			}

			return result;
		}

		private void UpdateButtonsEnabled()
		{
			_buttonDelete.Enabled = _listItems.SelectedIndex >= 0;
			_buttonMoveUp.Enabled = _listItems.SelectedIndex > 0;
			_buttonMoveDown.Enabled = _listItems.Items.Count > 1 && _listItems.SelectedIndex < _listItems.Items.Count - 1;
		}

		private void MoveSelectedItem(int newIndex)
		{
			var removed = _listItems.SelectedItem;
			_listItems.Items.Remove(removed);
			_listItems.Items.Insert(newIndex, removed);
			_listItems.SelectedItem = removed;
			UpdateButtonsEnabled();
		}

		private void ButtonMoveDownOnUp(object sender, EventArgs eventArgs)
		{
			MoveSelectedItem(_listItems.SelectedIndex + 1);
		}

		private void ButtonMoveUpOnUp(object sender, EventArgs eventArgs)
		{
			MoveSelectedItem(_listItems.SelectedIndex - 1);
		}

		private void ButtonDeleteOnUp(object sender, EventArgs eventArgs)
		{
			_listItems.Items.Remove(_listItems.SelectedItem);
			UpdateButtonsEnabled();
		}

		private void ButtonNewOnUp(object sender, EventArgs eventArgs)
		{
			var newItem = Activator.CreateInstance(_type);
			_listItems.Items.Add(new ListItem(newItem.ToString(), null, newItem));
			_listItems.SelectedIndex = _listItems.Items.Count - 1;
			UpdateButtonsEnabled();
		}

		private void PropertyGridOnPropertyChanged(object sender, EventArgs eventArgs)
		{
			if (_listItems.SelectedItem == null)
			{
				return;
			}

			_listItems.SelectedItem.Text = BuildItemText(_listItems.SelectedItem.Tag);
		}

		private void ListItemsOnSelectedIndexChanged(object sender, EventArgs eventArgs)
		{
			_propertyGrid.Object = _listItems.SelectedItem.Tag;
			UpdateButtonsEnabled();
		}

		public void SaveChanges()
		{
			_collection.Clear();

			foreach (var listItem in _listItems.Items)
			{
				_collection.Add(listItem.Tag);
			}
		}
	}
}