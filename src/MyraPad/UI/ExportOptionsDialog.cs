using System;
using Myra.Graphics2D.UI.File;

namespace MyraPad.UI
{
	public partial class ExportOptionsDialog
	{
		public ExportOptionsDialog()
		{
			BuildUI();

			_textNamespace.Text = Studio.Instance.Project.ExportOptions.Namespace;
			_textClassName.Text = Studio.Instance.Project.ExportOptions.Class;
			_textOutputPath.Text = Studio.Instance.Project.ExportOptions.OutputPath;

			_buttonChangeOutputPath.Click += ButtonChangeOutputPathOnClick;
		}

		private void ButtonChangeOutputPathOnClick(object sender, EventArgs eventArgs)
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