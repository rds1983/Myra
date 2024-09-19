 using Myra.Graphics2D.UI.Styles;
using System;

namespace Myra.Graphics2D.UI
{
	[Obsolete]
	internal class ListButton: ImageTextButton
	{
		private readonly ISelector _selector;

		public override bool IsPressed
		{
			get => base.IsPressed;

			set
			{
				if (IsPressed && _selector.SelectionMode == SelectionMode.Single && Parent != null)
				{
					// If this is last selected item
					// Don't allow it to be unselected
					var allow = false;
					foreach (var child in Parent.ChildrenCopy)
					{
						var asListButton = child as ListButton;
						if (asListButton == null || asListButton == this)
						{
							continue;
						}

						if (asListButton.IsPressed)
						{
							allow = true;
							break;
						}
					}

					if (!allow)
					{
						return;
					}
				}

				base.IsPressed = value;
			}
		}

		public ListButton(ImageTextButtonStyle bs, ISelector selector) : base(null)
		{
			_selector = selector;
			Toggleable = true;

			ApplyImageTextButtonStyle(bs);
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (!IsPressed)
			{
				return;
			}

			// Release other pressed radio buttons
			foreach (var child in Parent.ChildrenCopy)
			{
				var asListButton = child as ListButton;
				if (asListButton == null || asListButton == this)
				{
					continue;
				}

				asListButton.IsPressed = false;
			}
		}
	}
}
