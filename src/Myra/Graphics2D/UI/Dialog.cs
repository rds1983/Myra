using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using static Myra.Graphics2D.UI.Grid;
using System.Xml.Serialization;

#if !XENKO
using Microsoft.Xna.Framework.Input;
#else
using Xenko.Input;
#endif

namespace Myra.Graphics2D.UI
{
	public class Dialog : Window
	{
		[HiddenInEditor]
		[XmlIgnore]
		public Button ButtonOk { get; private set; }

		[HiddenInEditor]
		[XmlIgnore]
		public Button ButtonCancel { get; private set; }

		public Dialog(DialogStyle style) : base(style)
		{
			InternalChild.RowsProportions.Add(new Proportion());

			var buttonsGrid = new Grid()
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Right,
				GridRow = 2
			};

			buttonsGrid.ColumnsProportions.Add(new Proportion());
			buttonsGrid.ColumnsProportions.Add(new Proportion());

			ButtonOk = new Button
			{
				Text = "Ok"
			};

			ButtonOk.Click += (sender, args) =>
			{
				if (!CanCloseByOk())
				{
					return;
				}

				Result = true;
				Close();
			};

			buttonsGrid.Widgets.Add(ButtonOk);

			ButtonCancel = new Button
			{
				Text = "Cancel",
				GridColumn = 1
			};

			ButtonCancel.Click += (sender, args) =>
			{
				Result = false;
				Close();
			};

			buttonsGrid.Widgets.Add(ButtonCancel);

			InternalChild.Widgets.Add(buttonsGrid);
		}

		public Dialog(string style) : this(Stylesheet.Current.DialogStyles[style])
		{
		}

		public Dialog() : this(Stylesheet.Current.DialogStyle)
		{
		}

		public override void OnKeyDown(Keys k)
		{
			FireKeyDown(k);

			switch (k)
			{
				case Keys.Escape:
					ButtonCancel.DoClick();
					break;
				case Keys.Enter:
					ButtonOk.DoClick();
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
				Text = message,
				Wrap = true
			};

			return CreateMessageBox(title, messageLabel);
		}
	}
}