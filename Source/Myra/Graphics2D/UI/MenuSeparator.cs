using Myra.Attributes;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class MenuSeparator: IMenuItem
	{
		[HiddenInEditor]
		[JsonIgnore]
		public Menu Menu { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Widget Widget { get; set; }
	}
}
