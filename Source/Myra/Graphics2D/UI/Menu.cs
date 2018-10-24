using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
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
		public SeparatorStyle SeparatorStyle { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public MenuItemButton OpenMenuItem { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public bool IsOpen
		{
			get
			{
				return OpenMenuItem != null;
			}
		}

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

		protected Menu(MenuStyle style): base(style)
		{
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
				menuItemButton.MouseEntered += MouseOnEntered;
				menuItemButton.MouseLeft += MouseOnLeft;
				menuItemButton.SubMenu.Items = menuItem.Items;
				menuItemButton.Toggleable = menuItem.Items.Count > 0;

				widget = menuItemButton;
			}
			else
			{
				if (Orientation == Orientation.Horizontal)
				{
					widget = new VerticalSeparator(SeparatorStyle);
				} else
				{
					widget = new HorizontalSeparator(SeparatorStyle);
				}
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
				asMenuItemButton.MouseEntered -= MouseOnEntered;
				asMenuItemButton.MouseLeft -= MouseOnLeft;
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

		public void Close()
		{
			Desktop.HideContextMenu();
		}

		private void ShowSubMenu(MenuItemButton menuItem)
		{
			if (OpenMenuItem != null)
			{
				OpenMenuItem.IsPressed = false;
			}

			if (menuItem == null || !menuItem.CanOpen)
			{
				return;
			}

			Desktop.ShowContextMenu(menuItem.SubMenu, new Point(menuItem.Bounds.X, Bounds.Bottom));
			Desktop.ContextMenuClosed += DesktopOnContextMenuClosed;

			OpenMenuItem = menuItem;
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

		private void MouseOnEntered(object sender, EventArgs eventArgs)
		{
			if (!IsOpen)
			{
				return;
			}

			var menuItemButton = (MenuItemButton)sender;
			if (menuItemButton.CanOpen && OpenMenuItem != menuItemButton)
			{
				menuItemButton.IsPressed = true;
			}
		}

		private void MouseOnLeft(object sender, EventArgs eventArgs)
		{
		}

		private void ButtonOnDown(object sender, EventArgs eventArgs)
		{
			if (Desktop == null)
			{
				return;
			}

			var menuItemButton = (MenuItemButton)sender;
			if (menuItemButton.CanOpen)
			{
				ShowSubMenu(menuItemButton);
			}
		}

		private void ButtonOnUp(object sender, EventArgs eventArgs)
		{
			if (Desktop == null)
			{
				return;
			}

			Close();

			var menuItem = (MenuItemButton)sender;
			if (!menuItem.CanOpen)
			{
				((MenuItem)menuItem.Tag).FireSelected();
			}
			else
			{
				OpenMenuItem = null;
			}
		}

		private void Click(MenuItemButton menuItemButton)
		{
			if (Desktop == null)
			{
				return;
			}

			var menuItem = (MenuItem)menuItemButton.Tag;
			if (!menuItemButton.CanOpen)
			{
				Close();
				menuItem.FireSelected();
			}
			else
			{
				menuItemButton.IsPressed = true;
			}
		}

		private int GetSelectedIndex()
		{
			int selectedIndex = -1;

			if (OpenMenuItem != null)
			{
				selectedIndex = Widgets.IndexOf(OpenMenuItem);
			}
			else
			{
				for (var i = 0; i < Widgets.Count; ++i)
				{
					if (Widgets[i].IsMouseOver)
					{
						selectedIndex = i;
						break;
					}
				}
			}

			return selectedIndex;
		}

		public override void OnKeyDown(Keys k)
		{
			if (!MyraEnvironment.ShowUnderscores)
			{
				return;
			}

			if (k == Keys.Enter || k == Keys.Space)
			{
				int selectedIndex = GetSelectedIndex();
				if (selectedIndex != -1)
				{
					var button = Widgets[selectedIndex] as MenuItemButton;
					if (button != null && !button.CanOpen)
					{
						Click(button);
						return;
					}
				}
			}

			var ch = k.ToChar(false);
			foreach (var w in Widgets)
			{
				var button = w as MenuItemButton;
				if (button == null)
				{
					continue;
				}

				if (ch != null && button.UnderscoreChar == ch)
				{
					Click(button);
					return;
				}
			}



			if (OpenMenuItem != null)
			{
				OpenMenuItem.SubMenu.OnKeyDown(k);
			}
		}

		public void MoveSelection(int delta)
		{
			if (Widgets.Count == 0)
			{
				return;
			}

			// First step - determine index of currently selected item
			int selectedIndex = GetSelectedIndex();
			var oldIndex = selectedIndex;

			var iterations = 0;
			while (true)
			{
				if (iterations > Widgets.Count)
				{
					return;
				}

				selectedIndex += delta;

				if (selectedIndex < 0)
				{
					selectedIndex = Widgets.Count - 1;
				}

				if (selectedIndex >= Widgets.Count)
				{
					selectedIndex = 0;
				}

				if (Widgets[selectedIndex] is MenuItemButton)
				{
					break;
				}


				++iterations;
			}

			if (selectedIndex < 0 || selectedIndex >= Widgets.Count || selectedIndex == oldIndex)
			{
				return;
			}

			MenuItemButton menuItemButton;
			if (oldIndex != -1)
			{
				menuItemButton = Widgets[oldIndex] as MenuItemButton;
				if (menuItemButton != null)
				{
					menuItemButton.IsMouseOver = false;
					menuItemButton.IsPressed = false;
				}
			}

			menuItemButton = Widgets[selectedIndex] as MenuItemButton;
			if (menuItemButton != null)
			{
				if (!menuItemButton.CanOpen)
				{
					menuItemButton.IsMouseOver = true;
				}
				else
				{
					menuItemButton.IsPressed = true;
				}
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