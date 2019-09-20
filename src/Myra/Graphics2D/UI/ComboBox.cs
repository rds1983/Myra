using System;
using System.Linq;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using System.ComponentModel;

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
		private readonly Grid _itemsContainer;
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
			get { return Desktop.ContextMenu == _itemsContainerScroll; }

			private set
			{
				if (value == IsExpanded)
				{
					return;
				}

				if (IsExpanded)
				{
					Desktop.HideContextMenu();
				}
				else
				{
					_itemsContainer.Width = Bounds.Width;

					Desktop.ShowContextMenu(_itemsContainerScroll, new Point(Bounds.X, Bounds.Bottom));
				}
			}
		}

		public ComboBox(ComboBoxStyle style) : base(new ImageTextButton((ImageTextButtonStyle)null))
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_itemsContainer = new Grid();
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
			_itemsContainer.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			_itemsContainer.Widgets.Insert(index, button);

			UpdateSelectedItem();
			UpdateGridPositions();
		}

		protected override void RemoveItem(ListItem item)
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


		private void UpdateGridPositions()
		{
			for (var i = 0; i < _itemsContainer.Widgets.Count; ++i)
			{
				_itemsContainer.Widgets[i].GridRow = i;
			}
		}

		private void ItemOnClick(object sender, EventArgs eventArgs)
		{
			var widget = (ImageTextButton)sender;
			SelectedItem = (ListItem)widget.Tag;

			IsExpanded = false;
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

		public override void OnTouchDown()
		{
			base.OnTouchDown();

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
