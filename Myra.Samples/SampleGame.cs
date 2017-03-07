using System;

namespace Myra.Samples
{
	public class SampleGame
	{
		public static readonly Type[] AllSampleTypes;

		static SampleGame()
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
				typeof (Notepad),
				typeof (Primitives3DSample),
				typeof (TerrainSample)
			};
		}
	}
}