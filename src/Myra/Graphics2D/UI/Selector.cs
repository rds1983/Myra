using System.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Xml.Serialization;
using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	public abstract class Selector<WidgetType, ItemType> : SingleItemContainer<WidgetType>, ISelector
		where WidgetType : Control
		where ItemType : class, ISelectorItem
	{
		private ItemType _selectedItem;
		private readonly ObservableCollection<ItemType> _items = new ObservableCollection<ItemType>();

		SelectionMode ISelector.SelectionMode
		{
			get; set;
		}

		[Category("Data")]
		[Content]
		public ObservableCollection<ItemType> Items
		{
			get
			{
				return _items;
			}
		}

		[Browsable(false)]
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

		[Browsable(false)]
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

				if (((ISelector)this).SelectionMode == SelectionMode.Single && _selectedItem != null)
				{
					_selectedItem.IsSelected = false;
				}

				_selectedItem = value;

				if (_selectedItem != null)
				{
					_selectedItem.IsSelected = true;
				}

				FireSelectedIndexChanged();
				OnSelectedItemChanged();
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

		protected virtual void OnSelectedItemChanged()
		{
		}

		private void FireSelectedIndexChanged()
		{
			var ev = SelectedIndexChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		protected abstract void Reset();
		protected abstract void InsertItem(ItemType item, int index);
		protected abstract void RemoveItem(ItemType item);
	}
}