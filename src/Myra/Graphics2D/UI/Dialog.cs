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

		public Dialog(DialogStyle style) : base(style)
		{
			var buttonsGrid = new HorizontalBox()
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
				if (!CanCloseByOk())
				{
					return;
				}

				Result = true;
				Close();
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

		public Dialog(Stylesheet stylesheet, string style) : this(stylesheet.DialogStyles[style])
		{
		}

		public Dialog(Stylesheet stylesheet) : this(stylesheet.DialogStyle)
		{
		}

		public Dialog(string style) : this(Stylesheet.Current, style)
		{
		}

		public Dialog() : this(Stylesheet.Current)
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