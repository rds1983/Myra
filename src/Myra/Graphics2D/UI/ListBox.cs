using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using static Myra.Graphics2D.UI.Grid;

namespace Myra.Graphics2D.UI
{
	public class ListBox : Selector<ScrollPane, ListItem>
	{
		private readonly Grid _grid;

		[Browsable(false)]
		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(SelectionMode.Single)]
		public SelectionMode SelectionMode
		{
			get
			{
				return ((ISelector)this).SelectionMode;
			}

			set
			{
				((ISelector)this).SelectionMode = value;
			}
		}

		public ListBox(ListBoxStyle style) : base(new ScrollPane())
		{
			_grid = new Grid();
			InternalChild.Content = _grid;
			if (style != null)
			{
				ApplyListBoxStyle(style);
			}
		}

		public ListBox(Stylesheet stylesheet, string style) : this(stylesheet.ListBoxStyles[style])
		{
		}

		public ListBox(Stylesheet stylesheet) : this(stylesheet.ListBoxStyle)
		{
		}

		public ListBox(string style) : this(Stylesheet.Current, style)
		{
		}

		public ListBox() : this(Stylesheet.Current)
		{
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;

			var button = (ImageTextButton)item.Widget;
			button.Text = item.Text;
			button.TextColor = item.Color ?? ListBoxStyle.ListItemStyle.LabelStyle.TextColor;

			InvalidateMeasure();
		}

		private void UpdateGridPositions()
		{
			for (var i = 0; i < Items.Count; ++i)
			{
				var widget = _grid.Widgets[i];
				widget.GridRow = i;
			}
		}

		protected override void InsertItem(ListItem item, int index)
		{
			item.Changed += ItemOnChanged;

			Widget widget = null;

			if (!item.IsSeparator)
			{
				widget = new ListButton(ListBoxStyle.ListItemStyle, this)
				{
					Text = item.Text,
					TextColor = item.Color ?? ListBoxStyle.ListItemStyle.LabelStyle.TextColor,
					Tag = item,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch,
					Image = item.Image,
					ImageTextSpacing = item.ImageTextSpacing
				};

				((ImageTextButton)widget).PressedChanged += ButtonOnPressed;
			}
			else
			{
				widget = new HorizontalSeparator(ListBoxStyle.SeparatorStyle);
			}

			_grid.Widgets.Insert(index, widget);

			item.Widget = widget;

			UpdateGridPositions();
		}

		protected override void RemoveItem(ListItem item)
		{
			item.Changed -= ItemOnChanged;

			var index = _grid.Widgets.IndexOf(item.Widget);
			_grid.Widgets.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}

			UpdateGridPositions();
		}

		protected override void Reset()
		{
			while (_grid.Widgets.Count > 0)
			{
				RemoveItem((ListItem)_grid.Widgets[0].Tag);
			}
		}

		private void ButtonOnPressed(object sender, EventArgs eventArgs)
		{
			var item = (ImageTextButton)sender;
			if (!item.IsPressed)
			{
				return;
			}

			var listItem = (ListItem)item.Tag;
			if (SelectionMode == SelectionMode.Single)
			{
				SelectedItem = listItem;
			}
		}

		public void ApplyListBoxStyle(ListBoxStyle style)
		{
			ApplyWidgetStyle(style);

			ListBoxStyle = style;

			foreach (var item in Items)
			{
				var asButton = item.Widget as ImageTextButton;
				if (asButton != null)
				{
					asButton.ApplyImageTextButtonStyle(style.ListItemStyle);
					if (item.Color != null)
					{
						asButton.TextColor = item.Color.Value;
					}
				}

				var asSeparator = item.Widget as SeparatorWidget;
				if (asSeparator != null)
				{
					asSeparator.ApplyMenuSeparatorStyle(style.SeparatorStyle);
				}
			}
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyListBoxStyle(stylesheet.ListBoxStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.ListBoxStyles.Keys.ToArray();
		}
	}
}