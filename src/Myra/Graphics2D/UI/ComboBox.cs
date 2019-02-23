using System;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public class ComboBox : Selector<Button, ListItem>
	{
		private readonly Grid _itemsContainer;
		private ButtonStyle _dropDownItemStyle;

		[HiddenInEditor]
		[XmlIgnore]
		public bool IsExpanded
		{
			get { return Desktop.ContextMenu == _itemsContainer; }

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

					Desktop.ShowContextMenu(_itemsContainer, new Point(Bounds.X, Bounds.Bottom));
				}
			}
		}

		public ComboBox(ComboBoxStyle style) : base(new Button((ButtonStyle)null))
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_itemsContainer = new Grid();

			if (style != null)
			{
				ApplyComboBoxStyle(style);
			}
		}

		public ComboBox(string style) : this(Stylesheet.Current.ComboBoxStyles[style])
		{
		}

		public ComboBox() : this(Stylesheet.Current.ComboBoxStyle)
		{
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;
			var widget = (Button)item.Widget;

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

			button.MouseEntered += ItemOnMouseEntered;
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

		private void ItemOnMouseEntered(object sender, EventArgs args)
		{
			var widget = (Widget)sender;
		}

		private void ItemOnClick(object sender, EventArgs eventArgs)
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
				InternalChild.Text = item.Text;
				InternalChild.TextColor = item.Color ?? _dropDownItemStyle.LabelStyle.TextColor;
				((Button)item.Widget).IsPressed = true;
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
				((Button)item.Widget).ApplyButtonStyle(_dropDownItemStyle);
			}

			InternalChild.ApplyButtonStyle(style);
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
