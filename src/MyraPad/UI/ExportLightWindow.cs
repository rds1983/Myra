using Myra;
using System;
using TextCopy;

namespace MyraPad.UI
{
	public partial class ExportLightWindow
	{
		public string Code
		{
			get => _textCode.Text;

			set => _textCode.Text = value;
		}

		public ExportLightWindow()
		{
			BuildUI();

			_buttonCopyToClipboard.Click += _buttonCopyToClipboard_Click;
		}

		private void _buttonCopyToClipboard_Click(object sender, System.EventArgs e)
		{
			try
			{
				Clipboard.SetText(Code);
			}
			catch (Exception)
			{
				MyraEnvironment.InternalClipboard = Code;
			}
		}
	}
}