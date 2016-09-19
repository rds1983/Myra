using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	public class CheckBox: Button
	{
		public CheckBox(ButtonStyle bs) : base(bs)
		{
			Toggleable = true;
		}

		public CheckBox() : this(Stylesheet.Current.CheckBoxStyle)
		{
		}
	}
}
