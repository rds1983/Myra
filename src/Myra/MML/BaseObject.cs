using Myra.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.MML
{
	public class BaseObject: IItemWithId, INotifyAttachedPropertyChanged
	{
		private string _id = null;

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

		[XmlIgnore]
		[Browsable(false)]
		public readonly Dictionary<int, object> AttachedPropertiesValues = new Dictionary<int, object>();

		/// <summary>
		/// Holds custom user attributes not mapped to the object
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Dictionary<string, string> UserData { get; private set; } = new Dictionary<string, string>();

		/// <summary>
		/// External files used by this object
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Dictionary<string, string> Resources { get; private set; } = new Dictionary<string, string>();

		public event EventHandler IdChanged;

		protected internal virtual void OnIdChanged()
		{
			IdChanged.Invoke(this);
		}

		public virtual void OnAttachedPropertyChanged(BaseAttachedPropertyInfo propertyInfo)
		{
		}
	}
}