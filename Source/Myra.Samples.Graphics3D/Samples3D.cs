using System;

namespace Myra.Samples.Graphics3D
{
	public class Samples3D
	{
		public static readonly Type[] AllSampleTypes;

		static Samples3D()
		{
			AllSampleTypes = new[]
			{
				typeof (Primitives3DSample),
				typeof (TerrainSample)
			};
		}
	}
}
