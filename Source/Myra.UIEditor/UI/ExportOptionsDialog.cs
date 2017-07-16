using System;
using System.Windows.Forms;
using Myra.Graphics2D.UI;
using NLog;

namespace Myra.UIEditor.UI
{
	public class ExportOptionsDialog : Dialog
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly ExportOptionsWidget _exportOptionsWidget = new ExportOptionsWidget();


		public ExportOptionsDialog()
		{
			Content = _exportOptionsWidget;

			_exportOptionsWidget._textNamespace.Text = Studio.Instance.Project.ExportOptions.Namespace;
			_exportOptionsWidget._textClassName.Text = Studio.Instance.Project.ExportOptions.Class;
			_exportOptionsWidget._textOutputPath.Text = Studio.Instance.Project.ExportOptions.OutputPath;

			_exportOptionsWidget._buttonChangeOutputPath.Down += ButtonChangeOutputPathOnDown;

			ButtonOk.Down += ButtonOkOnDown;
		}

		private void ButtonChangeOutputPathOnDown(object sender, EventArgs eventArgs)
		{
			using (var dlg = new FolderBrowserDialog())
			{
				dlg.SelectedPath = _exportOptionsWidget._textOutputPath.Text;
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					_exportOptionsWidget._textOutputPath.Text = dlg.SelectedPath;
				}
			}
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
				export.Export();
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
				var msg = CreateMessageBox("Error", ex.Message);
				msg.ShowModal(Desktop);
			}
		}
	}
}