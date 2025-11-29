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

using System;

namespace GdxSkinImport.MonoGdx;

public class TextureRegion
{
	private float _u;
	private float _v;
	private float _u2;
	private float _v2;
	private int _regionWidth;
	private int _regionHeight;

	public TextureRegion()
	{ }

	public TextureRegion(TextureContext texture)
	{
		if (texture == null)
			throw new ArgumentNullException("texture");

		Texture = texture;
		SetRegion(0, 0, texture.Width, texture.Height);
	}

	public TextureRegion(TextureContext texture, int width, int height)
	{
		Texture = texture;
		SetRegion(0, 0, width, height);
	}

	public TextureRegion(TextureContext texture, int x, int y, int width, int height)
	{
		Texture = texture;
		SetRegion(x, y, width, height);
	}

	public TextureRegion(TextureContext texture, float u, float v, float u2, float v2)
	{
		Texture = texture;
		SetRegion(u, v, u2, v2);
	}

	public TextureRegion(TextureRegion region)
	{
		SetRegion(region);
	}

	public TextureRegion(TextureRegion region, int x, int y, int width, int height)
	{
		SetRegion(region, x, y, width, height);
	}

	public void SetRegion(TextureContext texture)
	{
		Texture = texture;
		SetRegion(0, 0, texture.Width, texture.Height);
	}

	public void SetRegion(int x, int y, int width, int height)
	{
		float invTexWidth = 1f / Texture.Width;
		float invTexHeight = 1f / Texture.Height;

		SetRegion(x * invTexWidth, y * invTexHeight, (x + width) * invTexWidth, (y + height) * invTexHeight);

		_regionWidth = Math.Abs(width);
		_regionHeight = Math.Abs(height);
	}

	public virtual void SetRegion(float u, float v, float u2, float v2)
	{
		_u = u;
		_v = v;
		_u2 = u2;
		_v2 = v2;

		_regionWidth = (int)Math.Round(Math.Abs(u2 - u) * Texture.Width);
		_regionHeight = (int)Math.Round(Math.Abs(v2 - v) * Texture.Height);
	}

	public void SetRegion(TextureRegion region)
	{
		Texture = region.Texture;
		SetRegion(region._u, region._v, region._u2, region._v2);
	}

	public void SetRegion(TextureRegion region, int x, int y, int width, int height)
	{
		Texture = region.Texture;
		SetRegion(region.RegionX + x, region.RegionY + y, width, height);
	}

	public TextureContext Texture { get; set; }

	public virtual float U
	{
		get { return _u; }
		set
		{
			_u = value;
			_regionWidth = (int)Math.Round(Math.Abs(_u2 - _u) * Texture.Width);
		}
	}

	public virtual float V
	{
		get { return _v; }
		set
		{
			_v = value;
			_regionHeight = (int)Math.Round(Math.Abs(_v2 - _v) * Texture.Height);
		}
	}

	public virtual float U2
	{
		get { return _u2; }
		set
		{
			_u2 = value;
			_regionWidth = (int)Math.Round(Math.Abs(_u2 - _u) * Texture.Width);
		}
	}

	public virtual float V2
	{
		get { return _v2; }
		set
		{
			_v2 = value;
			_regionHeight = (int)Math.Round(Math.Abs(_v2 - _v) * Texture.Height);
		}
	}

	public int RegionX
	{
		get { return (int)Math.Round(_u * Texture.Width); }
		set { U = value / (float)Texture.Width; }
	}

	public int RegionY
	{
		get { return (int)Math.Round(_v * Texture.Height); }
		set { V = value / (float)Texture.Height; }
	}

	public int RegionWidth
	{
		get { return _regionWidth; }
		set { U2 = _u + value / (float)Texture.Width; }
	}

	public int RegionHeight
	{
		get { return _regionHeight; }
		set { V2 = _v + value / (float)Texture.Height; }
	}

	public virtual void Flip(bool x, bool y)
	{
		if (x)
		{
			float temp = _u;
			_u = _u2;
			_u2 = temp;
		}

		if (y)
		{
			float temp = _v;
			_v = _v2;
			_v2 = temp;
		}
	}

	public bool IsFlipX
	{
		get { return _u > _u2; }
	}

	public bool IsFlipY
	{
		get { return _v > _v2; }
	}

	public virtual void Scroll(float xAmount, float yAmount)
	{
		if (xAmount != 0)
		{
			float width = (_u2 - _u) * Texture.Width;
			_u = (_u + xAmount) % 1f;
			_u2 = _u + width / Texture.Width;
		}

		if (yAmount != 0)
		{
			float height = (_v2 - _v) * Texture.Height;
			_v = (_v + yAmount) % 1f;
			_v2 = _v + height / Texture.Height;
		}
	}

	public TextureRegion[,] Split(int tileWidth, int tileHeight)
	{
		int x = RegionX;
		int y = RegionY;
		int width = RegionWidth;
		int height = RegionHeight;

		int rows = height / tileHeight;
		int cols = width / tileWidth;

		int startX = x;
		TextureRegion[,] tiles = new TextureRegion[rows, cols];

		for (int row = 0; row < rows; rows++, y += tileHeight)
		{
			x = startX;
			for (int col = 0; col < cols; col++, x += tileWidth)
				tiles[row, col] = new TextureRegion(Texture, x, y, tileWidth, tileHeight);
		}

		return tiles;
	}

	public static TextureRegion[,] Split(TextureContext texture, int tileWidth, int tileHeight)
	{
		TextureRegion region = new TextureRegion(texture);
		return region.Split(tileWidth, tileHeight);
	}
}
