namespace Myra.Graphics2D.UI
{
	public class Dialog: Window
	{
		private readonly Grid _dialogContent;

		public Button ButtonOk { get; set; }
		public Button ButtonCancel { get; set; }

		public override Widget Content
		{
			get
			{
				if (_dialogContent.Widgets.Count >= 2)
				{
					return _dialogContent.Widgets[1];
				}

				return null;
			}

			set
			{
				if (value == Content)
				{
					return;
				}

				// Remove existing
				if (Content != null)
				{
					_dialogContent.Widgets.Remove(Content);
				}

				if (value != null)
				{
					value.GridPositionY = 0;
					_dialogContent.Widgets.Add(value);
				}
			}
		}

		public Dialog()
		{
			_dialogContent = new Grid {RowSpacing = 8};

			_dialogContent.RowsProportions.Add(new Proportion());
			_dialogContent.RowsProportions.Add(new Proportion());

			var buttonsGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Right,
				GridPositionY = 1
			};

			buttonsGrid.ColumnsProportions.Add(new Proportion());
			buttonsGrid.ColumnsProportions.Add(new Proportion());

			ButtonOk = new Button
			{
				Text = "Ok"
			};

			ButtonOk.Up += (sender, args) =>
			{
				ModalResult = (int)DefaultModalResult.Ok;
				Close();
			};

			buttonsGrid.Widgets.Add(ButtonOk);

			var cancelButton = new Button
			{
				Text = "Cancel",
				GridPositionX = 1
			};

			cancelButton.Up += (sender, args) =>
			{
				ModalResult = (int)DefaultModalResult.Cancel;
				Close();
			};

			buttonsGrid.Widgets.Add(cancelButton);

			_dialogContent.Widgets.Add(buttonsGrid);

			base.Content = _dialogContent;
		}

		public static Dialog CreateMessageBox(string title, Widget content)
		{
			var w = new Dialog
			{
				Title = title,
				Content = content
			};

			return w;
		}

		public static Dialog CreateMessageBox(string title, string message)
		{
			var messageLabel = new TextBlock
			{
				Text = message
			};

			return CreateMessageBox(title, messageLabel);
		}
	}
}
