using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class Dialog : Window
	{
		[HiddenInEditor]
		[JsonIgnore]
		public Button ButtonOk { get; private set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Button ButtonCancel { get; private set; }

		public Dialog()
		{
			RowsProportions.Add(new Proportion());

			var buttonsGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Right,
				GridPositionY = 2
			};

			buttonsGrid.ColumnsProportions.Add(new Proportion());
			buttonsGrid.ColumnsProportions.Add(new Proportion());

			ButtonOk = new Button
			{
				Text = "Ok"
			};

			ButtonOk.Up += (sender, args) =>
			{
				if (!CanCloseByOk())
				{
					return;
				}

				Result = true;
				ModalResult = (int)DefaultModalResult.Ok;
				Close();
			};

			buttonsGrid.Widgets.Add(ButtonOk);

			ButtonCancel = new Button
			{
				Text = "Cancel",
				GridPositionX = 1
			};

			ButtonCancel.Up += (sender, args) =>
			{
				Result = false;
				ModalResult = (int)DefaultModalResult.Cancel;
				Close();
			};

			buttonsGrid.Widgets.Add(ButtonCancel);

			Widgets.Add(buttonsGrid);
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch(k)
			{
				case Keys.Escape:
					ButtonCancel.Press();
					break;
				case Keys.Enter:
					ButtonOk.Press();
					break;
			}
		}

		protected virtual bool CanCloseByOk()
		{
			return true;
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
