using System;

namespace Myra.Samples
{
	public static class Samples2D
	{
		public static readonly Type[] AllSampleTypes;

		static Samples2D()
		{
			AllSampleTypes = new[]
			{
				typeof (HelloWorldSample),
				typeof (FormattedTextSample),
				typeof (TextBlocksSample),
				typeof (GridSample),
				typeof (CustomUIStylesheetSample),
				typeof (SplitPaneSample),
				typeof (ScrollPaneSample),
				typeof (LoadUISample),
				typeof (Notepad)
			};
		}
	}
}