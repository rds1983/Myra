using System;
using System.Linq;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Utility;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class ComboBox : Selector<ImageTextButton, ListItem>
	{
		private class CustomScrollPane : ScrollPane
		{
			public int? MaximumHeight
			{
				get; set;
			}

			protected override Point InternalMeasure(Point availableSize)
			{
				var result = base.InternalMeasure(availableSize);

				if (MaximumHeight != null && result.Y > MaximumHeight.Value)
				{
					result.Y = MaximumHeight.Value;
				}

				return result;
			}
		}

		private readonly CustomScrollPane _itemsContainerScroll;
		private readonly VerticalBox _itemsContainer;
		private ImageTextButtonStyle _dropDownItemStyle;

		[Category("Behavior")]
		[DefaultValue(300)]
		public int? DropdownMaximumHeight
		{
			get
			{
				return _itemsContainerScroll.MaximumHeight;
			}

			set
			{
				_itemsContainerScroll.MaximumHeight = value;
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
			set
			{
				if (Desktop != null)
				{
					Desktop.ContextMenuClosing -= DesktopOnContextMenuClosing;
					Desktop.ContextMenuClosed -= DesktopOnContextMenuClosed;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.ContextMenuClosing += DesktopOnContextMenuClosing;
					Desktop.ContextMenuClosed += DesktopOnContextMenuClosed;
				}
			}
		}

		public ComboBox(ComboBoxStyle style) : base(new ImageTextButton((ImageTextButtonStyle)null))
		{
			InternalChild.Toggleable = true;
			InternalChild.PressedChanged += InternalChild_PressedChanged;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_itemsContainer = new VerticalBox
			{
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			_itemsContainerScroll = new CustomScrollPane
			{
				Content = _itemsContainer
			};

			DropdownMaximumHeight = 300;

			if (style != null)
			{
				ApplyComboBoxStyle(style);
			}
		}

		public ComboBox(Stylesheet stylesheet, string style) : this(stylesheet.ComboBoxStyles[style])
		{
		}

		public ComboBox(Stylesheet stylesheet) : this(stylesheet.ComboBoxStyle)
		{
		}

		public ComboBox(string style) : this(Stylesheet.Current, style)
		{
		}

		public ComboBox() : this(Stylesheet.Current)
		{
		}

		private void DesktopOnContextMenuClosing(object sender, ContextMenuClosingEventArgs args)
		{
			// Prevent autoclosing of the context menu if mouse is over combobox button
			// It'll be manually closed in the InternalChild_PressedChanged
			if (Bounds.Contains(Desktop.TouchPosition))
			{
				args.Cancel = true;
			}
		}

		private void DesktopOnContextMenuClosed(object sender, GenericEventArgs<Widget> genericEventArgs)
		{
			InternalChild.IsPressed = false;
		}

		private void InternalChild_PressedChanged(object sender, EventArgs e)
		{
			if (_itemsContainer.Widgets.Count == 0)
			{
				return;
			}

			if (InternalChild.IsPressed)
			{
				_itemsContainer.Width = Bounds.Width;
				Desktop.ShowContextMenu(_itemsContainerScroll, new Point(Bounds.X, Bounds.Bottom));
			} else
			{
				Desktop.HideContextMenu();
			}
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;
			var widget = (ImageTextButton)item.Widget;

			widget.Text = item.Text;
			widget.TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor;

			if (SelectedItem == item)
			{
				InternalChild.Text = item.Text;
				InternalChild.TextColor = widget.TextColor;
			}

			InvalidateMeasure();
		}

		protected override void InsertItem(ListItem item, int index)
		{
			item.Changed += ItemOnChanged;

			var button = new ListButton(_dropDownItemStyle, this)
			{
				Text = item.Text,
				TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor,
				Tag = item
			};

			item.Widget = button;

			button.Click += ItemOnClick;

			button.HorizontalAlignment = HorizontalAlignment.Stretch;
			button.VerticalAlignment = VerticalAlignment.Stretch;
			_itemsContainer.Widgets.Insert(index, button);

			UpdateSelectedItem();
		}

		protected override void RemoveItem(ListItem item)
		{
			item.Changed -= ItemOnChanged;

			var index = _itemsContainer.Widgets.IndexOf(item.Widget);
			_itemsContainer.Widgets.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}

			UpdateSelectedItem();
		}

		protected override void OnSelectedItemChanged()
		{
			base.OnSelectedItemChanged();
			UpdateSelectedItem();
		}

		protected override void Reset()
		{
			while (_itemsContainer.Widgets.Count > 0)
			{
				RemoveItem((ListItem)_itemsContainer.Widgets[0].Tag);
			}
		}

		private void ItemOnClick(object sender, EventArgs eventArgs)
		{
			var widget = (ImageTextButton)sender;
			SelectedItem = (ListItem)widget.Tag;

			Desktop.HideContextMenu();
		}

		private void UpdateSelectedItem()
		{
			var item = SelectedItem;
			if (item != null)
			{
				InternalChild.Text = item.Text;
				InternalChild.TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor;
				((ImageTextButton)item.Widget).IsPressed = true;
			}
			else
			{
				InternalChild.Text = string.Empty;
			}
		}

		public void ApplyComboBoxStyle(ComboBoxStyle style)
		{
			if (style.ItemsContainerStyle != null)
			{
				_itemsContainer.ApplyWidgetStyle(style.ItemsContainerStyle);
			}

			_dropDownItemStyle = style.ListItemStyle;

			foreach (var item in Items)
			{
				var button = (ImageTextButton)item.Widget;
				button.ApplyImageTextButtonStyle(_dropDownItemStyle);
				if (item.Color != null)
				{
					button.TextColor = item.Color.Value;
				}
			}

			InternalChild.ApplyImageTextButtonStyle(style);
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
