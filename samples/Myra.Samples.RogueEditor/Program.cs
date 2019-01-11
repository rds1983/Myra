using System;
using System.Windows.Forms;

namespace Myra.Samples.RogueEditor
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				var state = State.Load();
				using (var game = new Studio(state))
				{
					game.Run();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Error");
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
