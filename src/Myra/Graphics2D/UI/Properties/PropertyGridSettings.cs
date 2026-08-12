using System;
using System.ComponentModel;
using System.Xml.Serialization;
using AssetManagementBase;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// Settings for configuring a property grid's behavior and asset management.
	/// </summary>
	public class PropertyGridSettings
	{
		/// <summary>
		/// Gets or sets the asset manager used by the property grid.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public AssetManager AssetManager;

		/// <summary>
		/// Gets or sets the base path for relative asset paths.
		/// </summary>
		public string BasePath;

		/// <summary>
		/// Gets or sets the function used to retrieve image property values.
		/// </summary>
		public Func<string, string> ImagePropertyValueGetter;

		/// <summary>
		/// Gets or sets the function used to set image property values.
		/// </summary>
		public Action<string, string> ImagePropertyValueSetter;
	}
}
