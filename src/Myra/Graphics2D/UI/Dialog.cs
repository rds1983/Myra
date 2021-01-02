using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#endif

namespace Myra.Graphics2D.UI
{
	[StyleTypeName("Window")]
	public class Dialog : Window
	{
		[Browsable(false)]
		[XmlIgnore]
		public ImageTextButton ButtonOk { get; private set; }

		[Browsable(false)]
		[XmlIgnore]
		public ImageTextButton ButtonCancel { get; private set; }

		[Category("Behavior")]
		[DefaultValue(Keys.Enter)]
		public Keys ConfirmKey { get; set; }

		public Dialog(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			ConfirmKey = Keys.Enter;

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

			if (k == CloseKey)
			{
				CloseButton.DoClick();
			} else if (k == ConfirmKey)
			{
				ButtonOk.DoClick();
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