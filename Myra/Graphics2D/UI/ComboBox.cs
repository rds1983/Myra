using System;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class ComboBox : Button
	{
		private readonly Grid _itemsContainer;
		private bool _isExpanded;
		private int _selectingIndex = -1, _selectedIndex = -1;
		private ComboBoxItemStyle _dropDownItemStyle;

		public bool IsExpanded
		{
			get { return _isExpanded; }

			private set
			{
				if (value == _isExpanded)
				{
					return;
				}

				_isExpanded = value;

				SelectingIndex = SelectedIndex;

				if (!_isExpanded)
				{
					Desktop.HideContextMenu();
				}
				else
				{
					_itemsContainer.WidthHint = Bounds.Width;

					Desktop.ShowContextMenu(_itemsContainer, new Point(Bounds.X, Bounds.Bottom));
				}
			}
		}

		public int SelectingIndex
		{
			get { return _selectingIndex; }
			set
			{
				if (value == _selectingIndex)
				{
					return;
				}

				if (_selectingIndex != -1 && _selectingIndex < _itemsContainer.ChildCount)
				{
					_itemsContainer.Children[_selectingIndex].Background = _dropDownItemStyle.Background;
				}

				_selectingIndex = value;

				if (_selectingIndex != -1 && _selectingIndex < _itemsContainer.ChildCount)
				{
					var item = (Button)_itemsContainer.Children[_selectingIndex];
					item.Background = _dropDownItemStyle.SelectedBackground;
				}
			}
		}

		public int SelectedIndex
		{
			get { return _selectedIndex; }

			set
			{
				if (value == _selectedIndex)
				{
					return;
				}

				SelectingIndex = value;
				_selectedIndex = value;

				if (_selectedIndex != -1 && _selectedIndex < _itemsContainer.ChildCount)
				{
					var item = (Button)_itemsContainer.Children[_selectedIndex];
					Text = item.Text;
				}

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedIndexChanged;

		public ComboBox(ComboBoxStyle style) : base(style)
		{
			_itemsContainer = new Grid();


			if (style != null)
			{
				ApplyComboBoxStyle(style);
			}
		}

		public ComboBox() : this(Stylesheet.Current.ComboBoxStyle)
		{
		}

		private void AddItem(Widget item)
		{
			var pos = _itemsContainer.Children.Count;

			item.HorizontalAlignment = HorizontalAlignment.Stretch;
			item.VerticalAlignment = VerticalAlignment.Stretch;
			item.GridPosition.Y = pos;
			_itemsContainer.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));

			_itemsContainer.Children.Add(item);
		}

		public Button AddItem(string text)
		{
			var item = new Button(_dropDownItemStyle) {Text = text};

			item.MouseEntered += ItemOnMouseEntered;
			item.Down += ItemOnDown;

			AddItem(item);

			return item;
		}

		private void ItemOnMouseEntered(object sender, GenericEventArgs<Point> genericEventArgs)
		{
			var item = (Button)sender;
			var index = _itemsContainer.Children.IndexOf(item);
			SelectingIndex = index;
		}

		private void ItemOnDown(object sender, EventArgs eventArgs)
		{
			var item = (Button) sender;
			var index = _itemsContainer.Children.IndexOf(item);
			SelectedIndex = index;

			IsExpanded = false;
		}

		protected override void FireDown()
		{
			base.FireDown();

			if (_itemsContainer.Children.Count == 0)
			{
				return;
			}

			IsExpanded = !IsExpanded;
		}

		public void ApplyComboBoxStyle(ComboBoxStyle style)
		{
			if (style.ItemsContainerStyle != null)
			{
				_itemsContainer.ApplyWidgetStyle(style.ItemsContainerStyle);
			}

			_dropDownItemStyle = style.ComboBoxItemStyle;

			ApplyButtonStyle(style);
		}
	}
}
