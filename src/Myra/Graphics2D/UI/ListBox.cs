using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using static Myra.Graphics2D.UI.Grid;

namespace Myra.Graphics2D.UI
{
	public class ListBox : Selector<Grid, ListItem>
	{
		[HiddenInEditor]
		[XmlIgnore]
		public ListBoxStyle ListBoxStyle { get; set; }

		[EditCategory("Behavior")]
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

		public ListBox(ListBoxStyle style) : base(new Grid())
		{
			if (style != null)
			{
				ApplyListBoxStyle(style);
			}
		}

		public ListBox(string style) : this(Stylesheet.Current.ListBoxStyles[style])
		{
		}

		public ListBox() : this(Stylesheet.Current.ListBoxStyle)
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
				var widget = InternalChild.Widgets[i];
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

				((ImageTextButton)widget).Click += ButtonOnClick;
			}
			else
			{
				widget = new HorizontalSeparator(ListBoxStyle.SeparatorStyle);
			}

			InternalChild.RowsProportions.Insert(index, new Proportion(ProportionType.Auto));
			InternalChild.Widgets.Insert(index, widget);

			item.Widget = widget;

			UpdateGridPositions();
		}

		protected override void RemoveItem(ListItem item)
		{
			item.Changed -= ItemOnChanged;

			var index = InternalChild.Widgets.IndexOf(item.Widget);
			InternalChild.RowsProportions.RemoveAt(index);
			InternalChild.Widgets.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}

			UpdateGridPositions();
		}

		protected override void Reset()
		{
			while (InternalChild.Widgets.Count > 0)
			{
				RemoveItem((ListItem)InternalChild.Widgets[0].Tag);
			}
		}

		private void ButtonOnClick(object sender, EventArgs eventArgs)
		{
			var item = (ImageTextButton)sender;
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
					asButton.ApplyButtonStyle(style.ListItemStyle);
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