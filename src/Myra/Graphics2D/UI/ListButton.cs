using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	internal class ListButton: ImageTextButton
	{
		private readonly ISelector _selector;

		protected override bool CanChangePressedOnTouchUp
		{
			get
			{
				// At least one list button should be pressed
				return _selector.SelectionMode == SelectionMode.Multiple || !IsPressed;
			}
		}

		public ListButton(ImageTextButtonStyle bs, ISelector selector) : base(bs)
		{
			_selector = selector;
			Toggleable = true;
		}
	}
}
