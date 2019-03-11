namespace MyraPad
{
	public class Options
	{
		public bool AutoIndent;
		public int IndentSpacesSize;
		public bool AutoClose;

		public Options()
		{
			AutoIndent = true;
			IndentSpacesSize = 2;
			AutoClose = true;
		}
	}
}
