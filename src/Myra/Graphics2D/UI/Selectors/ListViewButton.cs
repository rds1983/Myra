namespace Myra.Graphics2D.UI
{
	internal class ListViewButton : ToggleButton
	{
		public ListViewButton() : base(null)
		{
		}

		public override bool IsPressed
		{
			get => base.IsPressed;

			set
			{
				if (IsPressed && Parent != null)
				{
					// If this is last pressed button
					// Don't allow it to be unpressed
					var allow = false;
					foreach (var child in Parent.ChildrenCopy)
					{
						var asListViewButton = child as ListViewButton;
						if (asListViewButton == null || asListViewButton == this)
						{
							continue;
						}

						if (asListViewButton.IsPressed)
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

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (Parent == null || !IsPressed)
			{
				return;
			}

			// Release other pressed radio buttons
			foreach (var child in Parent.ChildrenCopy)
			{
				var asRadio = child as ListViewButton;
				if (asRadio == null || asRadio == this)
				{
					continue;
				}

				asRadio.IsPressed = false;
			}
		}
	}
}
