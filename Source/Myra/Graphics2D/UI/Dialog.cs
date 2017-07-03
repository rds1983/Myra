namespace Myra.Graphics2D.UI
{
	public class Dialog: Window
	{
		public Button ButtonOk { get; set; }
		public Button ButtonCancel { get; set; }

		public static Dialog CreateMessageBox(string title, Widget content)
		{
			var w = new Dialog
			{
				Title = title
			};

			var contentGrid = new Grid();

			contentGrid.RowsProportions.Add(new Proportion());
			contentGrid.RowsProportions.Add(new Proportion());

			contentGrid.Widgets.Add(content);

			var buttonsGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Right,
				GridPositionY = 1
			};

			buttonsGrid.ColumnsProportions.Add(new Proportion());
			buttonsGrid.ColumnsProportions.Add(new Proportion());

			w.ButtonOk = new Button
			{
				Text = "Ok"
			};

			w.ButtonOk.Up += (sender, args) =>
			{
				w.ModalResult = (int)DefaultModalResult.Ok;
				w.Close();
			};

			buttonsGrid.Widgets.Add(w.ButtonOk);

			var cancelButton = new Button
			{
				Text = "Cancel",
				GridPositionX = 1
			};

			cancelButton.Up += (sender, args) =>
			{
				w.ModalResult = (int)DefaultModalResult.Cancel;
				w.Close();
			};

			buttonsGrid.Widgets.Add(cancelButton);

			contentGrid.Widgets.Add(buttonsGrid);

			w.Content = contentGrid;

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
