using System;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Abstract base class representing a record for property editing in the property grid.
	/// </summary>
	public abstract class Record
	{
		/// <summary>
		/// Gets or sets a value indicating whether this record has a setter method or property.
		/// </summary>
		public bool HasSetter { get; set; }

		/// <summary>
		/// Gets the name of the record.
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// Gets the type of the value represented by this record.
		/// </summary>
		public abstract Type Type { get; }
		/// <summary>
		/// Gets or sets the category that this record belongs to.
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// Gets the value of this record from the specified object.
		/// </summary>
		/// <param name="obj">The object to get the value from.</param>
		/// <returns>The value of the record.</returns>
		public abstract object GetValue(object obj);
		/// <summary>
		/// Sets the value of this record on the specified object.
		/// </summary>
		/// <param name="obj">The object to set the value on.</param>
		/// <param name="value">The new value to set.</param>
		public abstract void SetValue(object obj, object value);

		/// <summary>
		/// Finds an attribute of the specified type on this record.
		/// </summary>
		/// <typeparam name="T">The type of attribute to find.</typeparam>
		/// <returns>The attribute of the specified type, or null if not found.</returns>
		public abstract T FindAttribute<T>() where T : Attribute;
	}
}
