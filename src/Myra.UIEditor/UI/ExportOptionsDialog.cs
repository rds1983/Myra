using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;

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

			_exportOptionsWidget._buttonChangeOutputPath.Click += ButtonChangeOutputPathOnClick;
		}

		private void ButtonChangeOutputPathOnClick(object sender, EventArgs eventArgs)
		{
			var dlg = new FileDialog(FileDialogMode.ChooseFolder)
			{
				Folder = _exportOptionsWidget._textOutputPath.Text
			};

			dlg.Closed += (s, a) =>
			{
				if (!dlg.Result)
				{
					return;
				}

				_exportOptionsWidget._textOutputPath.Text = dlg.Folder;
			};

			dlg.ShowModal(Desktop);
		}

		public override void Close()
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

			base.Close();
		}
	}
}