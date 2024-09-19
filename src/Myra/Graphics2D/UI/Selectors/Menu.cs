using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.Attributes;
using FontStashSharp;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class Menu : Widget
	{
		private readonly SingleItemLayout<Grid> _layout;
		private Proportion _imageProportion = Proportion.Auto, _shortcutProportion = Proportion.Auto;
		private bool _dirty = true;
		private bool _internalSetSelectedIndex = false;

		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		[Browsable(false)]
		[XmlIgnore]
		public MenuStyle MenuStyle { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		internal MenuItem OpenMenuItem { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public bool IsOpen
		{
			get
			{
				return OpenMenuItem != null;
			}
		}

		[Browsable(false)]
		[Content]
		public ObservableCollection<IMenuItem> Items { get; } = new ObservableCollection<IMenuItem>();

		[Category("Appearance")]
		public SpriteFontBase LabelFont
		{
			get
			{
				return MenuStyle.LabelStyle.Font;
			}

			set
			{
				MenuStyle.LabelStyle.Font = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/TextColor")]
		public Color LabelColor
		{
			get
			{
				return MenuStyle.LabelStyle.TextColor;
			}

			set
			{
				MenuStyle.LabelStyle.TextColor = value;
			}
		}

		[Category("Appearance")]
		public IBrush SelectionHoverBackground
		{
			get
			{
				return InternalChild.SelectionHoverBackground;
			}

			set
			{
				InternalChild.SelectionHoverBackground = value;
			}
		}

		[Category("Appearance")]
		public IBrush SelectionBackground
		{
			get
			{
				return InternalChild.SelectionBackground;
			}

			set
			{
				InternalChild.SelectionBackground = value;
			}
		}

		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment LabelHorizontalAlignment { get; set; }

		[Category("Behavior")]
		[DefaultValue(true)]
		public bool HoverIndexCanBeNull
		{
			get
			{
				return InternalChild.HoverIndexCanBeNull;
			}

			set
			{
				InternalChild.HoverIndexCanBeNull = value;
			}
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

		[Browsable(false)]
		[XmlIgnore]
		public int? HoverIndex
		{
			get
			{
				if (Orientation == Orientation.Horizontal)
				{
					return InternalChild.HoverColumnIndex;
				}

				return InternalChild.HoverRowIndex;
			}

			set
			{

				if (Orientation == Orientation.Horizontal)
				{
					InternalChild.HoverColumnIndex = value;
				}
				else
				{
					InternalChild.HoverRowIndex = value;
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		public int? SelectedIndex
		{
			get
			{
				if (Orientation == Orientation.Horizontal)
				{
					return InternalChild.SelectedColumnIndex;
				}

				return InternalChild.SelectedRowIndex;
			}

			set
			{

				if (Orientation == Orientation.Horizontal)
				{
					InternalChild.SelectedColumnIndex = value;
				}
				else
				{
					InternalChild.SelectedRowIndex = value;
				}
			}
		}

		private MenuItem SelectedMenuItem
		{
			get
			{
				return GetMenuItem(SelectedIndex);
			}
		}

		private bool HasImage
		{
			get
			{
				if (Orientation == Orientation.Horizontal)
				{
					return false;
				}

				return InternalChild.ColumnsProportions[0] == _imageProportion;
			}

			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					return;
				}

				var hasImage = HasImage;
				if (hasImage == value)
				{
					return;
				}

				if (hasImage && !value)
				{
					InternalChild.ColumnsProportions.RemoveAt(0);
				}
				else if (!hasImage && value)
				{
					InternalChild.ColumnsProportions.Insert(0, _imageProportion);
				}

				_dirty = true;
			}
		}

		private bool HasShortcut
		{
			get
			{
				if (Orientation == Orientation.Horizontal)
				{
					return false;
				}

				return InternalChild.ColumnsProportions[InternalChild.ColumnsProportions.Count - 1] == _shortcutProportion;
			}

			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					return;
				}

				var hasShortcut = HasShortcut;
				if (hasShortcut == value)
				{
					return;
				}

				if (hasShortcut && !value)
				{
					InternalChild.ColumnsProportions.RemoveAt(InternalChild.ColumnsProportions.Count - 1);
				}
				else if (!hasShortcut && value)
				{
					InternalChild.ColumnsProportions.Add(_shortcutProportion);
				}

				_dirty = true;
			}
		}

		protected Grid InternalChild => _layout.Child;

		protected Menu(string styleName)
		{
			_layout = new SingleItemLayout<Grid>(this)
			{
				Child = new Grid
				{
					CanSelectNothing = true
				}
			};
			ChildrenLayout = _layout;

			Items.CollectionChanged += ItemsOnCollectionChanged;

			AcceptsKeyboardFocus = true;

			if (Orientation == Orientation.Horizontal)
			{
				InternalChild.GridSelectionMode = GridSelectionMode.Column;
				InternalChild.DefaultColumnProportion = Proportion.Auto;
				InternalChild.DefaultRowProportion = Proportion.Auto;
			}
			else
			{
				InternalChild.GridSelectionMode = GridSelectionMode.Row;
				InternalChild.ColumnsProportions.Add(Proportion.Fill);
				InternalChild.DefaultRowProportion = Proportion.Auto;
			}

			InternalChild.HoverIndexChanged += OnHoverIndexChanged;
			InternalChild.SelectedIndexChanged += OnSelectedIndexChanged;
			InternalChild.TouchUp += InternalChild_TouchUp;

			OpenMenuItem = null;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			HoverIndexCanBeNull = true;

			SetStyle(styleName);
		}

		private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				var index = args.NewStartingIndex;
				foreach (IMenuItem item in args.NewItems)
				{
					InsertItem(item, index);
					++index;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (IMenuItem item in args.OldItems)
				{
					RemoveItem(item);
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				InternalChild.Widgets.Clear();
			}

			_dirty = true;
		}

		/// <summary>
		/// Recursively search for the menu item by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>null if not found</returns>
		public MenuItem FindMenuItemById(string id)
		{
			foreach (var item in Items)
			{
				var asMenuItem = item as MenuItem;
				if (asMenuItem == null)
				{
					continue;
				}

				var result = asMenuItem.FindMenuItemById(id);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		private void UpdateWidgets()
		{
			var hasImage = false;
			var hasShortcut = false;
			foreach (var item in Items)
			{
				var menuItem = item as MenuItem;
				if (menuItem == null)
				{
					continue;
				}

				if (menuItem.Image != null)
				{
					hasImage = true;
				}

				if (!string.IsNullOrEmpty(menuItem.ShortcutText))
				{
					hasShortcut = true;
				}
			}

			HasImage = hasImage;
			HasShortcut = hasShortcut;
		}

		private void SetMenuItem(MenuItem menuItem)
		{
			menuItem.ImageWidget.Renderable = menuItem.Image;
			if (menuItem.ImageWidget.Renderable != null && !InternalChild.Widgets.Contains(menuItem.ImageWidget))
			{
				InternalChild.Widgets.Add(menuItem.ImageWidget);
			}
			else if (menuItem.ImageWidget.Renderable == null && InternalChild.Widgets.Contains(menuItem.ImageWidget))
			{
				InternalChild.Widgets.Remove(menuItem.ImageWidget);
			}

			menuItem.Shortcut.Text = menuItem.ShortcutText;
			if (menuItem.ShortcutColor != null)
			{
				menuItem.Shortcut.TextColor = menuItem.ShortcutColor.Value;
			}
			else if (MenuStyle != null && MenuStyle.ShortcutStyle != null)
			{
				menuItem.Shortcut.TextColor = MenuStyle.ShortcutStyle.TextColor;
			}

			if (!string.IsNullOrEmpty(menuItem.ShortcutText) && !InternalChild.Widgets.Contains(menuItem.Shortcut))
			{
				InternalChild.Widgets.Add(menuItem.Shortcut);
			}
			else if (string.IsNullOrEmpty(menuItem.ShortcutText) && InternalChild.Widgets.Contains(menuItem.Shortcut))
			{
				InternalChild.Widgets.Remove(menuItem.Shortcut);
			}

			menuItem.Label.Text = menuItem.DisplayText;
			if (menuItem.Color != null)
			{
				menuItem.Label.TextColor = menuItem.Color.Value;
			}
			else if (MenuStyle != null && MenuStyle.LabelStyle != null)
			{
				menuItem.Label.TextColor = MenuStyle.LabelStyle.TextColor;
			}

			menuItem.Label.HorizontalAlignment = LabelHorizontalAlignment;

			UpdateWidgets();
		}

		private void MenuItemOnChanged(object sender, EventArgs eventArgs)
		{
			SetMenuItem((MenuItem)sender);
		}

		private void InsertItem(IMenuItem item, int index)
		{
			item.Menu = this;

			var menuItem = item as MenuItem;
			if (menuItem != null)
			{
				menuItem.Changed += MenuItemOnChanged;

				if (Orientation == Orientation.Horizontal)
				{
					menuItem.Label.ApplyLabelStyle(MenuStyle.LabelStyle);
				}
				else
				{
					menuItem.ImageWidget.ApplyPressableImageStyle(MenuStyle.ImageStyle);
					menuItem.Label.ApplyLabelStyle(MenuStyle.LabelStyle);
					menuItem.Shortcut.ApplyLabelStyle(MenuStyle.ShortcutStyle);
				}

				// Add only label, as other widgets(image and shortcut) would be optionally added by SetMenuItem
				InternalChild.Widgets.Add(menuItem.Label);
				SetMenuItem((MenuItem)item);
			}
			else
			{
				SeparatorWidget separator;
				if (Orientation == Orientation.Horizontal)
				{
					separator = new VerticalSeparator(null);
				}
				else
				{
					separator = new HorizontalSeparator(null);
				}

				separator.ApplySeparatorStyle(MenuStyle.SeparatorStyle);

				InternalChild.Widgets.Add(separator);

				((MenuSeparator)item).Separator = separator;
			}
		}

		private void RemoveItem(IMenuItem item)
		{
			var menuItem = item as MenuItem;
			if (menuItem != null)
			{
				menuItem.Changed -= MenuItemOnChanged;
				InternalChild.Widgets.Remove(menuItem.ImageWidget);
				InternalChild.Widgets.Remove(menuItem.Label);
				InternalChild.Widgets.Remove(menuItem.Shortcut);
			}
			else
			{
				InternalChild.Widgets.Remove(((MenuSeparator)item).Separator);
			}
		}

		public void Close()
		{
			if (Desktop != null)
			{
				Desktop.HideContextMenu();
			}
			HoverIndex = SelectedIndex = null;
		}

		private Rectangle GetItemBounds(int index)
		{
			var bounds = InternalChild.Bounds;
			if (Orientation == Orientation.Horizontal)
			{
				return new Rectangle(bounds.X + InternalChild.GetCellLocationX(index),
					bounds.Y,
					InternalChild.GetColumnWidth(index),
					bounds.Height);
			}

			return new Rectangle(bounds.X,
				bounds.Y + InternalChild.GetCellLocationY(index),
				bounds.Width,
				InternalChild.GetRowHeight(index));
		}

		private void DesktopOnContextMenuClosed(object sender, GenericEventArgs<Widget> genericEventArgs)
		{
			OpenMenuItem = null;

			if (!_internalSetSelectedIndex)
			{
				SelectedIndex = HoverIndex = null;
			}
		}

		private void OnHoverIndexChanged(object sender, EventArgs eventArgs)
		{
			var menuItem = GetMenuItem(HoverIndex);
			if (menuItem == null && HoverIndexCanBeNull)
			{
				// Separators couldn't be selected
				HoverIndex = null;
				return;
			}

			if (!IsOpen)
			{
				return;
			}

			if (Desktop.ContextMenu != this && menuItem.CanOpen && OpenMenuItem != menuItem)
			{
				SelectedIndex = HoverIndex;
			}
		}

		private void ShowSubMenu(MenuItem menuItem)
		{
			var bounds = GetItemBounds(menuItem.Index);

			var pos = this is HorizontalMenu ? new Point(bounds.X, bounds.Bottom) : new Point(bounds.Right, bounds.Y);
			pos = ToGlobal(pos);

			Desktop.ShowContextMenu(menuItem.SubMenu, pos);
			OpenMenuItem = menuItem;
		}

		private void OnSelectedIndexChanged(object sender, EventArgs e)
		{
			if (OpenMenuItem != null)
			{
				try
				{
					_internalSetSelectedIndex = true;

					if (Desktop.ContextMenu != this)
					{
						Desktop.HideContextMenu();
					}
				}
				finally
				{
					_internalSetSelectedIndex = false;
				}
			}

			var menuItem = SelectedMenuItem;
			if (menuItem != null && menuItem.CanOpen)
			{
				ShowSubMenu(menuItem);
			}
		}

		private void InternalChild_TouchUp(object sender, EventArgs e)
		{
			var menuItem = SelectedMenuItem;
			if (menuItem != null && !menuItem.CanOpen)
			{
				Close();
				menuItem.FireSelected();
			}
		}

		private MenuItem GetMenuItem(int? index)
		{
			if (index == null)
			{
				return null;
			}

			return Items[index.Value] as MenuItem;
		}

		private void Click(int? index)
		{
			var menuItem = GetMenuItem(index);
			if (menuItem == null)
			{
				return;
			}

			if (!menuItem.CanOpen)
			{
				Close();
				menuItem.FireSelected();
			}
			else
			{
				SelectedIndex = HoverIndex = index;
			}
		}

		public override void OnKeyDown(Keys k)
		{
			if (k == Keys.Enter || k == Keys.Space)
			{
				int? selectedIndex = HoverIndex;
				if (selectedIndex != null)
				{
					var menuItem = Items[selectedIndex.Value] as MenuItem;
					if (menuItem != null && !menuItem.CanOpen)
					{
						Click(menuItem.Index);
						return;
					}
				}
			}

			var ch = k.ToChar(false);
			if (ch != null)
			{
				var c = char.ToLower(ch.Value);
				foreach (var w in Items)
				{
					var menuItem = w as MenuItem;
					if (menuItem == null)
					{
						continue;
					}

					if (menuItem.UnderscoreChar == c)
					{
						Click(menuItem.Index);
						return;
					}
				}
			}

			if (OpenMenuItem != null)
			{
				OpenMenuItem.SubMenu.OnKeyDown(k);
			}
		}

		public void MoveHover(int delta)
		{
			if (Items.Count == 0)
			{
				return;
			}

			// First step - determine index of currently selected item
			var si = SelectedIndex;
			if (si == null)
			{
				si = HoverIndex;
			}
			var hoverIndex = si != null ? si.Value : -1;
			var oldHover = hoverIndex;

			var iterations = 0;
			while (true)
			{
				if (iterations > Items.Count)
				{
					return;
				}

				hoverIndex += delta;

				if (hoverIndex < 0)
				{
					hoverIndex = Items.Count - 1;
				}

				if (hoverIndex >= Items.Count)
				{
					hoverIndex = 0;
				}

				if (Items[hoverIndex] is MenuItem)
				{
					break;
				}

				++iterations;
			}

			var menuItem = Items[hoverIndex] as MenuItem;
			if (menuItem != null)
			{
				HoverIndex = menuItem.Index;
			}
		}

		private void UpdateGrid()
		{
			if (!_dirty)
			{
				return;
			}

			var index = 0;
			var hasImage = HasImage;
			var hasShortcut = HasShortcut;

			var separatorSpan = 1;
			if (hasImage)
			{
				++separatorSpan;
			}
			if (hasShortcut)
			{
				++separatorSpan;
			}

			foreach (var item in Items)
			{
				var menuItem = item as MenuItem;
				if (menuItem != null)
				{
					if (Orientation == Orientation.Horizontal)
					{
						Grid.SetColumn(menuItem.Label, index);
						Grid.SetRow(menuItem.Label, 0);
					}
					else
					{
						var colIndex = 0;
						if (hasImage)
						{
							Grid.SetColumn(menuItem.ImageWidget, colIndex++);
							Grid.SetRow(menuItem.ImageWidget, index);
						}

						Grid.SetColumn(menuItem.Label, colIndex++);
						Grid.SetRow(menuItem.Label, index);

						if (hasShortcut)
						{
							Grid.SetColumn(menuItem.Shortcut, colIndex++);
							Grid.SetRow(menuItem.Shortcut, index);
						}
					}
				}
				else
				{
					var separator = (MenuSeparator)item;
					if (Orientation == Orientation.Horizontal)
					{
						Grid.SetColumn(separator.Separator, index);
						Grid.SetRow(separator.Separator, 0);
					}
					else
					{
						Grid.SetColumn(separator.Separator, 0);
						Grid.SetRow(separator.Separator, index);
						Grid.SetColumnSpan(separator.Separator, separatorSpan);
					}
				}

				item.Index = index;

				++index;
			}

			_dirty = false;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			UpdateGrid();
			return base.InternalMeasure(availableSize);
		}

		public void ApplyMenuStyle(MenuStyle style)
		{
			var clone = new MenuStyle(style);

			ApplyWidgetStyle(clone);

			MenuStyle = clone;

			InternalChild.SelectionHoverBackground = style.SelectionHoverBackground;
			InternalChild.SelectionBackground = style.SelectionBackground;
		}
	}
}