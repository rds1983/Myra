using Myra.Attributes;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	public abstract class Selector<WidgetType, ItemType>: SingleItemContainer<WidgetType> where WidgetType : Widget
		where ItemType: class, ISelectorItem
	{
		private ItemType _selectedItem;
		private readonly ObservableCollection<ItemType> _items = new ObservableCollection<ItemType>();

		[EditCategory("Data")]
		public ObservableCollection<ItemType> Items
		{
			get
			{
				return _items;
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public int? SelectedIndex
		{
			get
			{
				if (_selectedItem == null)
				{
					return null;
				}

				return _items.IndexOf(_selectedItem);
			}

			set
			{
				if (value == null || value.Value < 0 || value.Value >= Items.Count)
				{
					SelectedItem = default(ItemType);
					return;
				}

				SelectedItem = Items[value.Value];
			}
		}

		[HiddenInEditor]
		[XmlIgnore]
		public ItemType SelectedItem
		{
			get
			{
				return _selectedItem;
			}

			set
			{
				if (value == _selectedItem)
				{
					return;
				}

				if (_selectedItem != null)
				{
					UnselectItem(_selectedItem);
				}

				_selectedItem = value;

				if (_selectedItem != null)
				{
					SelectItem(_selectedItem);
				}

				var ev = SelectedIndexChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedIndexChanged;

		public Selector(WidgetType widget)
		{
			InternalChild = widget;
			widget.HorizontalAlignment = HorizontalAlignment.Stretch;
			widget.VerticalAlignment = VerticalAlignment.Stretch;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_items.CollectionChanged += ItemsOnCollectionChanged;
		}

		private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
				{
					var index = args.NewStartingIndex;
					foreach (ItemType item in args.NewItems)
					{
						InsertItem(item, index);
						++index;
					}
					break;
				}

				case NotifyCollectionChangedAction.Remove:
				{
					foreach (ItemType item in args.OldItems)
					{
						RemoveItem(item);
					}
					break;
				}

				case NotifyCollectionChangedAction.Reset:
				{
					Reset();
					break;
				}
			}

			InvalidateMeasure();
		}

		protected abstract void UnselectItem(ItemType item);
		protected abstract void SelectItem(ItemType item);
		protected abstract void Reset();
		protected abstract void InsertItem(ItemType item, int index);
		protected abstract void RemoveItem(ItemType item);
	}
}
