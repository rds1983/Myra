using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Myra.Edit;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ComboBox : Button
	{
		private readonly Grid _itemsContainer;
		private bool _isExpanded;
		private int _selectingIndex = -1, _selectedIndex = -1;
		private ComboBoxItemStyle _dropDownItemStyle;
		private readonly ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

		public ObservableCollection<ListItem> Items
		{
			get { return _items; }
		}

		[HiddenInEditor]
		[JsonIgnore]
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

					Desktop.ShowContextMenu(_itemsContainer, new Point(AbsoluteBounds.X, AbsoluteBounds.Bottom));
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
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
					_itemsContainer.Widgets[_selectingIndex].Background = _dropDownItemStyle.Background;
				}

				_selectingIndex = value;

				if (_selectingIndex != -1 && _selectingIndex < _itemsContainer.ChildCount)
				{
					var widget = (Button)_itemsContainer.Widgets[_selectingIndex];
					widget.Background = _dropDownItemStyle.SelectedBackground;

					((ListItem)widget.Tag).FireSelected();
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
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
					var widget = (Button)_itemsContainer.Widgets[_selectedIndex];
					Text = widget.Text;
					TextColor = widget.TextColor;
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
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_itemsContainer = new Grid();

			if (style != null)
			{
				ApplyComboBoxStyle(style);
			}

			_items.CollectionChanged += ItemsOnCollectionChanged;
		}

		private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				var index = args.NewStartingIndex;
				foreach (ListItem item in args.NewItems)
				{
					InsertItem(item, index);
					++index;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (ListItem item in args.OldItems)
				{
					RemoveItem(item);
				}
			}
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem) sender;
			var widget = FindWidgetByItem(item);

			widget.Text = item.Text;
			widget.TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor;
		}

		public ComboBox() : this(DefaultAssets.UIStylesheet.ComboBoxStyle)
		{
		}

		public Button FindWidgetByItem(ListItem item)
		{
			foreach (var widget in _itemsContainer.Widgets)
			{
				if (widget.Tag == item)
				{
					return (Button)widget;
				}
			}

			return null;
		}

		private void InsertItem(ListItem item, int index)
		{
			item.Changed += ItemOnChanged;

			var button = new Button(_dropDownItemStyle)
			{
				Text = item.Text,
				TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor,
				Tag = item
			};

			button.MouseEntered += ItemOnMouseEntered;
			button.Down += ItemOnDown;

			button.HorizontalAlignment = HorizontalAlignment.Stretch;
			button.VerticalAlignment = VerticalAlignment.Stretch;
			_itemsContainer.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			_itemsContainer.Widgets.Insert(index, button);

			UpdateGridPositions();
		}

		private void RemoveItem(ListItem item)
		{
			item.Changed -= ItemOnChanged;

			var widget = FindWidgetByItem(item);

			if (widget == null)
			{
				return;
			}

			var index = _itemsContainer.Widgets.IndexOf(widget);
			_itemsContainer.RowsProportions.RemoveAt(index);
			_itemsContainer.Widgets.RemoveAt(index);

			UpdateGridPositions();
		}

		private void UpdateGridPositions()
		{
			for (var i = 0; i < _itemsContainer.ChildCount; ++i)
			{
				_itemsContainer.Widgets[i].GridPositionY = i;
			}
		}

		private void ItemOnMouseEntered(object sender, GenericEventArgs<Point> genericEventArgs)
		{
			var item = (Button)sender;
			var index = _itemsContainer.Widgets.IndexOf(item);
			SelectingIndex = index;
		}

		private void ItemOnDown(object sender, EventArgs eventArgs)
		{
			var item = (Button) sender;
			var index = _itemsContainer.Widgets.IndexOf(item);
			SelectedIndex = index;

			IsExpanded = false;
		}

		protected override void FireDown()
		{
			base.FireDown();

			if (_itemsContainer.Widgets.Count == 0)
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

		protected override Point InternalMeasure(Point availableSize)
		{
			// Measure by the longest string
			var result = base.InternalMeasure(availableSize);

			foreach (var item in _itemsContainer.Widgets)
			{
				var childSize = item.Measure(new Point(10000, 10000));
				if (childSize.X > result.X)
				{
					result.X = childSize.X;
				}
			}

			// Add some x space
			result.X += 32;

			return result;
		}
	}
}
