using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class Container : Widget
	{
		[JsonIgnore]
		[HiddenInEditor]
		public abstract IEnumerable<Widget> Children { get; }

		[JsonIgnore]
		[HiddenInEditor]
		public abstract IEnumerable<Widget> ReverseChildren { get; }

		[JsonIgnore]
		[HiddenInEditor]
		public abstract int ChildCount { get; }

		public override bool Enabled
		{
			get { return base.Enabled; }

			set
			{
				if (base.Enabled == value)
				{
					return;
				}

				base.Enabled = value;

				foreach (var item in Children)
				{
					item.Enabled = value;
				}
			}
		}

		public override void OnMouseEntered(Point position)
		{
			base.OnMouseEntered(position);

			Children.HandleMouseMovement(position);
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			foreach (var w in Children)
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

			ReverseChildren.HandleMouseMovement(position);
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			ReverseChildren.HandleMouseDown(mb);
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			ReverseChildren.HandleMouseUp(mb);
		}

		internal override void MoveChildren(Point delta)
		{
			base.MoveChildren(delta);

			foreach (var child in Children)
			{
				if (!child.Visible)
					continue;

				child.MoveChildren(delta);
			}
		}

		public override void InternalRender(RenderContext batch)
		{
			foreach (var child in Children)
			{
				if (!child.Visible)
					continue;

				child.Render(batch);
			}
		}

		public override void OnDesktopChanged()
		{
			base.OnDesktopChanged();

			foreach (var child in Children)
			{
				child.Desktop = Desktop;
			}
		}

		public int CalculateTotalChildCount(bool visibleOnly)
		{
			var result = ChildCount;

			foreach (var child in Children)
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