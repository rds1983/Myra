// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// The following code is a port of DirectXTk http://directxtk.codeplex.com
// -----------------------------------------------------------------------------
// Microsoft Public License (Ms-PL)
//
// This license governs use of the accompanying software. If you use the 
// software, you accept this license. If you do not accept the license, do not
// use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and 
// "distribution" have the same meaning here as under U.S. copyright law.
// A "contribution" is the original software, or any additions or changes to 
// the software.
// A "contributor" is any person that distributes its contribution under this 
// license.
// "Licensed patents" are a contributor's patent claims that read directly on 
// its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the 
// license conditions and limitations in section 3, each contributor grants 
// you a non-exclusive, worldwide, royalty-free copyright license to reproduce
// its contribution, prepare derivative works of its contribution, and 
// distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license
// conditions and limitations in section 3, each contributor grants you a 
// non-exclusive, worldwide, royalty-free license under its licensed patents to
// make, have made, use, sell, offer for sale, import, and/or otherwise dispose
// of its contribution in the software or derivative works of the contribution 
// in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any 
// contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that 
// you claim are infringed by the software, your patent license from such 
// contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all 
// copyright, patent, trademark, and attribution notices that are present in the
// software.
// (D) If you distribute any portion of the software in source code form, you 
// may do so only under this license by including a complete copy of this 
// license with your distribution. If you distribute any portion of the software
// in compiled or object code form, you may only do so under a license that 
// complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The
// contributors give no express warranties, guarantees or conditions. You may
// have additional consumer rights under your local laws which this license 
// cannot change. To the extent permitted under your local laws, the 
// contributors exclude the implied warranties of merchantability, fitness for a
// particular purpose and non-infringement.

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Myra.Graphics3D.Modeling
{
	public static class PrimitivesFactory
	{
		private static Model CreateMesh<T>(GraphicsDevice device, T[] vertices, int[] indices,
			PrimitiveType primitiveType = PrimitiveType.TriangleList) where T : struct, IVertexType
		{
			var result = new Model();
			result.Init(device, vertices, indices, primitiveType);

			return result;
		}

		#region Cube

		public static Model CreateCube(GraphicsDevice device, float size = 1.0f)
		{
			var faceNormals = new[]
			{
				new Vector3(0, 0, 1),
				new Vector3(0, 0, -1),
				new Vector3(1, 0, 0),
				new Vector3(-1, 0, 0),
				new Vector3(0, 1, 0),
				new Vector3(0, -1, 0),
			};

			var textureCoordinates = new[]
			{
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1),
				new Vector2(0, 0),
			};
			var vertices = new VertexPositionNormalTexture[faceNormals.Length*4];
			var indices = new int[faceNormals.Length*6];

			size /= 2.0f;

			var vertexCount = 0;
			var indexCount = 0;

			// Create each face in turn.
			for (var i = 0; i < faceNormals.Length; i++)
			{
				var normal = faceNormals[i];

				// Get two vectors perpendicular both to the face normal and to each other.
				var basis = (i >= 4) ? Vector3.UnitZ : Vector3.UnitY;

				Vector3 side1;
				Vector3.Cross(ref normal, ref basis, out side1);

				Vector3 side2;
				Vector3.Cross(ref normal, ref side1, out side2);

				// Six indices (two triangles) per face.
				var vbase = i*4;
				indices[indexCount++] = (vbase + 0);
				indices[indexCount++] = (vbase + 1);
				indices[indexCount++] = (vbase + 2);

				indices[indexCount++] = (vbase + 0);
				indices[indexCount++] = (vbase + 2);
				indices[indexCount++] = (vbase + 3);

				// Four vertices per face.
				vertices[vertexCount++] = new VertexPositionNormalTexture((normal - side1 - side2)*size, normal,
					textureCoordinates[0]);
				vertices[vertexCount++] = new VertexPositionNormalTexture((normal - side1 + side2)*size, normal,
					textureCoordinates[1]);
				vertices[vertexCount++] = new VertexPositionNormalTexture((normal + side1 + side2)*size, normal,
					textureCoordinates[2]);
				vertices[vertexCount++] = new VertexPositionNormalTexture((normal + side1 - side2)*size, normal,
					textureCoordinates[3]);
			}

			return CreateMesh(device, vertices, indices);
		}

		#endregion

		#region Cylinder

		public static Model CreateCylinder(GraphicsDevice device, float height = 1.0f, float diameter = 1.0f,
			int tessellation = 32)
		{
			if (tessellation < 3)
				throw new ArgumentOutOfRangeException("tessellation", "tessellation must be >= 3");

			var vertices = new List<VertexPositionNormalTexture>();
			var indices = new List<int>();

			height /= 2;

			var topOffset = Vector3.UnitY*height;

			var radius = diameter/2;
			var stride = tessellation + 1;

			// Create a ring of triangles around the outside of the cylinder.
			for (var i = 0; i <= tessellation; i++)
			{
				var normal = GetCircleVector(i, tessellation);

				var sideOffset = normal*radius;

				var textureCoordinate = new Vector2((float) i/tessellation, 0);

				vertices.Add(new VertexPositionNormalTexture(sideOffset + topOffset, normal, textureCoordinate));
				vertices.Add(new VertexPositionNormalTexture(sideOffset - topOffset, normal, textureCoordinate + Vector2.UnitY));

				indices.Add(i*2);
				indices.Add((i*2 + 2)%(stride*2));
				indices.Add(i*2 + 1);

				indices.Add(i*2 + 1);
				indices.Add((i*2 + 2)%(stride*2));
				indices.Add((i*2 + 3)%(stride*2));
			}

			CreateCylinderCap(vertices, indices, tessellation, height, radius, true);
			CreateCylinderCap(vertices, indices, tessellation, height, radius, false);

			// Create flat triangle fan caps to seal the top and bottom.
			return CreateMesh(device, vertices.ToArray(), indices.ToArray());
		}

		// Helper computes a point on a unit circle, aligned to the x/z plane and centered on the origin.
		private static Vector3 GetCircleVector(int i, int tessellation)
		{
			var angle = (float) (i*2.0*Math.PI/tessellation);
			var dx = (float) Math.Sin(angle);
			var dz = (float) Math.Cos(angle);

			return new Vector3(dx, 0, dz);
		}

		private static void Swap<T>(ref T v1, ref T v2)
		{
			var tmp = v1;
			v1 = v2;
			v2 = tmp;
		}

		// Helper creates a triangle fan to close the end of a cylinder.
		private static void CreateCylinderCap(List<VertexPositionNormalTexture> vertices, List<int> indices, int tessellation,
			float height, float radius, bool isTop)
		{
			// Create cap indices.
			for (var i = 0; i < tessellation - 2; i++)
			{
				var i1 = (i + 1)%tessellation;
				var i2 = (i + 2)%tessellation;

				if (isTop)
				{
					Swap(ref i1, ref i2);
				}

				var vbase = vertices.Count;
				indices.Add(vbase);
				indices.Add(vbase + i1);
				indices.Add(vbase + i2);
			}

			// Which end of the cylinder is this?
			var normal = Vector3.UnitY;
			var textureScale = new Vector2(-0.5f);

			if (!isTop)
			{
				normal = -normal;
				textureScale.X = -textureScale.X;
			}

			// Create cap vertices.
			for (var i = 0; i < tessellation; i++)
			{
				var circleVector = GetCircleVector(i, tessellation);
				var position = (circleVector*radius) + (normal*height);
				var textureCoordinate = new Vector2(circleVector.X*textureScale.X + 0.5f, circleVector.Z*textureScale.Y + 0.5f);

				vertices.Add(new VertexPositionNormalTexture(position, normal, textureCoordinate));
			}
		}

		#endregion

		#region Sphere

		public static Model CreateSphere(GraphicsDevice device, float diameter = 1.0f, int tessellation = 16)
		{
			if (tessellation < 3) throw new ArgumentOutOfRangeException("tessellation", "Must be >= 3");

			var verticalSegments = tessellation;
			var horizontalSegments = tessellation*2;

			var vertices = new VertexPositionNormalTexture[(verticalSegments + 1)*(horizontalSegments + 1)];
			var indices = new int[(verticalSegments)*(horizontalSegments + 1)*6];

			var radius = diameter/2;

			var vertexCount = 0;
			// Create rings of vertices at progressively higher latitudes.
			for (var i = 0; i <= verticalSegments; i++)
			{
				var v = 1.0f - (float) i/verticalSegments;

				var latitude = (float) ((i*Math.PI/verticalSegments) - Math.PI/2.0);
				var dy = (float) Math.Sin(latitude);
				var dxz = (float) Math.Cos(latitude);

				// Create a single ring of vertices at this latitude.
				for (var j = 0; j <= horizontalSegments; j++)
				{
					var u = (float) j/horizontalSegments;

					var longitude = (float) (j*2.0*Math.PI/horizontalSegments);
					var dx = (float) Math.Sin(longitude);
					var dz = (float) Math.Cos(longitude);

					dx *= dxz;
					dz *= dxz;

					var normal = new Vector3(dx, dy, dz);
					var textureCoordinate = new Vector2(u, v);

					vertices[vertexCount++] = new VertexPositionNormalTexture(normal*radius, normal, textureCoordinate);
				}
			}

			// Fill the index buffer with triangles joining each pair of latitude rings.
			var stride = horizontalSegments + 1;

			var indexCount = 0;
			for (var i = 0; i < verticalSegments; i++)
			{
				for (var j = 0; j <= horizontalSegments; j++)
				{
					var nextI = i + 1;
					var nextJ = (j + 1)%stride;

					indices[indexCount++] = (i*stride + j);
					indices[indexCount++] = (nextI*stride + j);
					indices[indexCount++] = (i*stride + nextJ);

					indices[indexCount++] = (i*stride + nextJ);
					indices[indexCount++] = (nextI*stride + j);
					indices[indexCount++] = (nextI*stride + nextJ);
				}
			}

			return CreateMesh(device, vertices, indices);
		}

		#endregion

		#region Torus

		public static Model CreateTorus(GraphicsDevice device, float diameter = 1.0f, float thickness = 0.33333f,
			int tessellation = 32)
		{
			var vertices = new List<VertexPositionNormalTexture>();
			var indices = new List<int>();

			if (tessellation < 3)
				throw new ArgumentOutOfRangeException("tessellation", "tessellation parameter out of range");

			var stride = tessellation + 1;

			// First we loop around the main ring of the torus.
			for (var i = 0; i <= tessellation; i++)
			{
				var u = (float) i/tessellation;

				var outerAngle = i*MathHelper.TwoPi/tessellation - MathHelper.PiOver2;

				// Create a transform matrix that will align geometry to
				// slice perpendicularly though the current ring position.
				var transform = Matrix.CreateTranslation(diameter/2, 0, 0)*Matrix.CreateRotationY(outerAngle);

				// Now we loop along the other axis, around the side of the tube.
				for (int j = 0; j <= tessellation; j++)
				{
					float v = 1 - (float) j/tessellation;

					float innerAngle = j*MathHelper.TwoPi/tessellation + MathHelper.Pi;
					float dx = (float) Math.Cos(innerAngle), dy = (float) Math.Sin(innerAngle);

					// Create a vertex.
					var normal = new Vector3(dx, dy, 0);
					var position = normal*thickness/2;
					var textureCoordinate = new Vector2(u, v);

					Vector3.Transform(ref position, ref transform, out position);
					Vector3.TransformNormal(ref normal, ref transform, out normal);

					vertices.Add(new VertexPositionNormalTexture(position, normal, textureCoordinate));

					// And create indices for two triangles.
					int nextI = (i + 1)%stride;
					int nextJ = (j + 1)%stride;

					indices.Add(i*stride + j);
					indices.Add(i*stride + nextJ);
					indices.Add(nextI*stride + j);

					indices.Add(i*stride + nextJ);
					indices.Add(nextI*stride + nextJ);
					indices.Add(nextI*stride + j);
				}
			}

			return CreateMesh(device, vertices.ToArray(), indices.ToArray());
		}

		#endregion

		#region XZ Grid

		public static Model CreateXZGrid(GraphicsDevice device, Vector2 size)
		{
			var vertices = new List<VertexPositionNormalTexture>();
			var indices = new List<int>();

			var index = 0;

			var half = size/2;

			// Z lines
			for (var i = 0; i <= size.X; ++i)
			{
				var x = i - half.X;

				vertices.Add(new VertexPositionNormalTexture(new Vector3(x, 0, -half.Y), Vector3.Zero, Vector2.Zero));
				indices.Add(index++);
				vertices.Add(new VertexPositionNormalTexture(new Vector3(x, 0, half.Y), Vector3.Zero, Vector2.Zero));
				indices.Add(index++);
			}

			// X lines
			for (var i = 0; i <= size.Y; ++i)
			{
				var z = i - half.Y;

				vertices.Add(new VertexPositionNormalTexture(new Vector3(-half.X, 0, z), Vector3.Zero, Vector2.Zero));
				indices.Add(index++);
				vertices.Add(new VertexPositionNormalTexture(new Vector3(half.X, 0, z), Vector3.Zero, Vector2.Zero));
				indices.Add(index++);
			}

			return CreateMesh(device, vertices.ToArray(), indices.ToArray(), PrimitiveType.LineList);
		}

		#endregion
	}
}