using System;
using System.IO;
using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace Myra.Samples.AllWidgets
{
	public class Shader : IDisposable
	{
		private int _handle;

		public Shader(string vertexPath, string fragmentPath)
		{
			int vertex = LoadShader(ShaderType.VertexShader, vertexPath);
			int fragment = LoadShader(ShaderType.FragmentShader, fragmentPath);
			_handle = GL.CreateProgram();
			GLUtility.CheckError();

			GL.AttachShader(_handle, vertex);
			GLUtility.CheckError();

			GL.AttachShader(_handle, fragment);
			GLUtility.CheckError();

			GL.LinkProgram(_handle);
			GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out var status);
			if (status == 0)
			{
				throw new Exception($"Program failed to link with error: {GL.GetProgramInfoLog(_handle)}");
			}

			GL.DetachShader(_handle, vertex);
			GL.DetachShader(_handle, fragment);

			GL.DeleteShader(vertex);
			GL.DeleteShader(fragment);
		}

		public void Use()
		{
			GL.UseProgram(_handle);
			GLUtility.CheckError();
		}

		public void SetUniform(string name, int value)
		{
			int location = GL.GetUniformLocation(_handle, name);
			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}
			GL.Uniform1(location, value);
			GLUtility.CheckError();
		}

		public void SetUniform(string name, float value)
		{
			int location = GL.GetUniformLocation(_handle, name);
			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}
			GL.Uniform1(location, value);
			GLUtility.CheckError();
		}

		public unsafe void SetUniform(string name, Matrix4x4 value)
		{
			int location = GL.GetUniformLocation(_handle, name);
			if (location == -1)
			{
				throw new Exception($"{name} uniform not found on shader.");
			}

			GL.UniformMatrix4(location, 1, false, (float*)&value);
			GLUtility.CheckError();
		}

		public void Dispose()
		{
			GL.DeleteProgram(_handle);
		}

		private int LoadShader(ShaderType type, string path)
		{
			string src = File.ReadAllText(path);
			int handle = GL.CreateShader(type);
			GLUtility.CheckError();

			GL.ShaderSource(handle, src);
			GLUtility.CheckError();

			GL.CompileShader(handle);
			string infoLog = GL.GetShaderInfoLog(handle);
			if (!string.IsNullOrWhiteSpace(infoLog))
			{
				throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
			}

			return handle;
		}

		public int GetAttribLocation(string attribName)
		{
			var result = GL.GetAttribLocation(_handle, attribName);
			GLUtility.CheckError();
			return result;
		}
	}
}
