using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.MML;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyraPad
{
	internal class ChildCreator
	{
		public string Name { get; }
		public Func<object, IItemWithId> Creator { get; }

		public ChildCreator(string name, Func<object, IItemWithId> creator)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			Name = name;
			Creator = creator ?? throw new ArgumentNullException(nameof(creator));
		}
	}

	internal static class StringExtensions
	{
		/// <summary>
		/// Returns the character at the index of string, or returns null when out of range and does not throw exception.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		internal static char? GetCharSafely(this string @this, int index)
		{
			if (index < @this.Length && index >= 0)
			{
				return @this[index];
			}

			return null;
		}

		/// <summary>
		/// Returns a substring of this string, or returns null when out of range and does not throw exception.
		/// </summary>
		/// <param name="this"></param>
		/// <param name="index"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		internal static string SubstringSafely(this string @this, int index, int count)
		{
			if (count <= 0)
			{
				return string.Empty;
			}

			if (index < 0)
			{
				return null;
			}

			if (index >= @this.Length)
			{
				return null;
			}

			if (index + count >= @this.Length)
			{
				return null;
			}

			return @this.Substring(index, count);

		}

		public static int LastIndexOfSafely(this string @this, char character, int index)
		{
			if (index < 0 || index >= @this.Length)
			{
				return -1;
			}

			return @this.LastIndexOf(character, index);
		}

		public static int IndexOfSafely(this string @this, char character, int index)
		{
			if (index < 0 || index >= @this.Length)
			{
				return -1;
			}

			return @this.IndexOf(character, index);
		}

		public static bool Contains(this IEnumerable<Type> types, string name)
		{
			return (from t in types where t.Name == name select t).FirstOrDefault() != null;
		}

		public static string[] ToStringList(this IEnumerable<Type> types)
		{
			return (from t in types select t.Name).ToArray();
		}

		public static ChildCreator ToCreator(this Type t)
		{
			Func<object, IItemWithId> creator = parent =>
			{
				IItemWithId child;

				var constructor = t.GetConstructor(Type.EmptyTypes);
				if (constructor != null)
				{
					child = (IItemWithId)Activator.CreateInstance(t);
				}
				else
				{
					// Try with stylename constructor
					child = (IItemWithId)Activator.CreateInstance(t, Stylesheet.DefaultStyleName);
				}

				do
				{
					var asContentControl = parent as IContent;
					if (asContentControl != null)
					{
						asContentControl.Content = (Widget)child;
						break;
					}

					var asContainer = parent as IContainer;
					if (asContainer != null)
					{
						asContainer.Widgets.Add((Widget)child);
						break;
					}

					var asMenu = parent as Menu;
					if (asMenu != null)
					{
						asMenu.Items.Add((IMenuItem)child);
						break;
					}

					var asTabControl = parent as TabControl;
					if (asTabControl != null)
					{
						asTabControl.Items.Add((TabItem)child);
						break;
					}
				}
				while (false);

				return child;
			};

			return new ChildCreator(t.Name, creator);
		}

		public static ChildCreator[] ToCreators(this IEnumerable<Type> types)
		{
			var result = new List<ChildCreator>();

			foreach (var t in types)
			{
				var creator = t.ToCreator();

				if (creator == null)
				{
					continue;
				}

				result.Add(creator);
			}

			return result.ToArray();
		}
	}
}