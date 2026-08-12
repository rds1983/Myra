using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Myra.MML
{
	/// <summary>
	/// Interface for objects that can be notified when attached properties change.
	/// </summary>
	public interface INotifyAttachedPropertyChanged
	{
		/// <summary>
		/// Called when an attached property on this object has changed.
		/// </summary>
		/// <param name="propertyInfo">Information about the attached property that changed.</param>
		void OnAttachedPropertyChanged(BaseAttachedPropertyInfo propertyInfo);
	}

	/// <summary>
	/// Specifies options for how an attached property affects layout and measurement.
	/// </summary>
	public enum AttachedPropertyOption
	{
		/// <summary>The property change does not affect layout.</summary>
		None,

		/// <summary>The property change affects the arrange phase of layout.</summary>
		AffectsArrange,

		/// <summary>The property change affects the measure phase of layout.</summary>
		AffectsMeasure,
	}

	/// <summary>
	/// Base class for attached property metadata.
	/// </summary>
	public abstract class BaseAttachedPropertyInfo
	{
		/// <summary>
		/// Gets the type that owns this attached property.
		/// </summary>
		public Type OwnerType { get; private set; }

		/// <summary>
		/// Gets the unique identifier for this attached property.
		/// </summary>
		public int Id { get; private set; }

		/// <summary>
		/// Gets the name of the attached property.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the option that specifies how this property affects layout.
		/// </summary>
		public AttachedPropertyOption Option { get; private set; }

		/// <summary>
		/// Gets the attributes associated with this property.
		/// </summary>
		public Attribute[] Attributes { get; private set; }

		/// <summary>
		/// Gets the type of the property value.
		/// </summary>
		public abstract Type PropertyType { get; }

		/// <summary>
		/// Gets the default value for this property as an object.
		/// </summary>
		public abstract object DefaultValueObject { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseAttachedPropertyInfo"/> class.
		/// </summary>
		/// <param name="id">The unique identifier for the property.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="ownerType">The type that owns the property.</param>
		/// <param name="option">The option specifying how the property affects layout.</param>
		/// <param name="attributes">Optional attributes for the property.</param>
		protected BaseAttachedPropertyInfo(int id, string name, Type ownerType, AttachedPropertyOption option, Attribute[] attributes = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
			Name = name;
			Id = id;
			Option = option;
			Attributes = attributes;
		}

		/// <summary>
		/// Returns <c>true</c> if the specified object has a value stored for this attached property.
		/// </summary>
		/// <param name="obj">The object to check.</param>
		/// <returns><c>true</c> if a value exists; otherwise <c>false</c>.</returns>
		public bool HasValue(BaseObject obj) => obj.AttachedPropertiesValues.ContainsKey(Id);

		/// <summary>
		/// Gets the property value from the specified object.
		/// </summary>
		/// <param name="obj">The object to get the property value from.</param>
		/// <returns>The property value as an object.</returns>
		public abstract object GetValueObject(BaseObject obj);

		/// <summary>
		/// Sets the property value on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the property value on.</param>
		/// <param name="value">The property value to set.</param>
		public abstract void SetValueObject(BaseObject obj, object value);
	}

	/// <summary>
	/// Generic attached property metadata and accessor.
	/// </summary>
	/// <typeparam name="T">The type of the property value.</typeparam>
	public class AttachedPropertyInfo<T> : BaseAttachedPropertyInfo
	{
		/// <summary>
		/// Gets the default value for this property.
		/// </summary>
		public T DefaultValue { get; private set; }

		/// <summary>
		/// Gets the type of the property value.
		/// </summary>
		public override Type PropertyType => typeof(T);

		/// <summary>
		/// Gets the default value for this property as an object.
		/// </summary>
		public override object DefaultValueObject => DefaultValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="AttachedPropertyInfo{T}"/> class.
		/// </summary>
		/// <param name="id">The unique identifier for the property.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="ownerType">The type that owns the property.</param>
		/// <param name="defaultValue">The default value for the property.</param>
		/// <param name="option">The option specifying how the property affects layout.</param>
		/// <param name="attributes">Optional attributes for the property.</param>
		public AttachedPropertyInfo(int id, string name, Type ownerType, T defaultValue, AttachedPropertyOption option, Attribute[] attributes = null) :
			base(id, name, ownerType, option, attributes)
		{
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the property value from the specified object.
		/// </summary>
		/// <param name="obj">The object to get the property value from.</param>
		/// <returns>The property value, or the default value if not set.</returns>
		public T GetValue(BaseObject obj)
		{
			if (obj.AttachedPropertiesValues.TryGetValue(Id, out var value))
			{
				return (T)value;
			}

			return DefaultValue;
		}

		/// <summary>
		/// Sets the property value on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the property value on.</param>
		/// <param name="value">The property value to set.</param>
		public void SetValue(BaseObject obj, T value)
		{
			if (GetValue(obj).Equals(value))
			{
				return;
			}

			obj.AttachedPropertiesValues[Id] = value;

			var asWidget = obj as Widget;
			if (asWidget != null)
			{
				switch (Option)
				{
					case AttachedPropertyOption.None:
						break;
					case AttachedPropertyOption.AffectsArrange:
						asWidget.InvalidateArrange();
						break;
					case AttachedPropertyOption.AffectsMeasure:
						asWidget.InvalidateMeasure();
						break;
				}
			}

			obj.OnAttachedPropertyChanged(this);
		}

		/// <summary>
		/// Gets the property value from the specified object as an object.
		/// </summary>
		/// <param name="widget">The object to get the property value from.</param>
		/// <returns>The property value as an object.</returns>
		public override object GetValueObject(BaseObject widget) => GetValue(widget);

		/// <summary>
		/// Sets the property value on the specified object from an object.
		/// </summary>
		/// <param name="widget">The object to set the property value on.</param>
		/// <param name="value">The property value to set as an object.</param>
		public override void SetValueObject(BaseObject widget, object value) => SetValue(widget, (T)value);
	}


	/// <summary>
	/// Registry for managing all attached properties in the Myra framework.
	/// </summary>
	public static class AttachedPropertiesRegistry
	{
		private static readonly Dictionary<int, BaseAttachedPropertyInfo> _properties = new Dictionary<int, BaseAttachedPropertyInfo>();
		private static readonly Dictionary<Type, BaseAttachedPropertyInfo[]> _propertiesByType = new Dictionary<Type, BaseAttachedPropertyInfo[]>();

		/// <summary>
		/// Creates a new attached property and registers it.
		/// </summary>
		/// <typeparam name="T">The type of the property value.</typeparam>
		/// <param name="type">The type that owns the property.</param>
		/// <param name="name">The name of the property.</param>
		/// <param name="defaultValue">The default value for the property.</param>
		/// <param name="option">The option specifying how the property affects layout.</param>
		/// <param name="attributes">Optional attributes for the property.</param>
		/// <returns>The created attached property metadata.</returns>
		public static AttachedPropertyInfo<T> Create<T>(Type type, string name, T defaultValue, AttachedPropertyOption option, Attribute[] attributes = null)
		{
			var result = new AttachedPropertyInfo<T>(_properties.Count, name, type, defaultValue, option, attributes);
			_properties[result.Id] = result;

			return result;
		}

		/// <summary>
		/// Gets all attached properties defined on the specified type and its base types.
		/// </summary>
		/// <param name="type">The type to get properties for.</param>
		/// <returns>An array of all attached properties for the type.</returns>
		public static BaseAttachedPropertyInfo[] GetPropertiesOfType(Type type)
		{
			BaseAttachedPropertyInfo[] result;
			if (_propertiesByType.TryGetValue(type, out result))
			{
				return result;
			}

			var propertiesList = new List<BaseAttachedPropertyInfo>();

			// Build list of all attached properties
			var currentType = type;
			while (currentType != null && currentType != typeof(object))
			{
				// Make sure all static fields of type are initialized
				RuntimeHelpers.RunClassConstructor(currentType.TypeHandle);
				foreach (var pair in _properties)
				{
					if (pair.Value.OwnerType == currentType)
					{
						propertiesList.Add(pair.Value);
					}
				}

				currentType = currentType.BaseType;
			}

			result = propertiesList.ToArray();
			_propertiesByType[type] = result;

			return result;
		}
	}
}
