using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class CheckBox : ImageTextButton
	{
		[HiddenInEditor]
		[XmlIgnore]
		[EditCategory("Behavior")]
		[DefaultValue(true)]
		public override bool Toggleable
		{
			get { return base.Toggleable; }
			set { base.Toggleable = value; }
		}

		public CheckBox(ImageTextButtonStyle bs) : base(bs)
		{
			Toggleable = true;
		}

		public CheckBox(Stylesheet stylesheet, string style) : this(stylesheet.CheckBoxStyles[style])
		{
		}

		public CheckBox(Stylesheet stylesheet) : this(stylesheet.CheckBoxStyle)
		{
		}

		public CheckBox(string style) : this(Stylesheet.Current, style)
		{
		}

		public CheckBox() : this(Stylesheet.Current)
		{
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(stylesheet.CheckBoxStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.CheckBoxStyles.Keys.ToArray();
		}
	}
}
