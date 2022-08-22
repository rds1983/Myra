using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Assets;

namespace Myra.Graphics2D.UI.Properties
{
	public class PropertyGridSettings
	{
		[Browsable(false)]
		[XmlIgnore]
		public IAssetManager AssetManager;

		public string BasePath;
	}
}
