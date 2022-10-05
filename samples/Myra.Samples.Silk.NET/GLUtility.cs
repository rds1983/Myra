using Silk.NET.OpenGL;
using System;

namespace Myra.Samples.AllWidgets
{
	internal static class GLUtility
	{
		public static void CheckError()
		{
			var error = (ErrorCode)Env.Gl.GetError();
			if (error != ErrorCode.NoError)
				throw new Exception("GL.GetError() returned " + error.ToString());
		}
	}
}
