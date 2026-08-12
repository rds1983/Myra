using Myra.Events;
using Myra.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.MML
{
	/// <summary>
	/// Base class for all objects that support identifiers and attached properties.
	/// </summary>
	public class BaseObject: IItemWithId, INotifyAttachedPropertyChanged
	{
		private string _id = null;

		/// <summary>
		/// Gets or sets the unique identifier for this object.
		/// </summary>
		[DefaultValue(null)]
		public string Id
		{
			get
			{
				return _id;
			}

			set
			{
				if (value == _id)
				{
					return;
				}

				_id = value;
				OnIdChanged();
			}
		}

		/// <summary>
		/// Gets the dictionary storing values for all attached properties on this object.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public readonly Dictionary<int, object> AttachedPropertiesValues = new Dictionary<int, object>();

		/// <summary>
		/// Gets a dictionary of custom user attributes not mapped to the object.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Dictionary<string, string> UserData { get; private set; } = new Dictionary<string, string>();

		/// <summary>
		/// Occurs when the Id property changes.
		/// </summary>
		public event MyraEventHandler IdChanged;

		/// <summary>
		/// Called when the Id property changes.
		/// </summary>
		protected internal virtual void OnIdChanged()
		{
			IdChanged.Invoke(this, Graphics2D.UI.InputEventType.ValueChanged);
		}

		/// <summary>
		/// Called when an attached property changes on this object.
		/// </summary>
		/// <param name="propertyInfo">Information about the attached property that changed.</param>
		public virtual void OnAttachedPropertyChanged(BaseAttachedPropertyInfo propertyInfo)
		{
		}
	}
}