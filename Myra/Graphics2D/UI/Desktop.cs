using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.Text;
using Myra.Input;
using Myra.Utility;

namespace Myra.Graphics2D.UI
{
	public class Desktop
	{
		private SpriteBatch _batch;
		private bool _layoutDirty = true;
		private Rectangle _bounds;
		protected readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();

		public ObservableCollection<Widget> Widgets
		{
			get { return _widgets; }
		}

		public Rectangle Bounds
		{
			get { return _bounds; }

			set
			{
				if (value == _bounds)
				{
					return;
				}

				_bounds = value;
				InvalidateLayout();
			}
		}

		public Menu ContextMenu { get; private set; }

		public Desktop()
		{
			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

			InputAPI.MouseDown += InputOnMouseDown;
			InputAPI.MouseUp += InputOnMouseUp;
		}

		private void InputOnMouseUp(object sender, GenericEventArgs<MouseButtons> genericEventArgs)
		{
		}

		private void InputOnMouseDown(object sender, GenericEventArgs<MouseButtons> genericEventArgs)
		{
			if (ContextMenu != null && !ContextMenu.Bounds.Contains(InputAPI.MousePosition))
			{
				HideContextMenu();
			}
		}

		public void ShowContextMenu(Menu menu, Point position)
		{
			if (menu == null)
			{
				throw new ArgumentNullException("menu");
			}

			HideContextMenu();

			ContextMenu = menu;

			if (ContextMenu != null)
			{
				ContextMenu.HorizontalAlignment = HorizontalAlignment.Left;
				ContextMenu.VerticalAlignment = VerticalAlignment.Top;

				ContextMenu.XHint = position.X;
				ContextMenu.YHint = position.Y;

				ContextMenu.Visible = true;

				_widgets.Add(ContextMenu);
			}
		}

		public void HideContextMenu()
		{
			if (ContextMenu != null)
			{
				_widgets.Remove(ContextMenu);

				ContextMenu.Visible = false;
				ContextMenu = null;
			}
		}

		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (Widget w in args.NewItems)
				{
					w.Desktop = this;
					w.MeasureChanged += WOnMeasureChanged;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Widget w in args.OldItems)
				{
					w.MeasureChanged -= WOnMeasureChanged;
					w.Desktop = null;
				}
			}

			InvalidateLayout();
		}

		private void WOnMeasureChanged(object sender, EventArgs eventArgs)
		{
			InvalidateLayout();
		}

		public void Render(GraphicsDevice device)
		{
			InputAPI.Update();

			UpdateLayout();

			if (_batch == null)
			{
				_batch = new SpriteBatch(device);
			}

			_batch.BeginUI();

			foreach (var widget in _widgets)
			{
				if (widget.Visible)
				{
					widget.Render(_batch);
				}
			}

			_batch.End();
		}

		public void InvalidateLayout()
		{
			_layoutDirty = true;
		}

		public void UpdateLayout()
		{
			if (!_layoutDirty)
			{
				return;
			}

			foreach (var widget in _widgets)
			{
				if (widget.Visible)
				{
					widget.LayoutChild(_bounds);
				}
			}

			_layoutDirty = false;
		}
	}
}