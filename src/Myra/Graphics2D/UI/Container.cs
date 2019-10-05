using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework;
#else
using Xenko.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class Container : Control
	{
		private bool _childrenDirty = true;
		private readonly List<Control> _childrenCopy = new List<Control>();

		[XmlIgnore]
		[Browsable(false)]
		public abstract int ChildrenCount { get; }

		internal List<Control> ChildrenCopy
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

		public abstract Control GetChild(int index);

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
			InvalidateMeasure();
			_childrenDirty = true;
		}

		public override void OnMouseEntered()
		{
			base.OnMouseEntered();

			foreach (var w in ChildrenCopy)
			{
				if (!w.Visible)
				{
					continue;
				}

				w.HandleMouseMovement();
			}
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

				w.HandleMouseMovement();
			}
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			foreach (var w in ChildrenCopy)
			{
				w.HandleMouseMovement();
			}
		}

		public override void OnTouchDoubleClick()
		{
			base.OnTouchDoubleClick();

			foreach (var w in ChildrenCopy)
			{
				w.HandleTouchDoubleClick();
			}
		}

		public override void OnTouchEntered()
		{
			base.OnTouchEntered();

			foreach (var w in ChildrenCopy)
			{
				w.HandleTouchMovement();
			}
		}

		public override void OnTouchLeft()
		{
			base.OnTouchLeft();

			foreach (var w in ChildrenCopy)
			{
				if (!w.Visible)
				{
					continue;
				}

				w.OnTouchLeft();
			}
		}

		public override void OnTouchMoved()
		{
			base.OnTouchMoved();

			foreach (var w in ChildrenCopy)
			{
				w.HandleTouchMovement();
			}
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			foreach (var w in ChildrenCopy)
			{
				w.HandleTouchDown();
			}
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			foreach (var w in ChildrenCopy)
			{
				w.HandleTouchUp();
			}
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

		public abstract void RemoveChild(Control widget);
	}
}