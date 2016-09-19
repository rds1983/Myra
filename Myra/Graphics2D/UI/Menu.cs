using System;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Text;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class Menu : SingleItemContainer<Grid>
	{
		private readonly Orientation _orientation;

		public Orientation Orientation
		{
			get { return _orientation; }
		}

		public MenuItemStyle MenuItemStyle { get; private set; }
		public MenuSeparatorStyle SeparatorStyle { get; private set; }

		public Menu(Orientation orientation, MenuStyle style)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Widget = new Grid();

			_orientation = orientation;

			if (style != null)
			{
				ApplyMenuStyle(style);
			}
		}

		public Menu(Orientation orientation)
			: this(
				orientation,
				orientation == Orientation.Horizontal
					? Stylesheet.Current.HorizontalMenuStyle
					: Stylesheet.Current.VerticalMenuStyle)
		{
		}

		private void AddItem(Widget menuItem)
		{
			var pos = Widget.Children.Count;

			if (_orientation == Orientation.Horizontal)
			{
				menuItem.GridPosition.X = pos;
				Widget.ColumnsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			}
			else
			{
				menuItem.HorizontalAlignment = HorizontalAlignment.Stretch;
				menuItem.VerticalAlignment = VerticalAlignment.Stretch;
				menuItem.GridPosition.Y = pos;
				Widget.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			}

			Widget.Children.Add(menuItem);
		}

		public void AddMenuItem(MenuItem menuItem)
		{
			menuItem.ApplyMenuItemStyle(MenuItemStyle);
			menuItem.Down += ButtonOnDown;

			AddItem(menuItem);
		}

		public void AddSeparator()
		{
			var separator = new MenuSeparator(_orientation, SeparatorStyle);
			AddItem(separator);
		}

		private void ButtonOnDown(object sender, EventArgs args)
		{
			var menuItem = (MenuItem) sender;

			if (menuItem.SubMenu == null)
			{
				return;
			}

			if (menuItem.IsPressed)
			{
				Desktop.ShowContextMenu(menuItem.SubMenu, new Point(menuItem.Bounds.X, Bounds.Bottom));
			}
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			if (Desktop == null || Desktop.ContextMenu == null)
			{
				return;
			}

			foreach (var widget in Widget.Children)
			{
				var menuItem = widget as MenuItem;
				if (menuItem != null && menuItem.SubMenu != null && menuItem.Bounds.Contains(position) &&
				    Desktop.ContextMenu != menuItem.SubMenu)
				{
					Desktop.ShowContextMenu(menuItem.SubMenu, new Point(menuItem.Bounds.X, Bounds.Bottom));
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