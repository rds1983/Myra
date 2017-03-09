using System.Windows.Forms;

namespace Myra.Samples.WinForms
{
	public static class Utility
	{
		public static string SaveFileHandler()
		{
			using (var dlg = new SaveFileDialog())
			{
				dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
				return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : string.Empty;
			}
		}

		public static string OpenFileHandler()
		{
			using (var dlg = new OpenFileDialog())
			{
				dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
				return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : string.Empty;
			}
		}
	}
}