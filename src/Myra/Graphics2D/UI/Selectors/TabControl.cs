using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System;
using System.Collections.ObjectModel;
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
		private ObservableCollection<Proportion> _buttonProportions;
		private ObservableCollection<Proportion> _contentProportions;

		[Browsable(false)]
		[XmlIgnore]
		public TabControlStyle TabControlStyle { get; set; }

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
				if (_selectorPosition != value)
				{
					// Content proportions and widgets need to be reversed if switching between 
					// right/bottom and top/left
					bool newValueIsTopOrLeft = value == TabSelectorPosition.Top ||
											   value == TabSelectorPosition.Left;
					bool oldValueWasBottomOrRight = _selectorPosition == TabSelectorPosition.Bottom ||
													_selectorPosition == TabSelectorPosition.Right;

					bool newValueIsTopOrBottom = value == TabSelectorPosition.Top ||
												 value == TabSelectorPosition.Bottom;
					bool oldValueWasTopOrBottom = _selectorPosition == TabSelectorPosition.Top ||
												  _selectorPosition == TabSelectorPosition.Bottom;

					bool transposeContent = newValueIsTopOrLeft == oldValueWasBottomOrRight;
					ObservableCollection<Proportion> newButtonProportions;
					ObservableCollection<Proportion> newContentProportions;

					if (newValueIsTopOrBottom)
					{
						newButtonProportions = _gridButtons.ColumnsProportions;
						newContentProportions = InternalChild.RowsProportions;
					}
					else
					{
						newButtonProportions = _gridButtons.RowsProportions;
						newContentProportions = InternalChild.ColumnsProportions;
					}

					Grid.SetColumn(_panelContent, value == TabSelectorPosition.Left ? 1 : 0);
					Grid.SetRow(_panelContent, value == TabSelectorPosition.Top ? 1 : 0);

					Grid.SetColumn(_gridButtons, value == TabSelectorPosition.Right ? 1 : 0);
					Grid.SetRow(_gridButtons, value == TabSelectorPosition.Bottom ? 1 : 0);

					if (newButtonProportions != _buttonProportions)
					{
						for (int i = 0; i < _buttonProportions.Count; i++)
						{
							newButtonProportions.Add(_buttonProportions[i]);
						}

						_buttonProportions.Clear();
						_buttonProportions = newButtonProportions;
					}

					if (newContentProportions != _contentProportions)
					{
						for (int i = 0; i < _contentProportions.Count; i++)
						{
							newContentProportions.Add(_contentProportions[i]);
						}

						_contentProportions.Clear();
						_contentProportions = newContentProportions;
					}

					if (transposeContent)
					{
						for (int i = 0; i < InternalChild.Widgets.Count; i++)
						{
							_contentProportions.Move(_contentProportions.Count - 1, i);
							InternalChild.Widgets.Move(InternalChild.Widgets.Count - 1, i);
						}
					}

					for (int i = 0; i < InternalChild.Widgets.Count; i++)
					{
						Widget w = InternalChild.Widgets[i];
						if (newValueIsTopOrBottom)
						{
							Grid.SetColumn(w, 0);
							Grid.SetRow(w, i);
						}
						else
						{
							Grid.SetColumn(w, i);
							Grid.SetRow(w, 0);
						}
					}

					_selectorPosition = value;

					if (newValueIsTopOrBottom != oldValueWasTopOrBottom)
					{
						UpdateGridPositions();
					}
				}
			}
		}

		public TabControl(string styleName = Stylesheet.DefaultStyleName) : base(new Grid())
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			_gridButtons = new Grid();
			_panelContent = new Panel();
			Grid.SetRow(_panelContent, 1);

			// Default to Top selector position:
			_selectorPosition = TabSelectorPosition.Top;
			_buttonProportions = _gridButtons.ColumnsProportions;
			_contentProportions = InternalChild.RowsProportions;

			// button, then content
			_contentProportions.Add(new Proportion());
			_contentProportions.Add(new Proportion(ProportionType.Fill));
			InternalChild.Widgets.Add(_gridButtons);
			InternalChild.Widgets.Add(_panelContent);

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

		private void UpdateGridPositions()
		{
			bool tabSelectorIsLeftOrRight = TabSelectorPosition == TabSelectorPosition.Left ||
											TabSelectorPosition == TabSelectorPosition.Right;
			for (var i = 0; i < Items.Count; ++i)
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
				VerticalAlignment = item.ContentVerticalAlignment
			};

			panel.Widgets.Add(image);
			panel.Widgets.Add(label);

			var button = new ListViewButton
			{
				Tag = item,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch,
				Height = item.Height,
				Content = panel
			};

			button.ApplyButtonStyle(TabControlStyle.TabItemStyle);

			button.Click += ButtonOnClick;

			_buttonProportions.Insert(index, new Proportion(ProportionType.Auto));
			_gridButtons.Widgets.Insert(index, button);

			item.Button = button;

			UpdateGridPositions();

			if (Items.Count == 1)
			{
				// Select first item
				SelectedItem = item;
			}
		}

		protected override void RemoveItem(TabItem item)
		{
			item.Changed -= ItemOnChanged;

			var index = _gridButtons.Widgets.IndexOf(item.Button);
			_buttonProportions.RemoveAt(index);
			_gridButtons.Widgets.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}

			UpdateGridPositions();
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
			var item = (ListViewButton)sender;
			var index = _gridButtons.Widgets.IndexOf(item);
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