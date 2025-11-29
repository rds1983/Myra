/**
 * Copyright 2011-2013 See AUTHORS file.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XFramework = Microsoft.Xna.Framework;
using XGraphics = Microsoft.Xna.Framework.Graphics;

namespace GdxSkinImport.MonoGdx;

public static class XnaExt
{
	public static bool IsMapMap(this TextureFilter filter)
	{
		switch (filter)
		{
			case TextureFilter.LinearMipPoint:
			case TextureFilter.MinLinearMagPointMipLinear:
			case TextureFilter.MinLinearMagPointMipPoint:
			case TextureFilter.MinPointMagLinearMipLinear:
			case TextureFilter.MinPointMagLinearMipPoint:
			case TextureFilter.PointMipLinear:
				return true;
			default:
				return false;
		}
	}

	public static bool IsButtonPressed(this MouseState state, int button)
	{
		switch (button)
		{
			case 0: return state.LeftButton == ButtonState.Pressed;
			case 2: return state.MiddleButton == ButtonState.Pressed;
			case 1: return state.RightButton == ButtonState.Pressed;
			case 3: return state.XButton1 == ButtonState.Pressed;
			case 4: return state.XButton2 == ButtonState.Pressed;
			default: return false;
		}
	}

	public static Color MultiplyAlpha(this Color color, float alphaFactor)
	{
		return new Color(color.R, color.G, color.B, (byte)(color.A * alphaFactor));
	}

	public static Color Multiply(this Color color1, Color color2)
	{
		Vector4 v1 = color1.ToVector4();
		Vector4 v2 = color2.ToVector4();

		return new Color(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
	}

	public static class Matrix
	{
		public static XFramework.Matrix CreateOrthographic2D(float x, float y, float width, float height)
		{
			return CreateOrthographic(x, x + width, y, y + height, 0, 1);
		}

		public static XFramework.Matrix CreateOrthographic2D(float x, float y, float width, float height, float near, float far)
		{
			return CreateOrthographic(x, x + width, y, y + height, near, far);
		}

		public static XFramework.Matrix CreateOrthographic(float left, float right, float bottom, float top, float near, float far)
		{
			float xOrth = 2 / (right - left);
			float yOrth = 2 / (top - bottom);
			float zOrth = 1 / (near - far);

			float tx = (left + right) / (left - right);
			float ty = (top + bottom) / (bottom - top);
			float tz = (near) / (near - far);

			return new XFramework.Matrix(
				xOrth, 0, 0, 0,
				0, yOrth, 0, 0,
				0, 0, zOrth, 0,
				tx, ty, tz, 1
			);
		}

		public static void Translate(ref XFramework.Matrix matrix, Vector3 amount)
		{
			matrix.M41 += amount.X;
			matrix.M42 += amount.Y;
			matrix.M43 += amount.Z;
		}
	}

	public static class Texture2D
	{
		public static XGraphics.Texture2D FromFile(GraphicsDevice device, string file)
		{
			return FromFile(device, file, true);
		}

		public static XGraphics.Texture2D FromFile(GraphicsDevice device, string file, bool premultiplyAlpha)
		{
			XGraphics.Texture2D tex = null;

			using (FileStream fs = File.OpenRead(file))
			{
				tex = XGraphics.Texture2D.FromStream(device, fs);
			}

			if (premultiplyAlpha)
			{
				byte[] data = new byte[tex.Width * tex.Height * 4];
				tex.GetData(data);

				for (int i = 0; i < data.Length; i += 4)
				{
					int a = data[i + 3];
					data[i + 0] = (byte)(data[i + 0] * a / 255);
					data[i + 1] = (byte)(data[i + 1] * a / 255);
					data[i + 2] = (byte)(data[i + 2] * a / 255);
				}

				tex.SetData(data);
			}

			return tex;
		}
	}
}
