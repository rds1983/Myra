using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	public enum TabSelectorPosition
	{
		Top,
		Right,
		Bottom,
		Left
	}

	public class TabControl : Selector<Grid, TabItem>
	{
		private Grid _gridButtons;
		private Panel _panelContent;
		private TabSelectorPosition _selectorPosition;

		[Browsable(false)]
		[XmlIgnore]
		public TabControlStyle TabControlStyle { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public override SelectionMode SelectionMode { get => base.SelectionMode; set => base.SelectionMode = value; }

		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		[Category("Behavior")]
		[DefaultValue(TabSelectorPosition.Top)]
		public TabSelectorPosition TabSelectorPosition
		{
			get
			{
				return _selectorPosition;
			}
			set
			{
				if (value == _selectorPosition)
				{
					return;
				}

				_selectorPosition = value;
				UpdateSelectorPosition();
			}
		}

		[Category("Behavior")]
		public bool CloseableTabs { get; set; }

		public TabControl(string styleName = Stylesheet.DefaultStyleName) : base(new Grid())
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_gridButtons = new Grid();
			_panelContent = new Panel();

			_selectorPosition = TabSelectorPosition.Top;
			_gridButtons.DefaultColumnProportion = Proportion.Auto;
			_gridButtons.DefaultRowProportion = Proportion.Auto;

			InternalChild.DefaultColumnProportion = Proportion.Fill;
			InternalChild.DefaultRowProportion = Proportion.Fill;

			InternalChild.Widgets.Add(_gridButtons);
			InternalChild.Widgets.Add(_panelContent);

			UpdateSelectorPosition();

			ClipToBounds = true;

			SetStyle(styleName);
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (TabItem)sender;

			var label = item.LabelWidget;
			label.Text = item.Text;
			label.TextColor = item.Color ?? TabControlStyle.TabItemStyle.LabelStyle.TextColor;

			if (SelectedItem == item)
			{
				UpdateContent();
			}

			InvalidateMeasure();
		}

		private void UpdateSelectorPosition()
		{
			switch (_selectorPosition)
			{
				case TabSelectorPosition.Top:
					Grid.SetColumn(_gridButtons, 0);
					Grid.SetRow(_gridButtons, 0);

					Grid.SetColumn(_panelContent, 0);
					Grid.SetRow(_panelContent, 1);

					InternalChild.ColumnsProportions.Clear();
					InternalChild.RowsProportions.Clear();
					InternalChild.RowsProportions.Add(Proportion.Auto);
					InternalChild.RowsProportions.Add(Proportion.Fill);
					break;

				case TabSelectorPosition.Right:
					Grid.SetColumn(_gridButtons, 1);
					Grid.SetRow(_gridButtons, 0);

					Grid.SetColumn(_panelContent, 0);
					Grid.SetRow(_panelContent, 0);

					InternalChild.ColumnsProportions.Clear();
					InternalChild.ColumnsProportions.Add(Proportion.Fill);
					InternalChild.ColumnsProportions.Add(Proportion.Auto);
					InternalChild.RowsProportions.Clear();
					break;

				case TabSelectorPosition.Bottom:
					Grid.SetColumn(_gridButtons, 0);
					Grid.SetRow(_gridButtons, 1);

					Grid.SetColumn(_panelContent, 0);
					Grid.SetRow(_panelContent, 0);


					InternalChild.ColumnsProportions.Clear();
					InternalChild.RowsProportions.Clear();
					InternalChild.RowsProportions.Add(Proportion.Fill);
					InternalChild.RowsProportions.Add(Proportion.Auto);
					break;

				case TabSelectorPosition.Left:
					Grid.SetColumn(_gridButtons, 0);
					Grid.SetRow(_gridButtons, 0);

					Grid.SetColumn(_panelContent, 1);
					Grid.SetRow(_panelContent, 0);

					InternalChild.ColumnsProportions.Clear();
					InternalChild.ColumnsProportions.Add(Proportion.Auto);
					InternalChild.ColumnsProportions.Add(Proportion.Fill);
					InternalChild.RowsProportions.Clear();
					break;
			}

			UpdateButtonsGrid();
		}

		private void UpdateButtonsGrid()
		{
			bool tabSelectorIsLeftOrRight = TabSelectorPosition == TabSelectorPosition.Left ||
											TabSelectorPosition == TabSelectorPosition.Right;
			for (var i = 0; i < _gridButtons.Widgets.Count; ++i)
			{
				var widget = _gridButtons.Widgets[i];
				if (tabSelectorIsLeftOrRight)
				{
					Grid.SetColumn(widget, 0);
					Grid.SetRow(widget, i);
				}
				else
				{
					Grid.SetColumn(widget, i);
					Grid.SetRow(widget, 0);
				}
			}
		}

		protected override void InsertItem(TabItem item, int index)
		{
			item.Changed += ItemOnChanged;

			var image = new Image
			{
				Renderable = item.Image
			};

			var label = new Label(null)
			{
				Text = item.Text,
				TextColor = item.Color ?? TabControlStyle.TabItemStyle.LabelStyle.TextColor,
			};

			label.ApplyLabelStyle(TabControlStyle.TabItemStyle.LabelStyle);

			var panel = new HorizontalStackPanel
			{
				Spacing = item.ImageTextSpacing,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			panel.Widgets.Add(image);
			panel.Widgets.Add(label);

			var button = new ListViewButton
			{
				Tag = item,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Height = item.Height,
				Content = panel,
				ButtonsContainer = _gridButtons
			};

			button.ApplyButtonStyle(TabControlStyle.TabItemStyle);

			button.Click += ButtonOnClick;

			item.Button = button;

			if (!CloseableTabs)
			{
				_gridButtons.Widgets.Insert(index, button);
			} else
			{
				var topItemPanel = new HorizontalStackPanel();
				topItemPanel.Widgets.Add(button);
				StackPanel.SetProportionType(button, ProportionType.Fill);

				var closeButton = new Button
				{
					Content = new Image(),
					HorizontalAlignment = HorizontalAlignment.Right
				};

				closeButton.Click += (s, e) => RemoveItem(item);

				var style = TabControlStyle;
				if (style.CloseButtonStyle != null)
				{
					closeButton.ApplyButtonStyle(style.CloseButtonStyle);
					if (style.CloseButtonStyle.ImageStyle != null)
					{
						var closeImage = (Image)closeButton.Content;
						closeImage.ApplyPressableImageStyle(style.CloseButtonStyle.ImageStyle);
					}
				}

				topItemPanel.Widgets.Add(closeButton);
				_gridButtons.Widgets.Insert(index, topItemPanel);
			}

			UpdateButtonsGrid();

			if (Items.Count == 1)
			{
				// Select first item
				SelectedItem = item;
			}
		}

		private int GetButtonIndex(ListViewButton button)
		{
			var index = -1;
			for (var i = 0; i < _gridButtons.Widgets.Count; ++i)
			{
				var widget = _gridButtons.Widgets[i];
				if (widget == button || widget.FindChild<ListViewButton>() == button)
				{
					index = i;
					break;
				}
			}

			return index;
		}

		protected override void RemoveItem(TabItem item)
		{
			item.Changed -= ItemOnChanged;

			var index = GetButtonIndex(item.Button);
			if (index < 0)
			{
				return;
			}

			_gridButtons.Widgets.RemoveAt(index);
			Items.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}

			UpdateButtonsGrid();
		}

		private void UpdateContent()
		{
			_panelContent.Widgets.Clear();
			if (SelectedItem != null && SelectedItem.Content != null)
			{
				_panelContent.Widgets.Add(SelectedItem.Content);
			}
		}

		protected override void OnSelectedItemChanged()
		{
			base.OnSelectedItemChanged();
			UpdateContent();
		}

		protected override void Reset()
		{
			while (_gridButtons.Widgets.Count > 0)
			{
				RemoveItem((TabItem)_gridButtons.Widgets[0].Tag);
			}
		}

		private void ButtonOnClick(object sender, EventArgs eventArgs)
		{
			var button = (ListViewButton)sender;
			var index = GetButtonIndex(button);
			if (index < 0)
			{
				return;
			}

			SelectedIndex = index;
		}

		public void ApplyTabControlStyle(TabControlStyle style)
		{
			ApplyWidgetStyle(style);

			TabControlStyle = style;

			TabSelectorPosition = style.TabSelectorPosition;
			InternalChild.RowSpacing = style.HeaderSpacing;
			_gridButtons.ColumnSpacing = style.ButtonSpacing;

			_panelContent.ApplyWidgetStyle(style.ContentStyle);

			foreach (var item in Items)
			{
				item.Button.ApplyButtonStyle(style.TabItemStyle);

				var label = (Label)item.LabelWidget;
				label.ApplyLabelStyle(style.TabItemStyle.LabelStyle);
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTabControlStyle(stylesheet.TabControlStyles.SafelyGetStyle(name));
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var tabControl = (TabControl)w;

			TabControlStyle = tabControl.TabControlStyle;
			TabSelectorPosition = tabControl.TabSelectorPosition;

			foreach (var item in tabControl.Items)
			{
				Items.Add(item.Clone());
			}
		}
	}
}