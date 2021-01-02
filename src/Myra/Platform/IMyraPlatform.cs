using FontStashSharp.Interfaces;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Platform
{
	public interface IMyraPlatform: ITexture2DManager
	{
		Point ViewSize { get; }

		IMyraRenderer CreateRenderer();

		MouseInfo GetMouseInfo();

		void SetKeysDown(bool[] keys);
	}
}
