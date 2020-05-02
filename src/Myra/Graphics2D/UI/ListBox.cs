using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

#if !STRIDE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#else
using Stride.Core.Mathematics;
using Stride.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class ListBox : Selector<ScrollViewer, ListItem>
	{
		private readonly VerticalStackPanel _box;
		internal ComboBox _parentComboBox;

		[Browsable(false)]
		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get; set;
		}

		[Category("Behavior")]
		[DefaultValue(SelectionMode.Single)]
		public override SelectionMode SelectionMode
		{
			get
			{
				return base.SelectionMode;
			}

			set
			{
				base.SelectionMode = value;
			}
		}

		protected internal override bool AcceptsMouseWheelFocus => InternalChild.AcceptsMouseWheelFocus;

		internal protected override bool AcceptsKeyboardFocus
		{
			get { return true; }
		}

		public ListBox(string styleName = Stylesheet.DefaultStyleName) : base(new ScrollViewer())
		{
			_box = new VerticalStackPanel();
			InternalChild.Content = _box;

			SetStyle(styleName);
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;

			var button = (ImageTextButton)item.Widget;
			button.Text = item.Text;
			button.TextColor = item.Color ?? ListBoxStyle.ListItemStyle.LabelStyle.TextColor;

			InvalidateMeasure();
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
				var separator = new HorizontalSeparator(null);
				separator.ApplySeparatorStyle(ListBoxStyle.SeparatorStyle);
				widget = separator;
			}

			_box.Widgets.Insert(index, widget);

			item.Widget = widget;
		}

		protected override void RemoveItem(ListItem item)
		{
			item.Changed -= ItemOnChanged;

			var index = _box.Widgets.IndexOf(item.Widget);
			_box.Widgets.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}
		}

		protected override void Reset()
		{
			while (_box.Widgets.Count > 0)
			{
				RemoveItem((ListItem)_box.Widgets[0].Tag);
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

		private void ComboHideDropdown()
		{
			if (_parentComboBox == null)
			{
				return;
			}

			_parentComboBox.HideDropdown();
		}

		protected override void OnSelectedItemChanged()
		{
			base.OnSelectedItemChanged();

			if (_parentComboBox != null)
			{
				_parentComboBox.UpdateSelectedItem();
			}
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch(k)
			{
				case Keys.Up:
					if (SelectedIndex != null && SelectedIndex.Value > 0)
					{
						SelectedIndex = SelectedIndex.Value - 1;
						UpdateScrolling();
					}
					break;
				case Keys.Down:
					if (SelectedIndex != null && SelectedIndex.Value < Items.Count - 1)
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

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (!InternalChild._verticalScrollingOn || !InternalChild._verticalScrollbarFrame.Contains(Desktop.TouchPosition))
			{
				ComboHideDropdown();
			}
		}

		private void UpdateScrolling()
		{
			if (SelectedItem == null)
			{
				return;
			}

			var p = SelectedItem.Widget.Bounds;

			var bounds = _box.ActualBounds;
			InternalChild.UpdateLayout();
			var sz = new Point(InternalChild.Bounds.Width, InternalChild.Bounds.Height);
			var maximum = InternalChild.ScrollMaximum;

			p.X -= bounds.X;
			p.Y -= bounds.Y;

			var lineHeight = CrossEngineStuff.LineSpacing(ListBoxStyle.ListItemStyle.LabelStyle.Font);

			var sp = InternalChild.ScrollPosition;

			if (p.Y < sp.Y)
			{
				sp.Y = p.Y;
			}
			else if (p.Y + lineHeight > sp.Y + sz.Y)
			{
				sp.Y = (p.Y + lineHeight - sz.Y);
			}

			InternalChild.ScrollPosition = sp;
		}

		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			InternalChild.OnMouseWheel(delta);
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
					asSeparator.ApplySeparatorStyle(style.SeparatorStyle);
				}
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyListBoxStyle(stylesheet.ListBoxStyles[name]);
		}
	}
}