using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class CheckBox: Button
	{
		internal class CheckBoxMetadata
		{
			[HiddenInEditor]
			[JsonIgnore]
			public bool Toggleable { get; set; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Behavior")]
		public override bool Toggleable
		{
			get { return base.Toggleable; }
			set { base.Toggleable = value; }
		}

		public CheckBox(ButtonStyle bs) : base(bs)
		{
			Toggleable = true;
		}

		public CheckBox() : this(DefaultAssets.UIStylesheet.CheckBoxStyle)
		{
		}
	}
}
