using System;
using System.Collections;
using Microsoft.Xna.Framework;
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

		public Func<Color?, Color?> ColorChangeHandler
		{
			get { return _propertyGrid.ColorChangeHandler; }

			set { _propertyGrid.ColorChangeHandler = value; }
		}

		public CollectionEditor(IList collection, Type type)
		{
			WidthHint = 500;
			HeightHint = 400;

			_collection = collection;
			_type = type;

			Widget = new Grid
			{
				PaddingLeft = 8,
				PaddingRight = 8,
				PaddingBottom = 8,
				PaddingTop = 8,
				ColumnSpacing = 8,
				RowSpacing = 8
			};

			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));

			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill));
			Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

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

			Widget.Widgets.Add(_listItems);

			_propertyGrid = new PropertyGrid {GridPositionX = 1};
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;

			Widget.Widgets.Add(_propertyGrid);

			var buttonsGrid = new Grid
			{
				GridPositionY = 1,
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
				GridPositionX = 1
			};
			_buttonDelete.Up += ButtonDeleteOnUp;
			buttonsGrid.Widgets.Add(_buttonDelete);

			_buttonMoveUp = new Button
			{
				Text = "Up",
				GridPositionX = 2
			};
			_buttonMoveUp.Up += ButtonMoveUpOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveUp);

			_buttonMoveDown = new Button
			{
				Text = "Down",
				GridPositionX = 3
			};
			_buttonMoveDown.Up += ButtonMoveDownOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveDown);

			Widget.Widgets.Add(buttonsGrid);

			UpdateButtonsEnabled();
		}

		private string BuildItemText(object item)
		{
			var asItemWithId = item as IItemWithId;
			if (asItemWithId != null)
			{
				return "#" + asItemWithId.Id;
			}

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