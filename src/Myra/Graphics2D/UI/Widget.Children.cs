using Myra.Attributes;
using Myra.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	partial class Widget
	{
		private bool _childrenDirty = true;
		private readonly List<Widget> _childrenCopy = new List<Widget>();

		[Browsable(false)]
		[XmlIgnore]
		public ILayout ChildrenLayout { get; set; }

		[Browsable(false)]
		[Content]
		protected internal ObservableCollection<Widget> Children { get; } = new ObservableCollection<Widget>();

		protected internal IEnumerable<Widget> ChildrenCopy
		{
			get
			{
				// We return copy of our collection
				// To prevent exception when someone modifies the collection during the iteration
				UpdateChildren();

				return _childrenCopy;
			}
		}

		private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (Widget w in args.NewItems)
				{
					OnChildAdded(w);
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Widget w in args.OldItems)
				{
					OnChildRemoved(w);
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (Widget w in ChildrenCopy)
				{
					OnChildRemoved(w);
				}
			}

			InvalidateChildren();
		}

		protected virtual void OnChildAdded(Widget w)
		{
			w.Desktop = Desktop;
			w.Parent = this;
		}

		protected virtual void OnChildRemoved(Widget w)
		{
			w.Desktop = null;
			w.Parent = null;
		}

		private void UpdateChildren()
		{
			if (!_childrenDirty)
			{
				return;
			}

			_childrenCopy.Clear();

			for (var i = 0; i < Children.Count; ++i)
			{
				_childrenCopy.Add(Children[i]);
			}

			_childrenCopy.SortWidgetsByZIndex();

			_childrenDirty = false;
		}

		private void InvalidateChildren()
		{
			InvalidateMeasure();
			_childrenDirty = true;
		}

		public int CalculateTotalChildCount(bool visibleOnly)
		{
			var result = _childrenCopy.Count;

			foreach (var child in ChildrenCopy)
			{
				if (visibleOnly && !child.Visible)
				{
					continue;
				}

				var asCont = child as Widget;
				if (asCont != null)
				{
					result += asCont.CalculateTotalChildCount(visibleOnly);
				}
			}

			return result;
		}

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
		/// Gets all children in Widget matching on optional predicate.
		/// </summary>
		/// <param name="recursive">If true, indicates that child Widgets will also be iterated.</param>
		/// <param name="predicate">Predicate to filter children</param>
		/// <returns>Children found</returns>
		public IEnumerable<Widget> GetChildren(bool recursive = false, Func<Widget, bool> predicate = null)
		{
			return GetChildren(this, recursive, predicate);
		}

		internal static IEnumerable<Widget> GetChildren(Widget Widget, bool recursive = false,
			Func<Widget, bool> predicate = null)
		{
			foreach (var widget in Widget.ChildrenCopy)
			{
				if (predicate?.Invoke(widget) ?? true)
				{
					yield return widget;
				}

				if (recursive && widget is Widget childWidget)
				{
					foreach (var innerWidget in GetChildren(childWidget, recursive, predicate))
					{
						yield return innerWidget;
					}
				}
			}
		}

		internal static WidgetT FindChildById<WidgetT>(Widget Widget, string Id) where WidgetT : Widget
		{
			return FindChild<WidgetT>(Widget, w => w.Id == Id);
		}

		internal static Widget FindChildById(Widget Widget, string Id)
		{
			return FindChild(Widget, w => w.Id == Id);
		}

		internal static WidgetT FindChild<WidgetT>(Widget Widget,
			Func<WidgetT, bool> predicate = null) where WidgetT : Widget
		{
			foreach (var widget in Widget.ChildrenCopy)
			{
				if (widget is WidgetT casted && (predicate?.Invoke(casted) ?? true))
				{
					return casted;
				}
				else if (widget is Widget childWidget)
				{
					var child = FindChild(childWidget, predicate);

					if (child != null)
					{
						return child;
					}
				}
			}

			return null;
		}

		internal static Widget FindChild(Widget Widget, Func<Widget, bool> predicate = null)
		{
			foreach (var widget in Widget.ChildrenCopy)
			{
				if (predicate?.Invoke(widget) ?? true)
				{
					return widget;
				}
				else if (widget is Widget childWidget)
				{
					var child = FindChild(childWidget, predicate);

					if (child != null)
					{
						return child;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Finds the first widget with matching <paramref name="Id"/>
		/// </summary>
		/// <param name="Id">Id to match on</param>
		/// <returns>Widget instance if found otherwise null</returns>

		[Obsolete("Use FindChildById")]
		public Widget FindWidgetById(string Id) => FindChildById(Id);
	}
}
