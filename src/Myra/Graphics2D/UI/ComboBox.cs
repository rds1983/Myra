using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Utility;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#else
using Stride.Core.Mathematics;
using Stride.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class ComboBox : SelectorBase<ImageTextButton, ListItem>
	{
		private readonly ListBox _listBox = new ListBox(null);

		[Category("Behavior")]
		[DefaultValue(300)]
		public int? DropdownMaximumHeight
		{
			get
			{
				return _listBox.MaxHeight;
			}

			set
			{
				_listBox.MaxHeight = value;
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public bool IsExpanded
		{
			get { return InternalChild.IsPressed; }
		}

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				if (Desktop != null)
				{
					Desktop.ContextMenuClosed -= DesktopOnContextMenuClosed;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.ContextMenuClosed += DesktopOnContextMenuClosed;
				}
			}
		}

		public override bool AcceptsKeyboardFocus => true;

		public override ObservableCollection<ListItem> Items => _listBox.Items;

		public override ListItem SelectedItem { get => _listBox.SelectedItem; set => _listBox.SelectedItem = value; }
		public override SelectionMode SelectionMode { get => _listBox.SelectionMode; set => _listBox.SelectionMode = value; }
		public override int? SelectedIndex { get => _listBox.SelectedIndex; set => _listBox.SelectedIndex = value; }

		public override event EventHandler SelectedIndexChanged
		{
			add
			{
				_listBox.SelectedIndexChanged += value;
			}

			remove
			{
				_listBox.SelectedIndexChanged -= value;
			}
		}

		public ComboBox(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new ImageTextButton(null)
			{
				Toggleable = true,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			InternalChild.PressedChanged += InternalChild_PressedChanged;

			_listBox._parentComboBox = this;
			_listBox.Items.CollectionChanged += Items_CollectionChanged;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			DropdownMaximumHeight = 300;

			SetStyle(styleName);
		}

		internal void HideDropdown()
		{
			if (Desktop.ContextMenu == _listBox)
			{
				Desktop.HideContextMenu();
			}
		}

		private void DesktopOnContextMenuClosed(object sender, GenericEventArgs<Widget> genericEventArgs)
		{
			InternalChild.IsPressed = false;
		}

		private void InternalChild_PressedChanged(object sender, EventArgs e)
		{
			if (_listBox.Items.Count == 0)
			{
				return;
			}

			if (InternalChild.IsPressed)
			{
				if (_listBox.SelectedIndex == null && Items.Count > 0)
				{
					_listBox.SelectedIndex = 0;
				}

				_listBox.Width = BorderBounds.Width;
				Desktop.ShowContextMenu(_listBox, new Point(Bounds.X, Bounds.Bottom));
			}
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;

			if (SelectedItem == item)
			{
				InternalChild.Text = item.Text;

				var widget = (ListButton)item.Tag;
				InternalChild.TextColor = widget.TextColor;
			}

			InvalidateMeasure();
		}

		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
				{
					foreach (ListItem item in args.NewItems)
					{
						item.Changed += ItemOnChanged;
					}
					break;
				}

				case NotifyCollectionChangedAction.Remove:
				{
					foreach (ListItem item in args.OldItems)
					{
						item.Changed -= ItemOnChanged;
					}

					UpdateSelectedItem();
					break;
				}
			}

			InvalidateMeasure();
		}

		internal void UpdateSelectedItem()
		{
			var item = SelectedItem;
			if (item != null)
			{
				InternalChild.Text = item.Text;
				InternalChild.TextColor = item.Color ?? _listBox.ListBoxStyle.ListItemStyle.LabelStyle.TextColor;
				((ImageTextButton)item.Widget).IsPressed = true;
			}
			else
			{
				InternalChild.Text = string.Empty;
			}
		}

		public void ApplyComboBoxStyle(ComboBoxStyle style)
		{
			if (style.ListBoxStyle != null)
			{
				var dropdownMaximumHeight = DropdownMaximumHeight;
				_listBox.ApplyListBoxStyle(style.ListBoxStyle);
				DropdownMaximumHeight = dropdownMaximumHeight;
			}

			InternalChild.ApplyImageTextButtonStyle(style);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			// Measure by the longest string
			var result = base.InternalMeasure(availableSize);

			// Reset ListBox Width so it wont be return by ListBox.Measure
			_listBox.Width = null;

			var listResult = _listBox.Measure(new Point(10000, 10000));
			if (listResult.X > result.X)
			{
				result.X = listResult.X;
			}

			// Add some x space
			result.X += 32;

			return result;
		}

		public override void Arrange()
		{
			base.Arrange();

			_listBox.Width = BorderBounds.Width;
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			_listBox.OnKeyDown(k);
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyComboBoxStyle(stylesheet.ComboBoxStyles[name]);
		}
	}
}
