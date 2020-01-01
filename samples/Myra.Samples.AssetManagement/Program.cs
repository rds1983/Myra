using System;
using System.Windows.Forms;

namespace Myra.Samples.AssetManagement
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				using (var game = new AssetManagementGame())
					game.Run();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
