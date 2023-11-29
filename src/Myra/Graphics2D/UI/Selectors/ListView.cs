using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using Myra.Attributes;
using System.Collections;
using System.Collections.Generic;
using Myra.Utility;
using System.Linq;
using info.lundin.math;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	public class ListView : Widget
	{
		private class ListViewButton : ToggleButton
		{
			public ListViewButton() : base(null)
			{
			}

			public override bool IsPressed
			{
				get => base.IsPressed;

				set
				{
					if (IsPressed && Parent != null)
					{
						// If this is last pressed button
						// Don't allow it to be unpressed
						var allow = false;
						foreach (var child in Parent.ChildrenCopy)
						{
							var asListViewButton = child as ListViewButton;
							if (asListViewButton == null || asListViewButton == this)
							{
								continue;
							}

							if (asListViewButton.IsPressed)
							{
								allow = true;
								break;
							}
						}

						if (!allow)
						{
							return;
						}
					}

					base.IsPressed = value;
				}
			}

			public override void OnPressedChanged()
			{
				base.OnPressedChanged();

				if (Parent == null || !IsPressed)
				{
					return;
				}

				// Release other pressed radio buttons
				foreach (var child in Parent.ChildrenCopy)
				{
					var asRadio = child as ListViewButton;
					if (asRadio == null || asRadio == this)
					{
						continue;
					}

					asRadio.IsPressed = false;
				}
			}
		}

		private class WidgetsEnumerator : IEnumerator<Widget>, IEnumerator
		{
			private readonly ListView _listView;
			private int _index = -1;

			public Widget Current => _listView.GetChildByIndex(_index);

			object IEnumerator.Current => _listView.GetChildByIndex(_index);

			public WidgetsEnumerator(ListView listView)
			{
				_listView = listView;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				++_index;

				return _index < _listView.ChildrenCount;
			}

			public void Reset()
			{
				_index = -1;
			}
		}

		private class WidgetsCollection : IList<Widget>, IList
		{
			private readonly ListView _listView;
			private Object _syncRoot;

			public object this[int index]
			{
				get => _listView.GetChildByIndex(index);
				set => _listView.SetChildByIndex(index, (Widget)value);
			}

			public bool IsFixedSize => false;

			public bool IsReadOnly => false;

			public int Count => Container.Widgets.Count;

			public bool IsSynchronized => false;

			// Synchronization root for this object.
			public virtual Object SyncRoot
			{
				get
				{
					if (_syncRoot == null)
					{
						System.Threading.Interlocked.CompareExchange(ref _syncRoot, new Object(), null);
					}
					return _syncRoot;
				}
			}

			Widget IList<Widget>.this[int index]
			{
				get => _listView.GetChildByIndex(index);
				set => _listView.SetChildByIndex(index, value);
			}

			private VerticalStackPanel Container => _listView._box;


			public WidgetsCollection(ListView listView)
			{
				_listView = listView;
			}

			public int Add(object value)
			{
				Add((Widget)value);
				return _listView.ChildrenCount - 1;
			}

			public void Clear()
			{
				Container.Widgets.Clear();
			}

			public bool Contains(object value) => FindButton((Widget)value) != null;

			public void CopyTo(Array array, int index)
			{
				throw new NotImplementedException();
			}

			public IEnumerator GetEnumerator() => new WidgetsEnumerator(_listView);

			public int IndexOf(object value)
			{
				var button = FindButton((Widget)value);
				return Container.Widgets.IndexOf(button);
			}

			public void Insert(int index, object value)
			{
				((IList<Widget>)this).Insert(index, (Widget)value);
			}

			public void Remove(object value)
			{
				((IList<Widget>)this).Remove((Widget)value);
			}

			public void RemoveAt(int index)
			{
				Container.Widgets.RemoveAt(index);
			}

			public int IndexOf(Widget item)
			{
				var button = FindButton(item);
				if (button == null)
				{
					return -1;
				}

				return Container.Widgets.IndexOf(button);
			}

			public void Insert(int index, Widget item)
			{
				var button = CreateButton(item);
				Container.Widgets.Insert(index, button);
			}

			public void Add(Widget item)
			{
				var button = CreateButton(item);
				Container.Widgets.Add(button);
			}

			public bool Contains(Widget item)
			{
				var button = FindButton(item);
				return button != null;
			}

			public void CopyTo(Widget[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public bool Remove(Widget item)
			{
				var button = FindButton(item);
				if (button == null)
				{
					return false;
				}

				Container.Widgets.Remove(button);
				return true;
			}

			IEnumerator<Widget> IEnumerable<Widget>.GetEnumerator() => new WidgetsEnumerator(_listView);

			private ListViewButton CreateButton(Widget w)
			{
				var button = new ListViewButton()
				{
					Content = w,
					HorizontalAlignment = HorizontalAlignment.Stretch
				};

				button.Click += _listView.ButtonOnClick;

				button.ApplyButtonStyle(_listView.ListBoxStyle.ListItemStyle);

				return button;
			}

			private ListViewButton FindButton(Widget w)
			{
				return (from ListViewButton b in Container.Widgets where b.Content == w select b).FirstOrDefault();
			}
		}

		private readonly ScrollViewer _scrollViewer;
		private readonly VerticalStackPanel _box;
		private Widget _selectedItem;

		internal ComboBox _parentComboBox;

		[Browsable(false)]
		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(SelectionMode.Single)]
		public SelectionMode SelectionMode { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public ScrollViewer ScrollViewer => _scrollViewer;

		[Content]
		[Browsable(false)]
		public IList<Widget> Widgets { get; }

		public int? SelectedIndex
		{
			get
			{
				if (_selectedItem == null)
				{
					return null;
				}

				return Widgets.IndexOf(_selectedItem);
			}

			set
			{
				if (value == null || value.Value < 0 || value.Value >= Widgets.Count)
				{
					SelectedItem = null;
					return;
				}

				SelectedItem = Widgets[value.Value];
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public Widget SelectedItem
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

				_selectedItem = value;

				SelectedIndexChanged.Invoke(this);
				OnSelectedItemChanged();
			}
		}

		private int ChildrenCount => _box.Children.Count;

		public event EventHandler SelectedIndexChanged;


		public ListView(string styleName = Stylesheet.DefaultStyleName)
		{
			_scrollViewer = new ScrollViewer();
			ChildrenLayout = new SingleItemLayout<ScrollViewer>(this)
			{
				Child = _scrollViewer
			};

			AcceptsKeyboardFocus = true;

			_box = new VerticalStackPanel();
			_scrollViewer.Content = _box;

			Widgets = new WidgetsCollection(this);

			SetStyle(styleName);
		}

		private void ButtonOnClick(object sender, EventArgs eventArgs)
		{
			var button = (ListViewButton)sender;
			if (!button.IsPressed)
			{
				return;
			}

			if (SelectionMode == SelectionMode.Single)
			{
				SelectedItem = button.Content;
			}

			ComboHideDropdown();
		}

		private void ComboHideDropdown()
		{
			if (_parentComboBox == null)
			{
				return;
			}

			_parentComboBox.HideDropdown();
		}

		protected void OnSelectedItemChanged()
		{
			if (_parentComboBox != null)
			{
				_parentComboBox.UpdateSelectedItem();
			}
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Up:
					if (SelectedIndex == null && Widgets.Count > 0)
					{
						SelectedIndex = Widgets.Count - 1;
						UpdateScrolling();
					}

					if (SelectedIndex != null && SelectedIndex.Value > 0)
					{
						SelectedIndex = SelectedIndex.Value - 1;
						UpdateScrolling();
					}
					break;
				case Keys.Down:
					if (SelectedIndex == null && Widgets.Count > 0)
					{
						SelectedIndex = 0;
						UpdateScrolling();
					}

					if (SelectedIndex != null && SelectedIndex.Value < Widgets.Count - 1)
					{
						SelectedIndex = SelectedIndex.Value + 1;
						UpdateScrolling();
					}
					break;
				case Keys.Enter:
					ComboHideDropdown();
					break;
			}
		}

		private void OnItemCollectionChanged()
		{
			_scrollViewer.ResetScroll();
		}

		private void UpdateScrolling()
		{
			if (SelectedItem == null)
			{
				return;
			}

			_scrollViewer.UpdateArrange();

			// Determine item position within ListBox
			var widget = SelectedItem;
			var p = _box.ToLocal(widget.ToGlobal(widget.Bounds.Location));

			var lineHeight = ListBoxStyle.ListItemStyle.LabelStyle.Font.LineHeight;

			var sp = _scrollViewer.ScrollPosition;

			var sz = new Point(_scrollViewer.Bounds.Width, _scrollViewer.Bounds.Height);
			if (p.Y < sp.Y)
			{
				sp.Y = p.Y;
			}
			else if (p.Y + lineHeight > sp.Y + sz.Y)
			{
				sp.Y = p.Y + lineHeight - sz.Y;
			}

			_scrollViewer.ScrollPosition = sp;
		}

		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			_scrollViewer.OnMouseWheel(delta);
		}

		public void ApplyListBoxStyle(ListBoxStyle style)
		{
			ApplyWidgetStyle(style);

			ListBoxStyle = style;

			foreach (var item in Widgets)
			{
				var asButton = item.Parent as ListViewButton;
				if (asButton != null)
				{
					asButton.ApplyButtonStyle(style.ListItemStyle);
				}
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyListBoxStyle(stylesheet.ListBoxStyles.SafelyGetStyle(name));
		}

		private Widget GetChildByIndex(int index)
		{
			var button = (ListViewButton)_box.Children[index];

			return button.Content;
		}

		private void SetChildByIndex(int index, Widget widget)
		{
			var button = (ListViewButton)_box.Children[index];

			button.Content = widget;
		}
	}
}