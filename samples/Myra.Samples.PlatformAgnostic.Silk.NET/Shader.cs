using System;
using System.IO;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Myra.Samples.AllWidgets
{
	public class Shader : IDisposable
	{
		private uint _handle;

		public Shader(string vertexPath, string fragmentPath)
		{
			uint vertex = LoadShader(ShaderType.VertexShader, vertexPath);
			uint fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
			_handle = Env.Gl.CreateProgram();
			GLUtility.CheckError();

			Env.Gl.AttachShader(_handle, vertex);
			GLUtility.CheckError();

			Env.Gl.AttachShader(_handle, fragment);
			GLUtility.CheckError();

			Env.Gl.LinkProgram(_handle);
			Env.Gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
			if (status == 0)
			{
				throw new Exception($"Program failed to link with error: {Env.Gl.GetProgramInfoLog(_handle)}");
			}

			Env.Gl.DetachShader(_handle, vertex);
			Env.Gl.DetachShader(_handle, fragment);

			Env.Gl.DeleteShader(vertex);
			Env.Gl.DeleteShader(fragment);
		}

		public void Use()
		{
			Env.Gl.UseProgram(_handle);
			GLUtility.CheckError();
		}

		public void SetUniform(string name, int value)
		{
			int location = Env.Gl.GetUniformLocation(_handle, name);
			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}
			Env.Gl.Uniform1(location, value);
			GLUtility.CheckError();
		}

		public void SetUniform(string name, float value)
		{
			int location = Env.Gl.GetUniformLocation(_handle, name);
			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}
			Env.Gl.Uniform1(location, value);
			GLUtility.CheckError();
		}

		public unsafe void SetUniform(string name, Matrix4x4 value)
		{
			int location = Env.Gl.GetUniformLocation(_handle, name);
			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			Env.Gl.UniformMatrix4(location, 1, false, (float*)&value);
			GLUtility.CheckError();
		}

		public void Dispose()
		{
			Env.Gl.DeleteProgram(_handle);
		}

		private uint LoadShader(ShaderType type, string path)
		{
			string src = File.ReadAllText(path);
			uint handle = Env.Gl.CreateShader(type);
			GLUtility.CheckError();

			Env.Gl.ShaderSource(handle, src);
			GLUtility.CheckError();

			Env.Gl.CompileShader(handle);
			string infoLog = Env.Gl.GetShaderInfoLog(handle);
			if (!string.IsNullOrWhiteSpace(infoLog))
			{
				throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
			}

			return handle;
		}

		public int GetAttribLocation(string attribName)
		{
			var result = Env.Gl.GetAttribLocation(_handle, attribName);
			GLUtility.CheckError();
			return result;
		}
	}
}