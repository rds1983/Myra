using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Myra.Attributes;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public abstract class Container : Widget
	{
		private bool _childrenDirty = true;
		private readonly List<Widget> _childrenCopy = new List<Widget>();
		private readonly List<Widget> _reverseChildrenCopy = new List<Widget>();

		[JsonIgnore]
		[HiddenInEditor]
		public abstract IEnumerable<Widget> Children { get; }

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

		internal List<Widget> ReverseChildrenCopy
		{
			get
			{
				UpdateWidgets();

				return _reverseChildrenCopy;
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

				foreach (var item in Children)
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

				foreach (var child in Children)
				{
					child.Desktop = Desktop;
				}
			}
		}

		private void UpdateWidgets()
		{
			if (!_childrenDirty)
			{
				return;
			}

			_childrenCopy.Clear();
			_childrenCopy.AddRange(Children);

			_reverseChildrenCopy.Clear();
			_reverseChildrenCopy.AddRange(Children);
			_reverseChildrenCopy.Reverse();

			_childrenDirty = false;
		}

		protected void InvalidateChildren()
		{
			_childrenDirty = true;
		}


		public override void OnMouseEntered()
		{
			base.OnMouseEntered();

			Children.HandleMouseMovement();
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

				if (w.WasMouseOver)
				{
					w.OnMouseLeft();
				}
			}
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			ReverseChildrenCopy.HandleMouseMovement();
		}

		public override void OnMouseDown(MouseButtons mb)
		{
			base.OnMouseDown(mb);

			ReverseChildrenCopy.HandleMouseDown(mb);
		}

		public override void OnMouseUp(MouseButtons mb)
		{
			base.OnMouseUp(mb);

			ReverseChildrenCopy.HandleMouseUp(mb);
		}

		public override void OnMouseDoubleClick(MouseButtons mb)
		{
			base.OnMouseDoubleClick(mb);

			ReverseChildrenCopy.HandleMouseDoubleClick(mb);
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			ReverseChildrenCopy.HandleTouchDown();
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			ReverseChildrenCopy.HandleTouchUp();
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

		public int CalculateTotalChildCount(bool visibleOnly)
		{
			var result = ChildrenCopy.Count;

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

		public abstract void RemoveChild(Widget widget);
	}
}