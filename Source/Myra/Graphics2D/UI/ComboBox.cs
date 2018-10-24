using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ComboBox : Button
	{
		private readonly Grid _itemsContainer;
		private bool _isExpanded;
		private ButtonStyle _dropDownItemStyle;
		private readonly ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();
		private int _selectingIndex = -1, _selectedIndex = -1;

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
				return _selectingIndex;
			}

			set
			{
				if (value == _selectingIndex)
				{
					return;
				}

				_selectingIndex = value;
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public ListItem SelectingItem
		{
			get { return _selectingIndex >= 0 && _selectingIndex < Items.Count ? Items[_selectingIndex] : null; }
			set
			{
				if (value == SelectingItem)
				{
					return;
				}

				if (value == null)
				{
					SelectingIndex = -1;
				}
				else
				{
					var index = Items.IndexOf(value);
					if (index < 0)
					{
						throw new ArgumentException("item is not in the list");
					}

					SelectingIndex = index;
				}
			}
		}

		[EditCategory("Data")]
		[DefaultValue(-1)]
		public int SelectedIndex
		{
			get
			{
				return _selectedIndex;
			}

			set
			{
				if (value == _selectedIndex)
				{
					return;
				}

				var item = SelectedItem;
				if (item != null && item.Widget != null)
				{
					((Button)item.Widget).IsPressed = false;
				}

				SelectingIndex = value;
				_selectedIndex = value;

				item = SelectedItem;

				if (item != null && item.Widget != null)
				{
					((Button)item.Widget).IsPressed = true;
				}

				UpdateSelectedItem();

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		[HiddenInEditor]
		[JsonIgnore]
		public ListItem SelectedItem
		{
			get { return _selectedIndex >= 0 && _selectedIndex < Items.Count ? Items[_selectedIndex] : null; }
			set
			{
				if (value == SelectedItem)
				{
					return;
				}

				if (value == null)
				{
					SelectedIndex = -1;
				}
				else
				{
					var index = Items.IndexOf(value);
					if (index < 0)
					{
						throw new ArgumentException("item is not in the list");
					}

					SelectedIndex = index;
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

			_itemsContainer = new Grid((GridStyle)null);

			if (style != null)
			{
				ApplyComboBoxStyle(style);
			}

			_items.CollectionChanged += ItemsOnCollectionChanged;
		}

		public ComboBox(string style) : this(Stylesheet.Current.ComboBoxStyles[style])
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
				Toggleable = true,
				Tag = item
			};

			item.Widget = button;

			button.MouseEntered += ItemOnMouseEntered;
			button.Down += ItemOnDown;

			button.HorizontalAlignment = HorizontalAlignment.Stretch;
			button.VerticalAlignment = VerticalAlignment.Stretch;
			_itemsContainer.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			_itemsContainer.Widgets.Insert(index, button);

			UpdateSelectedItem();
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

			UpdateSelectedItem();
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
			SelectingItem = (ListItem)widget.Tag;
		}

		private void ItemOnDown(object sender, EventArgs eventArgs)
		{
			var widget = (Button)sender;
			SelectedItem = (ListItem)widget.Tag;

			IsExpanded = false;
		}

		private void UpdateSelectedItem()
		{
			var item = SelectedItem;
			if (item != null)
			{
				Text = item.Text;
				TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor;
			}
			else
			{
				Text = string.Empty;
			}
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

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyComboBoxStyle(stylesheet.ComboBoxStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.ComboBoxStyles.Keys.ToArray();
		}
	}
}
