using Myra.Attributes;
using Myra.Utility;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	public abstract class SelectorBase<WidgetType, ItemType> : Widget, ISelectorT<ItemType>
		where WidgetType : Widget
		where ItemType : class, ISelectorItem
	{
		private readonly SingleItemLayout<WidgetType> _layout;

		[DefaultValue(SelectionMode.Single)]
		public abstract SelectionMode SelectionMode { get; set; }

		[Browsable(false)]
		[Content]
		public abstract ObservableCollection<ItemType> Items { get; }

		[Browsable(false)]
		[XmlIgnore]
		public abstract int? SelectedIndex { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public abstract ItemType SelectedItem { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		protected WidgetType InternalChild => _layout.Child;

		public abstract event EventHandler SelectedIndexChanged;

		protected SelectorBase(WidgetType widget)
		{
			_layout = new SingleItemLayout<WidgetType>(this)
			{
				Child = widget
			};
			ChildrenLayout = _layout;
		}
	}

	public abstract class Selector<WidgetType, ItemType> : SelectorBase<WidgetType, ItemType>
		where WidgetType : Widget
		where ItemType : class, ISelectorItem
	{
		private ItemType _selectedItem;

		public override SelectionMode SelectionMode { get; set; }

		public override ObservableCollection<ItemType> Items { get; } = new ObservableCollection<ItemType>();

		public override int? SelectedIndex
		{
			get
			{
				if (_selectedItem == null)
				{
					return null;
				}

				return Items.IndexOf(_selectedItem);
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

		public override ItemType SelectedItem
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

				if (SelectionMode == SelectionMode.Single && _selectedItem != null)
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

		public override event EventHandler SelectedIndexChanged;
		public event EventHandler ItemsCollectionChanged;

		protected Selector(WidgetType widget): base(widget)
		{
			widget.HorizontalAlignment = HorizontalAlignment.Stretch;
			widget.VerticalAlignment = VerticalAlignment.Stretch;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			Items.CollectionChanged += ItemsOnCollectionChanged;
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

			OnItemCollectionChanged();
			InvalidateMeasure();
		}

		protected virtual void OnItemCollectionChanged()
		{
			ItemsCollectionChanged.Invoke(this);
		}

		protected virtual void OnSelectedItemChanged()
		{
		}

		private void FireSelectedIndexChanged()
		{
			SelectedIndexChanged.Invoke(this);
		}

		protected abstract void Reset();
		protected abstract void InsertItem(ItemType item, int index);
		protected abstract void RemoveItem(ItemType item);
	}
}