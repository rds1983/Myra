using System.ComponentModel;
using System.Collections.Generic;
using System.Xml.Serialization;
using Myra.Utility;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class Container : Widget
	{
		private bool _childrenDirty = true;
		private readonly List<Widget> _childrenCopy = new List<Widget>();

		[XmlIgnore]
		[Browsable(false)]
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

			internal set
			{
				base.Desktop = value;

				foreach (var child in ChildrenCopy)
				{
					child.Desktop = value;
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

			_childrenCopy.SortWidgetsByZIndex();

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

			ChildrenCopy.ProcessMouseMovement();
		}

		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			ChildrenCopy.ProcessMouseMovement();
		}

		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			ChildrenCopy.ProcessMouseMovement();
		}

		public override void OnTouchEntered()
		{
			base.OnTouchEntered();

			ChildrenCopy.ProcessTouchMovement();
		}

		public override void OnTouchLeft()
		{
			base.OnTouchLeft();

			ChildrenCopy.ProcessTouchMovement();
		}

		public override void OnTouchMoved()
		{
			base.OnTouchMoved();

			ChildrenCopy.ProcessTouchMovement();
		}

		public override void OnTouchDown()
		{
			base.OnTouchDown();

			ChildrenCopy.ProcessTouchDown();
		}

		public override void OnTouchUp()
		{
			base.OnTouchUp();

			ChildrenCopy.ProcessTouchUp();
		}

		public override void OnTouchDoubleClick()
		{
			base.OnTouchDoubleClick();

			ChildrenCopy.ProcessTouchDoubleClick();
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			foreach (var child in ChildrenCopy)
			{
				child.Render(context);
			}
		}

		internal override void InvalidateTransform()
		{
			base.InvalidateTransform();

			foreach (var child in ChildrenCopy)
			{
				child.InvalidateTransform();
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


		/// <summary>
		/// Finds first child widget of type <typeparamref name="WidgetT"/> with specified <paramref name="Id"/>
		/// </summary>
		/// <typeparam name="WidgetT">Widget type</typeparam>
		/// <param name="Id">Id of widget</param>
		/// <returns>Widget instance if found otherwise null</returns>
		public WidgetT FindChildById<WidgetT>(string Id) where WidgetT : Widget
		{
			return FindChild<WidgetT>(this, w => w.Id == Id);
		}

		/// <summary>
		/// Finds first child widget of type <typeparamref name="WidgetT"/>. If <paramref name="predicate"/> is null -
		/// the first widget of <typeparamref name="WidgetT"/> is returned,
		/// otherwise the first widget of <typeparamref name="WidgetT"/> matching <paramref name="predicate"/> is returned.
		/// </summary>
		/// <typeparam name="WidgetT">Widget type</typeparam>
		/// <param name="predicate">Predicate to match on widget</param>
		/// <returns>Widget instance if found otherwise null</returns>
		public WidgetT FindChild<WidgetT>(Func<WidgetT, bool> predicate = null) where WidgetT : Widget
		{
			return FindChild(this, predicate);
		}

		/// <summary>
		/// Finds the first widget with matching <paramref name="Id"/>
		/// </summary>
		/// <param name="Id">Id to match on</param>
		/// <returns>Widget instance if found otherwise null</returns>
		public Widget FindChildById(string Id)
		{
			return FindChild(this, w => w.Id == Id);
		}

		/// <summary>
		/// Finds the first child found by predicate.
		/// </summary>
		/// <param name="predicate">Predicate to match on widget</param>
		/// <returns>Widget instance if found otherwise null</returns>
		public Widget FindChild(Func<Widget, bool> predicate)
		{
			return FindChild(this, predicate);
		}

		/// <summary>
		/// Gets all children in container matching on optional predicate.
		/// </summary>
		/// <param name="recursive">If true, indicates that child containers will also be iterated.</param>
		/// <param name="predicate">Predicate to filter children</param>
		/// <returns>Children found</returns>
		public IEnumerable<Widget> GetChildren(bool recursive = false, Func<Widget, bool> predicate = null)
        {
			return GetChildren(this, recursive, predicate);
        }

		internal static IEnumerable<Widget> GetChildren(Container container, bool recursive = false,
			Func<Widget, bool> predicate = null)
        {
            foreach (var widget in container.ChildrenCopy)
            {
				if (predicate?.Invoke(widget) ?? true)
				{
					yield return widget;
				}

				if (recursive && widget is Container childContainer)
				{
                    foreach (var innerWidget in GetChildren(childContainer, recursive, predicate))
                    {
						yield return innerWidget;
                    }
				}
			}
        }

		internal static WidgetT FindChildById<WidgetT>(Container container, string Id) where WidgetT : Widget
		{
			return FindChild<WidgetT>(container, w => w.Id == Id);
		}

		internal static Widget FindChildById(Container container, string Id)
		{
			return FindChild(container, w => w.Id == Id);
		}

		internal static WidgetT FindChild<WidgetT>(Container container, 
			Func<WidgetT, bool> predicate = null) where WidgetT : Widget
		{
			foreach (var widget in container.ChildrenCopy)
			{
				if (widget is WidgetT casted && (predicate?.Invoke(casted) ?? true))
				{
					return casted;
				}
				else if (widget is Container childContainer)
				{
					var child = FindChild(childContainer, predicate);

					if (child != null)
					{
						return child;
					}
				}
			}

			return null;
		}

		internal static Widget FindChild(Container container, Func<Widget, bool> predicate = null)
        {
			foreach (var widget in container.ChildrenCopy)
			{
				if (predicate?.Invoke(widget) ?? true)
				{
					return widget;
				}
				else if (widget is Container childContainer)
				{
					var child = FindChild(childContainer, predicate);

					if (child != null)
					{
						return child;
					}
				}
			}

			return null;
		}
	}
}