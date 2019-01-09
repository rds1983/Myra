using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Myra.Graphics2D.UI
{
	public abstract class Container : Widget
	{
		private bool _childrenDirty = true;
		private readonly List<Widget> _childrenCopy = new List<Widget>();

		[JsonIgnore]
		[HiddenInEditor]
		public abstract int ChildrenCount { get; }

		internal List<Widget> ChildrenCopy
		{
			get
			{
				// We return copy of our collection
				// To prevent exception when someone modifies the collection during the iteration
				UpdateWidgets();

				return _childrenCopy;
			}
		}

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

				foreach (var item in ChildrenCopy)
				{
					item.Enabled = value;
				}
			}
		}

		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}
			set
			{
				base.Desktop = value;

				foreach (var child in ChildrenCopy)
				{
					child.Desktop = Desktop;
				}
			}
		}

		public abstract Widget GetChild(int index);

		private void UpdateWidgets()
		{
			if (!_childrenDirty)
			{
				return;
			}

			_childrenCopy.Clear();

			for (var i = 0; i < ChildrenCount; ++i)
			{
				_childrenCopy.Add(GetChild(i));
			}

			_childrenDirty = false;
		}

		protected void InvalidateChildren()
		{
			_childrenDirty = true;
		}

		public override void OnMouseEntered()
		{
			base.OnMouseEntered();

			ChildrenCopy.HandleMouseMovement();
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			foreach (var w in ChildrenCopy)
			{
				if (!w.Visible)
				{
					continue;
				}

				if (w.WasMouseOver)
				{
					w.OnMouseLeft();
				}
			}
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			ChildrenCopy.HandleMouseMovement();
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			ChildrenCopy.HandleMouseDown(mb);
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			ChildrenCopy.HandleMouseUp(mb);
		}

		public override void OnMouseDoubleClick(MouseButtons mb)
		{
			base.OnMouseDoubleClick(mb);

			ChildrenCopy.HandleMouseDoubleClick(mb);
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			ChildrenCopy.HandleTouchDown();
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			ChildrenCopy.HandleTouchUp();
		}

		internal override void MoveChildren(Point delta)
		{
			base.MoveChildren(delta);

			foreach (var child in ChildrenCopy)
			{
				if (!child.Visible)
					continue;

				child.MoveChildren(delta);
			}
		}

		public override void InternalRender(RenderContext batch)
		{
			foreach (var child in ChildrenCopy)
			{
				if (!child.Visible)
					continue;

				child.Render(batch);
			}
		}

		public int CalculateTotalChildCount(bool visibleOnly)
		{
			var result = ChildrenCopy.Count;

			foreach (var child in ChildrenCopy)
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

		public abstract void RemoveChild(Widget widget);
	}
}