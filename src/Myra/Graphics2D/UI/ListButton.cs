using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	internal class ListButton: Button
	{
		protected override bool CanChangePressedOnTouchUp
		{
			get
			{
				// At least one list button should be pressed
				return !IsPressed;
			}
		}

		public ListButton(ButtonStyle bs) : base(bs)
		{
			Toggleable = true;
		}
	}
}
