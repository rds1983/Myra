#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Platform
{
	public struct MouseInfo
	{
		public Point Position;
		public bool IsLeftButtonDown;
		public bool IsMiddleButtonDown;
		public bool IsRightButtonDown;
		public float Wheel;
	}
}
