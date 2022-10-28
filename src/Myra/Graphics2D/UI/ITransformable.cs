#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Numerics;
#endif

namespace Myra.Graphics2D.UI
{
	internal interface ITransformable
	{
		Vector2 ToLocal(Vector2 source);
		Vector2 ToGlobal(Vector2 pos);
	}
}
