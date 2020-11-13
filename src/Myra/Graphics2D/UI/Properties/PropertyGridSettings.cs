using System.ComponentModel;
using System.Xml.Serialization;
using XNAssets;

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
