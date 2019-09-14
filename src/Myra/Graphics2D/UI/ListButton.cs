using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	internal class ListButton: ImageTextButton
	{
		private readonly ISelector _selector;

		public ListButton(ImageTextButtonStyle bs, ISelector selector) : base(bs)
		{
			_selector = selector;
			Toggleable = true;
		}

		protected override bool CanChangeToggleable(bool value)
		{
			// At least one list button should be pressed
			return _selector.SelectionMode == SelectionMode.Multiple ||
				!IsPressed;
		}

		public override void InternalRender(RenderContext batch)
		{
			var w = IsMouseOver;
			if (w)
			{
				var b = GetCurrentBackground();
			}
			base.InternalRender(batch);
		}
	}
}
