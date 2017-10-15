using Myra.Attributes;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class MenuSeparator : IMenuItem
	{
		public string Id { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Menu Menu { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Widget Widget { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public char? UnderscoreChar { get { return null; } }
	}
}
