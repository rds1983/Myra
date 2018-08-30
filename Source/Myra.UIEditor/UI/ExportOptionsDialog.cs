using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Myra.Editor.UI.File;
using Myra.Graphics2D.UI;

namespace Myra.UIEditor.UI
{
	public class ExportOptionsDialog : Dialog
	{
		private readonly ExportOptionsWidget _exportOptionsWidget = new ExportOptionsWidget();


		public ExportOptionsDialog()
		{
			Title = "Export";

			Content = _exportOptionsWidget;

			_exportOptionsWidget._textNamespace.Text = Studio.Instance.Project.ExportOptions.Namespace;
			_exportOptionsWidget._textClassName.Text = Studio.Instance.Project.ExportOptions.Class;
			_exportOptionsWidget._textOutputPath.Text = Studio.Instance.Project.ExportOptions.OutputPath;

			_exportOptionsWidget._buttonChangeOutputPath.Down += ButtonChangeOutputPathOnDown;

			ButtonOk.Down += ButtonOkOnDown;
		}

		private void ButtonChangeOutputPathOnDown(object sender, EventArgs eventArgs)
		{
			var dlg = new Editor.UI.File.FileDialog(FileDialogMode.ChooseFolder);
			dlg.Folder = _exportOptionsWidget._textOutputPath.Text;

			dlg.Closed += (s, a) =>
			{
				if (dlg.ModalResult != (int)Myra.Graphics2D.UI.Window.DefaultModalResult.Ok)
				{
					return;
				}

				_exportOptionsWidget._textOutputPath.Text = dlg.Folder;
			};

			dlg.ShowModal(Desktop);
		}

		private void ButtonOkOnDown(object sender, EventArgs eventArgs)
		{
			try
			{
				Studio.Instance.Project.ExportOptions.Namespace = _exportOptionsWidget._textNamespace.Text;
				Studio.Instance.Project.ExportOptions.OutputPath = _exportOptionsWidget._textOutputPath.Text;
				Studio.Instance.Project.ExportOptions.Class = _exportOptionsWidget._textClassName.Text;
				Studio.Instance.IsDirty = true;

				var export = new ExporterCS(Studio.Instance.Project);

				var strings = new List<string>();
				strings.Add("Success. Following files had been written:");
				strings.AddRange(export.Export());

				var dlg = CreateMessageBox("Export To C#", string.Join("\n", strings));
				dlg.ShowModal(Desktop);
			}
			catch (Exception ex)
			{
				var msg = CreateMessageBox("Error", ex.Message);
				msg.ShowModal(Desktop);
			}
		}
	}
}