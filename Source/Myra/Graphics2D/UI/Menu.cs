using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class Menu : GridBased, IMenuItemsContainer
	{
		private ObservableCollection<IMenuItem> _items;

		[HiddenInEditor]
		[JsonIgnore]
		public abstract Orientation Orientation { get; }

		[HiddenInEditor]
		[JsonIgnore]
		public MenuItemStyle MenuItemStyle { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public MenuSeparatorStyle SeparatorStyle { get; private set; }

		private MenuItemButton ParentMenuItemButton { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public MenuItemButton OpenMenuItem { get; private set; }

		[HiddenInEditor]
		public ObservableCollection<IMenuItem> Items
		{
			get { return _items; }

			internal set
			{
				if (_items == value)
				{
					return;
				}

				if (_items != null)
				{
					_items.CollectionChanged -= ItemsOnCollectionChanged;

					foreach (var menuItem in _items)
					{
						RemoveItem(menuItem);
					}
				}

				_items = value;

				if (_items != null)
				{
					_items.CollectionChanged += ItemsOnCollectionChanged;

					foreach (var menuItem in _items)
					{
						AddItem(menuItem);
					}
				}
			}
		}

		protected Menu(MenuStyle style)
		{
			CanFocus = true;
			OpenMenuItem = null;

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			if (style != null)
			{
				ApplyMenuStyle(style);
			}

			Items = new ObservableCollection<IMenuItem>();
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
		}

		/// <summary>
		/// Recursively search for the menu item by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns>null if not found</returns>
		public MenuItem FindMenuItemById(string id)
		{
			foreach (var item in _items)
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

		private void UpdateGridPositions()
		{
			for (var i = 0; i < Widgets.Count; ++i)
			{
				var widget = Widgets[i];
				if (Orientation == Orientation.Horizontal)
				{
					widget.GridPositionX = i;
				}
				else
				{
					widget.GridPositionY = i;
				}
			}
		}

		private void AddItem(Widget menuItem, int index)
		{
			if (Orientation == Orientation.Horizontal)
			{
				ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			}
			else
			{
				menuItem.HorizontalAlignment = HorizontalAlignment.Stretch;
				menuItem.VerticalAlignment = VerticalAlignment.Stretch;
				RowsProportions.Add(new Proportion(ProportionType.Auto));
			}

			Widgets.Insert(index, menuItem);

			UpdateGridPositions();
		}

		private void MenuItemOnChanged(object sender, EventArgs eventArgs)
		{
			var menuItem = (MenuItem)sender;

			var asMenuItemButton = menuItem.Widget as MenuItemButton;
			if (asMenuItemButton == null)
			{
				return;
			}

			asMenuItemButton.Text = menuItem.Text;
			asMenuItemButton.TextColor = menuItem.Color ?? MenuItemStyle.LabelStyle.TextColor;
		}

		private void InsertItem(IMenuItem iMenuItem, int index)
		{
			var menuItem = iMenuItem as MenuItem;
			Widget widget;
			if (menuItem != null)
			{
				menuItem.Changed += MenuItemOnChanged;
				var menuItemButton = new MenuItemButton(MenuItemStyle)
				{
					Text = menuItem.Text,
					Tag = menuItem
				};

				if (menuItem.Color.HasValue)
				{
					menuItemButton.TextColor = menuItem.Color.Value;
				}

				menuItemButton.Down += ButtonOnDown;
				menuItemButton.Up += ButtonOnUp;
				menuItemButton.SubMenu.Items = menuItem.Items;
				menuItemButton.Toggleable = menuItem.Items.Count > 0;

				widget = menuItemButton;
			}
			else
			{
				widget = new MenuSeparatorWidget(Orientation, SeparatorStyle);
			}

			iMenuItem.Menu = this;
			iMenuItem.Widget = widget;

			AddItem(widget, index);
			UpdateGridPositions();
		}

		private void AddItem(IMenuItem item)
		{
			InsertItem(item, Widgets.Count);
		}

		private void RemoveItem(IMenuItem iMenuItem)
		{
			var menuItem = iMenuItem as MenuItem;
			if (menuItem != null)
			{
				menuItem.Changed -= MenuItemOnChanged;
			}

			var widget = iMenuItem.Widget;

			if (widget == null)
			{
				return;
			}

			var asMenuItemButton = widget as MenuItemButton;
			if (asMenuItemButton != null)
			{
				asMenuItemButton.Down -= ButtonOnDown;
				asMenuItemButton.Up -= ButtonOnUp;
			}

			var index = Widgets.IndexOf(widget);

			if (Orientation == Orientation.Horizontal)
			{
				ColumnsProportions.RemoveAt(index);
			}
			else
			{
				RowsProportions.RemoveAt(index);
			}

			Widgets.RemoveAt(index);
			UpdateGridPositions();
		}

		private void ButtonOnUp(object sender, EventArgs eventArgs)
		{
			if (Desktop == null)
			{
				return;
			}

			Desktop.HideContextMenu();

			var menuItem = (MenuItemButton)sender;
			if (menuItem.SubMenu == null || menuItem.SubMenu.Items.Count == 0)
			{
				((MenuItem)menuItem.Tag).FireSelected();
			}
			else
			{
				menuItem.IsSubMenuOpen = false;
				OpenMenuItem = null;
			}
		}

		private void ShowSubMenu(MenuItemButton menuItem)
		{
			if (menuItem == null || menuItem.SubMenu == null || menuItem.SubMenu.Items.Count == 0)
			{
				return;
			}

			Desktop.ShowContextMenu(menuItem.SubMenu, new Point(menuItem.Bounds.X, Bounds.Bottom));
			Desktop.ContextMenuClosed += DesktopOnContextMenuClosed;

			OpenMenuItem = menuItem;
			menuItem.IsSubMenuOpen = true;
		}

		public override void OnDesktopChanging()
		{
			if (Desktop == null)
			{
				return;
			}

			Desktop.ContextMenuClosing -= DesktopOnContextMenuClosing;
			Desktop.ContextMenuClosed -= DesktopOnContextMenuClosed;
		}

		public override void OnDesktopChanged()
		{
			if (Desktop == null)
			{
				return;
			}

			Desktop.ContextMenuClosing += DesktopOnContextMenuClosing;
			Desktop.ContextMenuClosed += DesktopOnContextMenuClosed;
		}

		private void DesktopOnContextMenuClosing(object sender, ContextMenuClosingEventArgs args)
		{
			// Prevent closing/opening of the context menu
			if (OpenMenuItem != null && OpenMenuItem.Bounds.Contains(Desktop.MousePosition))
			{
				args.Cancel = true;
			}
		}

		private void DesktopOnContextMenuClosed(object sender, GenericEventArgs<Widget> genericEventArgs)
		{
			if (OpenMenuItem == null) return;

			OpenMenuItem.IsPressed = false;
		}

		private void ButtonOnDown(object sender, EventArgs args)
		{
			var menuItem = (MenuItemButton)sender;

			if (menuItem.SubMenu == null || menuItem.SubMenu.Items.Count == 0)
			{
				return;
			}

			ShowSubMenu(menuItem);
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			if (Desktop == null || OpenMenuItem == null)
			{
				return;
			}

			foreach (var widget in Widgets)
			{
				var menuItem = widget as MenuItemButton;
				if (menuItem != null && menuItem.SubMenu != null && menuItem.Bounds.Contains(position) &&
					OpenMenuItem != menuItem)
				{
					Select((MenuItem)widget.Tag);
				}
			}
		}

		public override void OnKeyDown(Keys k)
		{
			if (!GlyphRenderOptions.DrawUnderscores)
			{
				return;
			}

			var ch = k.ToChar(false);
			if (ch == null)
			{
				return;
			}

			foreach (var w in Widgets)
			{
				var button = w as MenuItemButton;
				if (button == null)
				{
					continue;
				}
				

				if (button.UnderscoreChar == ch)
				{
					Desktop.HideContextMenu();
					MenuItem menuItem = (MenuItem)button.Tag;
					menuItem.FireSelected();
					break;
				}
			}
		}

		public void Select(IMenuItem menuItem)
		{
			if (OpenMenuItem != null)
			{
				OpenMenuItem.IsPressed = false;
			}

			var asButton = menuItem.Widget as MenuItemButton;
			if (asButton != null)
			{
				asButton.IsPressed = true;
			}
		}

		public void ApplyMenuStyle(MenuStyle style)
		{
			ApplyWidgetStyle(style);

			MenuItemStyle = style.MenuItemStyle;
			SeparatorStyle = style.SeparatorStyle;
		}
	}
}