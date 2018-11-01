using Myra.Attributes;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	public class MenuSeparator : IMenuItem
	{
		[DefaultValue(null)]
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
