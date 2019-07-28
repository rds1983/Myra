using System;
using System.Collections;
using Myra.Graphics2D.UI;

namespace Myra.Graphics2D.UI.Properties
{
	public class CollectionEditor : SingleItemContainer<Grid>
	{
		private readonly IList _collection;
		private readonly Type _type;
		private readonly ListBox _listItems;
		private readonly PropertyGrid _propertyGrid;
		private readonly ImageTextButton _buttonDelete, _buttonMoveUp, _buttonMoveDown;

		public CollectionEditor(IList collection, Type type)
		{
			Width = 500;
			Height = 400;

			_collection = collection;
			_type = type;

			InternalChild = new Grid
			{
				PaddingLeft = 8,
				PaddingRight = 8,
				PaddingBottom = 8,
				PaddingTop = 8,
				ColumnSpacing = 8,
				RowSpacing = 8
			};

			InternalChild.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			InternalChild.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			InternalChild.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));
			InternalChild.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_listItems = new ListBox
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			_listItems.SelectedIndexChanged += ListItemsOnSelectedIndexChanged;

			// Add initial items
			foreach (var item in _collection)
			{
				_listItems.Items.Add(new ListItem(BuildItemText(item), null, item));
			}

			InternalChild.Widgets.Add(_listItems);

			_propertyGrid = new PropertyGrid { GridColumn = 1 };
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;

			InternalChild.Widgets.Add(_propertyGrid);

			var buttonsGrid = new Grid
			{
				GridRow = 1,
				ColumnSpacing = 4
			};

			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			var buttonNew = new ImageTextButton { Text = "New" };
			buttonNew.Click += ButtonNewOnUp;
			buttonsGrid.Widgets.Add(buttonNew);

			_buttonDelete = new ImageTextButton
			{
				Text = "Delete",
				GridColumn = 1
			};
			_buttonDelete.Click += ButtonDeleteOnUp;
			buttonsGrid.Widgets.Add(_buttonDelete);

			_buttonMoveUp = new ImageTextButton
			{
				Text = "Up",
				GridColumn = 2
			};
			_buttonMoveUp.Click += ButtonMoveUpOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveUp);

			_buttonMoveDown = new ImageTextButton
			{
				Text = "Down",
				GridColumn = 3
			};
			_buttonMoveDown.Click += ButtonMoveDownOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveDown);

			InternalChild.Widgets.Add(buttonsGrid);

			UpdateButtonsEnabled();
		}

		private string BuildItemText(object item)
		{
			return item.ToString();
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
			MoveSelectedItem(_listItems.SelectedIndex.Value + 1);
		}

		private void ButtonMoveUpOnUp(object sender, EventArgs eventArgs)
		{
			MoveSelectedItem(_listItems.SelectedIndex.Value - 1);
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
			if (_listItems.SelectedItem != null)
			{
				_propertyGrid.Object = _listItems.SelectedItem.Tag;
			}
			else
			{
				_propertyGrid.Object = null;
			}

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