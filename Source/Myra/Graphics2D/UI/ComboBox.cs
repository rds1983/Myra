using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ComboBox : Button
	{
		private ListItem _selectingItem, _selectedItem;

		private readonly Grid _itemsContainer;
		private bool _isExpanded;
		private ListItemStyle _dropDownItemStyle;
		private readonly ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

		[EditCategory("Data")]
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

				SelectingItem = SelectedItem;

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

		[HiddenInEditor]
		[JsonIgnore]
		public int SelectingIndex
		{
			get
			{
				if (_selectedItem == null)
				{
					return -1;
				}

				return _items.IndexOf(_selectingItem);
			}

			set
			{
				if (value < 0 || value >= Items.Count)
				{
					SelectingItem = null;
					return;
				}

				SelectingItem = Items[value];
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public ListItem SelectingItem
		{
			get { return _selectingItem; }
			set
			{
				if (value == _selectingItem)
				{
					return;
				}

				if (_selectingItem != null && _selectingItem.Widget != null)
				{
					_selectingItem.Widget.Background = _dropDownItemStyle.Background;
				}

				_selectingItem = value;

				if (_selectingItem != null)
				{
					if (_selectingItem.Widget != null)
					{
						_selectingItem.Widget.Background = _dropDownItemStyle.SelectedBackground;
					}

					_selectingItem.FireSelected();
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public int SelectedIndex
		{
			get
			{
				if (_selectedItem == null)
				{
					return -1;
				}

				return _items.IndexOf(_selectedItem);
			}

			set
			{
				if (value < 0 || value >= Items.Count)
				{
					SelectedItem = null;
					return;
				}

				SelectedItem = Items[value];
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public ListItem SelectedItem
		{
			get { return _selectedItem; }

			set
			{
				if (value == _selectedItem)
				{
					return;
				}

				SelectingItem = value;
				_selectedItem = value;

				if (_selectedItem != null)
				{
					Text = _selectedItem.Text;
					TextColor = _selectedItem.Color ?? _dropDownItemStyle.LabelStyle.TextColor;
				}
				else
				{
					Text = string.Empty;
				}

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override HorizontalAlignment ContentHorizontalAlignment
		{
			get { return base.ContentHorizontalAlignment; }
			set { base.ContentHorizontalAlignment = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override VerticalAlignment ContentVerticalAlignment
		{
			get { return base.ContentVerticalAlignment; }
			set { base.ContentVerticalAlignment = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override bool Toggleable
		{
			get { return base.Toggleable; }

			set
			{
				base.Toggleable = value;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override int? ImageWidthHint
		{
			get { return base.ImageWidthHint; }
			set { base.ImageWidthHint = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override int? ImageHeightHint
		{
			get { return base.ImageHeightHint; }
			set { base.ImageHeightHint = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override bool ImageVisible
		{
			get { return base.ImageVisible; }
			set { base.ImageVisible = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override Color TextColor
		{
			get { return base.TextColor; }
			set { base.TextColor = value; }
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

		public ComboBox(string style): this(Stylesheet.Current.ComboBoxVariants[style])
		{
		}

		public ComboBox() : this(Stylesheet.Current.ComboBoxStyle)
		{
		}

		private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						var index = args.NewStartingIndex;
						foreach (ListItem item in args.NewItems)
						{
							InsertItem(item, index);
							++index;
						}
						break;
					}

				case NotifyCollectionChangedAction.Remove:
					{
						foreach (ListItem item in args.OldItems)
						{
							RemoveItem(item);
						}
						break;
					}

				case NotifyCollectionChangedAction.Reset:
					{
						while (_itemsContainer.Widgets.Count > 0)
						{
							RemoveItem((ListItem)_itemsContainer.Widgets[0].Tag);
						}
						break;
					}
			}

			InvalidateMeasure();
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;
			var widget = (Button)item.Widget;

			widget.Text = item.Text;
			widget.TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor;

			if (SelectedItem == item)
			{
				Text = item.Text;
				TextColor = widget.TextColor;
			}

			InvalidateMeasure();
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

			item.Widget = button;

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

			var index = _itemsContainer.Widgets.IndexOf(item.Widget);
			_itemsContainer.RowsProportions.RemoveAt(index);
			_itemsContainer.Widgets.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}

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
			var widget = (Widget)sender;
			SelectingItem = (ListItem) widget.Tag;
		}

		private void ItemOnDown(object sender, EventArgs eventArgs)
		{
			var widget = (Button) sender;
			SelectedItem = (ListItem) widget.Tag;

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

			_dropDownItemStyle = style.ListItemStyle;

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
