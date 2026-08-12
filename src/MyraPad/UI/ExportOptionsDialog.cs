using System;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;

namespace MyraPad.UI
{
	public partial class ExportOptionsDialog
	{
		public ExportOptionsDialog()
		{
			BuildUI();

			_buttonChangeOutputPath.Click += ButtonChangeOutputPathOnClick;
		}

		public void SetOptions(ExportOptions options)
		{
			_textNamespace.Text = options.Namespace;
			_textClassName.Text = options.Class;
			_textOutputPath.Text = options.OutputPath;
			_comboFieldsVisibility.SelectedIndex = (int)options.FieldsVisibility;
		}

		public void GetOptions(ExportOptions options)
		{
			options.Namespace = _textNamespace.Text;
			options.Class = _textClassName.Text;
			options.OutputPath = _textOutputPath.Text;
			options.FieldsVisibility = (ExportOptionsFieldsVisibility)_comboFieldsVisibility.SelectedIndex;
		}

		private void ButtonChangeOutputPathOnClick(object sender, MyraEventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.ChooseFolder)
			{
				Folder = _textOutputPath.Text
			};

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				_textOutputPath.Text = dlg.Folder;
			};

			dlg.ShowModal(Desktop);
		}
	}
}