using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class Container : Widget
	{
		[JsonIgnore]
		public abstract IEnumerable<Widget> Items { get; }

		[JsonIgnore]
		public abstract int ChildCount { get; }

		protected Container()
		{
			LocationChanged += OnLocationChanged;
		}

		private void OnLocationChanged(object sender, EventArgs eventArgs)
		{
			// Update children locations
			foreach (var child in Items)
			{
				if (!child.Visible)
					continue;

				child.Location = Location + child.LocationInParent;
			}
		}

		public override void OnMouseEntered(Point position)
		{
			base.OnMouseEntered(position);

			Items.HandleMouseMovement(position);
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			foreach (var w in Items)
			{
				if (!w.Visible)
				{
					continue;
				}

				if (w.IsMouseOver)
				{
					w.OnMouseLeft();
				}
			}
		}

		public override void OnMouseMoved(Point position)
		{
			base.OnMouseMoved(position);

			Items.HandleMouseMovement(position);
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			Items.HandleMouseDown(mb);
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			Items.HandleMouseUp(mb);
		}

		public override void InternalRender(SpriteBatch batch)
		{
			var bounds = ClientBounds;

			foreach (var child in Items)
			{
				if (!child.Visible)
					continue;

				var childBounds = child.Bounds;

				// Render if.Visible
				if (childBounds.Intersects(bounds))
				{
					child.Render(batch);
				}
			}
		}

		public override void UpdateLayout()
		{
			base.UpdateLayout();

			foreach (var child in Items)
			{
				if (!child.Visible)
					continue;

				child.UpdateLayout();
			}
		}

		public override void OnDesktopChanged()
		{
			base.OnDesktopChanged();

			foreach (var child in Items)
			{
				child.Desktop = Desktop;
			}
		}

		public int CalculateTotalChildCount(bool visibleOnly)
		{
			var result = ChildCount;

			foreach (var child in Items)
			{
				if (visibleOnly && !child.Visible)
				{
					continue;
				}

				var asCont = child as Container;
				if (asCont != null)
				{
					result += asCont.CalculateTotalChildCount(visibleOnly);
				}
			}

			return result;
		}
	}
}