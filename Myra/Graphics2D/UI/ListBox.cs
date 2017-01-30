using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Myra.Edit;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class ListBox : GridBased
	{
		private ListItem _selectedItem;
		private readonly ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();

		public ObservableCollection<ListItem> Items
		{
			get { return _items; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public ComboBoxItemStyle ListBoxItemStyle { get; set; }

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
				if (value < 0 && value >= Items.Count)
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

				if (_selectedItem != null && ListBoxItemStyle != null)
				{
					_selectedItem.Widget.Background = ListBoxItemStyle.Background;
				}

				_selectedItem = value;

				if (_selectedItem != null)
				{
					if (ListBoxItemStyle != null)
					{
						_selectedItem.Widget.Background = ListBoxItemStyle.SelectedBackground;
					}

					_selectedItem.FireSelected();
				}

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedIndexChanged;

		public ListBox(ListBoxStyle style)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			if (style != null)
			{
				ApplyListBoxStyle(style);
			}

			_items.CollectionChanged += ItemsOnCollectionChanged;
		}

		public ListBox()
			: this(
				DefaultAssets.UIStylesheet.ListBoxStyle)
		{
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

		private void UpdateGridPositions()
		{
			for (var i = 0; i < Items.Count; ++i)
			{
				var widget = Widgets[i];
				widget.GridPosition.Y = i;
			}
		}

		private void InsertItem(ListItem item, int index)
		{
			var widget = new Button(ListBoxItemStyle)
			{
				Text = item.Text,

				Tag = item,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
			};

			widget.Down += ButtonOnDown;

			RowsProportions.Insert(index, new Proportion(ProportionType.Auto));
			Widgets.Insert(index, widget);

			item.Widget = widget;

			UpdateGridPositions();
		}

		private void RemoveItem(ListItem item)
		{
			var index = Widgets.IndexOf(item.Widget);
			RowsProportions.RemoveAt(index);
			Widgets.RemoveAt(index);

			UpdateGridPositions();
		}

		private void ButtonOnDown(object sender, EventArgs eventArgs)
		{
			var item = (Button) sender;
			var index = Widgets.IndexOf(item);
			SelectedIndex = index;
		}

		public void ApplyListBoxStyle(ListBoxStyle style)
		{
			ApplyWidgetStyle(style);

			ListBoxItemStyle = style.ListBoxItemStyle;
		}
	}
}