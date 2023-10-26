using System;
using System.Collections;

namespace Myra.Graphics2D.UI.Properties
{
	public class CollectionEditor : Widget
	{
		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Vertical);
		private readonly IList _collection;
		private readonly Type _type;
		private readonly ListBox _listItems;
		private readonly PropertyGrid _propertyGrid;
		private readonly Button _buttonDelete, _buttonMoveUp, _buttonMoveDown;

		public CollectionEditor(IList collection, Type type)
		{
			ChildrenLayout = _layout;

			Width = 500;
			Height = 400;

			_collection = collection;
			_type = type;

			Children.Add(new HorizontalSeparator());

			var splitPanel = new HorizontalSplitPane();
			StackPanel.SetProportionType(splitPanel, ProportionType.Fill);

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

			splitPanel.Widgets.Add(_listItems);

			_propertyGrid = new PropertyGrid();
			Grid.SetColumn(_propertyGrid, 1);
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;
			splitPanel.Widgets.Add(_propertyGrid);

			Children.Add(splitPanel);
			Children.Add(new HorizontalSeparator());

			var buttonsGrid = new Grid
			{
				ColumnSpacing = 4
			};

			Grid.SetRow(buttonsGrid, 1);

			buttonsGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			buttonsGrid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));

			var buttonNew = Button.CreateTextButton("New");
			buttonNew.Click += ButtonNewOnUp;
			buttonsGrid.Widgets.Add(buttonNew);

			_buttonDelete = Button.CreateTextButton("Delete");
			Grid.SetColumn(_buttonDelete, 1);
			_buttonDelete.Click += ButtonDeleteOnUp;
			buttonsGrid.Widgets.Add(_buttonDelete);

			_buttonMoveUp = Button.CreateTextButton("Up");
			Grid.SetColumn(_buttonDelete, 2);
			_buttonMoveUp.Click += ButtonMoveUpOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveUp);

			_buttonMoveDown = Button.CreateTextButton("Down");
			Grid.SetColumn(_buttonDelete, 3);
			_buttonMoveDown.Click += ButtonMoveDownOnUp;
			buttonsGrid.Widgets.Add(_buttonMoveDown);

			Children.Add(buttonsGrid);

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