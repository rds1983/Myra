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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GdxSkinImport.MonoGdx;

public class NinePatch
{
	private const int TopLeft = 0;
	private const int TopCenter = 1;
	private const int TopRight = 2;
	private const int MiddleLeft = 3;
	private const int MiddleCenter = 4;
	private const int MiddleRight = 5;
	private const int BottomLeft = 6;
	private const int BottomCenter = 7;
	private const int BottomRight = 8;

	private int _bottomLeft = -1;
	private int _bottomCenter = -1;
	private int _bottomRight = -1;
	private int _middleLeft = -1;
	private int _middleCenter = -1;
	private int _middleRight = -1;
	private int _topLeft = -1;
	private int _topCenter = -1;
	private int _topRight = -1;

	private int _padLeft = -1;
	private int _padRight = -1;
	private int _padTop = -1;
	private int _padBottom = -1;

	private VertexPositionColorTexture[] _vertices = new VertexPositionColorTexture[9 * 4];
	private int _index;

	private NinePatch()
	{
		Color = Color.White;
	}

	public NinePatch(TextureContext texture, int left, int right, int top, int bottom)
		: this(new TextureRegion(texture), left, right, top, bottom)
	{ }

	public NinePatch(TextureRegion region, int left, int right, int top, int bottom)
		: this()
	{
		if (region == null)
			throw new ArgumentNullException("region");

		int middleWidth = region.RegionWidth - left - right;
		int middleHeight = region.RegionHeight - top - bottom;

		TextureRegion[] patches = new TextureRegion[9];
		if (top > 0)
		{
			if (left > 0)
				patches[TopLeft] = new TextureRegion(region, 0, 0, left, top);
			if (middleWidth > 0)
				patches[TopCenter] = new TextureRegion(region, left, 0, middleWidth, top);
			if (right > 0)
				patches[TopRight] = new TextureRegion(region, left + middleWidth, 0, right, top);
		}
		if (middleHeight > 0)
		{
			if (left > 0)
				patches[MiddleLeft] = new TextureRegion(region, 0, top, left, middleHeight);
			if (middleWidth > 0)
				patches[MiddleCenter] = new TextureRegion(region, left, top, middleWidth, middleHeight);
			if (right > 0)
				patches[MiddleRight] = new TextureRegion(region, left + middleWidth, top, right, middleHeight);
		}
		if (bottom > 0)
		{
			if (left > 0)
				patches[BottomLeft] = new TextureRegion(region, 0, top + middleHeight, left, bottom);
			if (middleWidth > 0)
				patches[BottomCenter] = new TextureRegion(region, left, top + middleHeight, middleWidth, bottom);
			if (right > 0)
				patches[BottomRight] = new TextureRegion(region, left + middleWidth, top + middleHeight, right, bottom);
		}

		// If split only vertical, move splits from right to center.
		if (left == 0 && middleWidth == 0)
		{
			patches[TopCenter] = patches[TopRight];
			patches[MiddleCenter] = patches[MiddleRight];
			patches[BottomCenter] = patches[BottomRight];
			patches[TopRight] = null;
			patches[MiddleRight] = null;
			patches[BottomRight] = null;
		}

		// If split only horizontal, move splits from bottom to center.
		if (top == 0 && middleHeight == 0)
		{
			patches[MiddleLeft] = patches[BottomLeft];
			patches[MiddleCenter] = patches[BottomCenter];
			patches[MiddleRight] = patches[BottomRight];
			patches[BottomLeft] = null;
			patches[BottomCenter] = null;
			patches[BottomRight] = null;
		}

		Load(patches);
	}

	public NinePatch(TextureContext texture, Color color)
		: this(texture)
	{
		Color = color;
	}

	public NinePatch(TextureContext texture)
		: this(new TextureRegion(texture))
	{ }

	public NinePatch(TextureRegion region, Color color)
		: this(region)
	{
		Color = color;
	}

	public NinePatch(params TextureRegion[] patches)
		: this()
	{
		if (patches.Length == 1)
		{
			Load(new TextureRegion[] {
					null, null, null,
					null, patches[0], null,
					null, null, null,
				});
			return;
		}

		if (patches.Length != 9)
			throw new ArgumentException("NinePatch needs nine TextureRegions.");

		Load(patches);

		float leftWidth = LeftWidth;
		if ((patches[TopLeft] != null && patches[TopLeft].RegionWidth != leftWidth)
			|| (patches[MiddleLeft] != null && patches[MiddleLeft].RegionWidth != leftWidth)
			|| (patches[BottomLeft] != null && patches[BottomLeft].RegionWidth != leftWidth))
			throw new Exception("Left side patches must have same width.");

		float rightWidth = RightWidth;
		if ((patches[TopRight] != null && patches[TopRight].RegionWidth != rightWidth)
			|| (patches[MiddleRight] != null && patches[MiddleRight].RegionWidth != rightWidth)
			|| (patches[BottomRight] != null && patches[BottomRight].RegionWidth != rightWidth))
			throw new Exception("Right side patches must have same width.");

		float bottomHeight = BottomHeight;
		if ((patches[BottomLeft] != null && patches[BottomLeft].RegionHeight != bottomHeight)
			|| (patches[BottomCenter] != null && patches[BottomCenter].RegionHeight != bottomHeight)
			|| (patches[BottomRight] != null && patches[BottomRight].RegionHeight != bottomHeight))
			throw new Exception("Bottom side patches must have same height.");

		float topHeight = TopHeight;
		if ((patches[TopLeft] != null && patches[TopLeft].RegionHeight != topHeight)
			|| (patches[TopCenter] != null && patches[TopCenter].RegionHeight != topHeight)
			|| (patches[TopRight] != null && patches[TopRight].RegionHeight != topHeight))
			throw new Exception("Top side patches must have same height.");
	}

	public NinePatch(NinePatch ninePatch)
		: this(ninePatch, ninePatch.Color)
	{ }

	public NinePatch(NinePatch ninePatch, Color color)
		: this()
	{
		Texture = ninePatch.Texture;

		_bottomLeft = ninePatch._bottomLeft;
		_bottomCenter = ninePatch._bottomCenter;
		_bottomRight = ninePatch._bottomRight;
		_middleLeft = ninePatch._middleLeft;
		_middleCenter = ninePatch._middleCenter;
		_middleRight = ninePatch._middleRight;
		_topLeft = ninePatch._topLeft;
		_topCenter = ninePatch._topCenter;
		_topRight = ninePatch._topRight;

		LeftWidth = ninePatch.LeftWidth;
		RightWidth = ninePatch.RightWidth;
		MiddleWidth = ninePatch.MiddleWidth;
		MiddleHeight = ninePatch.MiddleHeight;
		TopHeight = ninePatch.TopHeight;
		BottomHeight = ninePatch.BottomHeight;

		_vertices = new VertexPositionColorTexture[ninePatch._vertices.Length];
		_index = ninePatch._index;
		Array.Copy(ninePatch._vertices, _vertices, _vertices.Length);

		Color = ninePatch.Color;
	}

	private void Load(TextureRegion[] patches)
	{
		Color color = Color.White;

		if (patches[BottomLeft] != null)
		{
			_bottomLeft = Add(patches[BottomLeft], color);
			LeftWidth = patches[BottomLeft].RegionWidth;
			BottomHeight = patches[BottomLeft].RegionHeight;
		}
		if (patches[BottomCenter] != null)
		{
			_bottomCenter = Add(patches[BottomCenter], color);
			MiddleWidth = Math.Max(MiddleWidth, patches[BottomCenter].RegionWidth);
			BottomHeight = Math.Max(BottomHeight, patches[BottomCenter].RegionHeight);
		}
		if (patches[BottomRight] != null)
		{
			_bottomRight = Add(patches[BottomRight], color);
			RightWidth = Math.Max(RightWidth, patches[BottomRight].RegionWidth);
			BottomHeight = Math.Max(BottomHeight, patches[BottomRight].RegionHeight);
		}

		if (patches[MiddleLeft] != null)
		{
			_middleLeft = Add(patches[MiddleLeft], color);
			LeftWidth = Math.Max(LeftWidth, patches[MiddleLeft].RegionWidth);
			MiddleHeight = Math.Max(MiddleHeight, patches[MiddleLeft].RegionHeight);
		}
		if (patches[MiddleCenter] != null)
		{
			_middleCenter = Add(patches[MiddleCenter], color);
			MiddleWidth = Math.Max(MiddleWidth, patches[MiddleCenter].RegionWidth);
			MiddleHeight = Math.Max(MiddleHeight, patches[MiddleCenter].RegionHeight);
		}
		if (patches[MiddleRight] != null)
		{
			_middleRight = Add(patches[MiddleRight], color);
			RightWidth = Math.Max(RightWidth, patches[MiddleRight].RegionWidth);
			MiddleHeight = Math.Max(MiddleHeight, patches[MiddleRight].RegionHeight);
		}

		if (patches[TopLeft] != null)
		{
			_topLeft = Add(patches[TopLeft], color);
			LeftWidth = Math.Max(LeftWidth, patches[TopLeft].RegionWidth);
			TopHeight = Math.Max(TopHeight, patches[TopLeft].RegionHeight);
		}
		if (patches[TopCenter] != null)
		{
			_topCenter = Add(patches[TopCenter], color);
			MiddleWidth = Math.Max(MiddleWidth, patches[TopCenter].RegionWidth);
			TopHeight = Math.Max(TopHeight, patches[TopCenter].RegionHeight);
		}
		if (patches[TopRight] != null)
		{
			_topRight = Add(patches[TopRight], color);
			RightWidth = Math.Max(RightWidth, patches[TopRight].RegionWidth);
			TopHeight = Math.Max(TopHeight, patches[TopRight].RegionHeight);
		}

		if (_index < _vertices.Length)
		{
			VertexPositionColorTexture[] newVertices = new VertexPositionColorTexture[_index];
			Array.Copy(_vertices, newVertices, _index);
			_vertices = newVertices;
		}
	}

	private int Add(TextureRegion region, Color color)
	{
		if (Texture == null)
			Texture = region.Texture;
		else if (Texture != region.Texture)
			throw new ArgumentException("All regions must be from the same texture");

		float u = region.U;
		float v = region.V2;
		float u2 = region.U2;
		float v2 = region.V;

		_vertices[_index + 0].Color = color;
		_vertices[_index + 0].TextureCoordinate = new Vector2(u, v);

		_vertices[_index + 1].Color = color;
		_vertices[_index + 1].TextureCoordinate = new Vector2(u, v2);

		_vertices[_index + 2].Color = color;
		_vertices[_index + 2].TextureCoordinate = new Vector2(u2, v2);

		_vertices[_index + 3].Color = color;
		_vertices[_index + 3].TextureCoordinate = new Vector2(u2, v);

		_index += 4;

		return _index - 4;
	}

	public Color Color { get; set; }

	public TextureContext Texture { get; private set; }

	public float LeftWidth { get; set; }
	public float RightWidth { get; set; }
	public float TopHeight { get; set; }
	public float BottomHeight { get; set; }
	public float MiddleWidth { get; set; }
	public float MiddleHeight { get; set; }

	public float TotalWidth
	{
		get { return LeftWidth + MiddleWidth + RightWidth; }
	}

	public float TotalHeight
	{
		get { return TopHeight + MiddleHeight + BottomHeight; }
	}

	public void SetPadding(int left, int right, int top, int bottom)
	{
		_padLeft = left;
		_padRight = right;
		_padTop = top;
		_padBottom = bottom;
	}

	public float PadLeft
	{
		get { return (_padLeft == -1) ? LeftWidth : _padLeft; }
		set { _padLeft = (int)value; }
	}

	public float PadRight
	{
		get { return (_padRight == -1) ? RightWidth : _padRight; }
		set { _padRight = (int)value; }
	}

	public float PadTop
	{
		get { return (_padTop == -1) ? TopHeight : _padTop; }
		set { _padTop = (int)value; }
	}

	public float PadBottom
	{
		get { return (_padBottom == -1) ? BottomHeight : _padBottom; }
		set { _padBottom = (int)value; }
	}
}
