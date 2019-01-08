using System.ComponentModel;
using System.Linq;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class RadioButton : Button
	{
		[HiddenInEditor]
		[JsonIgnore]
		[EditCategory("Behavior")]
		[DefaultValue(true)]
		public override bool Toggleable
		{
			get
			{
				return base.Toggleable;
			}
			set
			{
				base.Toggleable = value;
			}
		}

		public RadioButton(ButtonStyle bs) : base(bs)
		{
			Toggleable = true;
		}

		public RadioButton(string style) : this(Stylesheet.Current.RadioButtonStyles[style])
		{
		}

		public RadioButton() : this(Stylesheet.Current.RadioButtonStyle)
		{
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (IsPressed)
			{
				foreach (var child in Parent.ChildrenCopy)
				{
					var asRadio = child as RadioButton;

					if (asRadio == null || asRadio == this)
					{
						continue;
					}

					asRadio.IsPressed = false;
				}
			}
		}

		protected override void SetStyleByName(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.RadioButtonStyles[name]);
		}

		internal override string[] GetStyleNames(Stylesheet stylesheet)
		{
			return stylesheet.RadioButtonStyles.Keys.ToArray();
		}
	}
}