namespace Myra.Graphics2D.UI
{
	internal class ListViewButton : ToggleButton
	{
		public Widget ButtonsContainer { get; set; }
		public Widget TopParent => ButtonsContainer ?? Parent;


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
					foreach (var child in TopParent.ChildrenCopy)
					{
						var asListViewButton = child as ListViewButton;
						if (asListViewButton == this)
						{
							continue;
						}

						if (asListViewButton == null)
						{
							asListViewButton = child.FindChild<ListViewButton>();
							if (asListViewButton == null || asListViewButton == this)
							{
								continue;
							}
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
			foreach (var child in TopParent.ChildrenCopy)
			{
				var asListViewButton = child as ListViewButton;
				if (asListViewButton == this)
				{
					continue;
				}

				if (asListViewButton == null)
				{
					asListViewButton = child.FindChild<ListViewButton>();
					if (asListViewButton == null || asListViewButton == this)
					{
						continue;
					}
				}

				asListViewButton.IsPressed = false;
			}
		}
	}
}
