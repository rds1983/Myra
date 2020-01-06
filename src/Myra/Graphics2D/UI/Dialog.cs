using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
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
		[Browsable(false)]
		[XmlIgnore]
		public ImageTextButton ButtonOk { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public ImageTextButton ButtonCancel { get; private set; }

		public Dialog(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			var buttonsGrid = new HorizontalStackPanel()
			{
				Spacing = 8,
				HorizontalAlignment = HorizontalAlignment.Right
			};

			ButtonOk = new ImageTextButton
			{
				Text = "Ok"
			};

			ButtonOk.Click += (sender, args) =>
			{
				OnOk();
			};

			buttonsGrid.Widgets.Add(ButtonOk);

			ButtonCancel = new ImageTextButton
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

		public override void OnKeyDown(Keys k)
		{
			FireKeyDown(k);

			switch (k)
			{
				case Keys.Escape:
					if (ButtonCancel.Visible)
					{
						ButtonCancel.DoClick();
					}
					break;
				case Keys.Enter:
					if (ButtonOk.Visible)
					{
						ButtonOk.DoClick();
					}
					break;
			}
		}

		protected internal virtual void OnOk()
		{
			if (!CanCloseByOk())
			{
				return;
			}

			Result = true;
			Close();
		}

		protected internal virtual bool CanCloseByOk()
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
			var messageLabel = new Label
			{
				Text = message,
				Wrap = true
			};

			return CreateMessageBox(title, messageLabel);
		}
	}
}