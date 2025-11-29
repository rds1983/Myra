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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GdxSkinImport.MonoGdx;
public class Sprite : TextureRegion
{
	private bool _dirty;
	private float _x;
	private float _y;
	private float _width;
	private float _height;
	private float _originX;
	private float _originY;
	private float _rotation;
	private float _scaleX = 1;
	private float _scaleY = 1;
	private Color _color;

	private VertexPositionColorTexture[] _computed = new VertexPositionColorTexture[4];

	public Sprite()
	{
		Color = new Color(1f, 1f, 1f, 1f);
	}

	public Sprite(TextureContext texture)
		: this(texture, 0, 0, texture.Width, texture.Height)
	{ }

	public Sprite(TextureContext texture, int srcWidth, int srcHeight)
		: this(texture, 0, 0, srcWidth, srcHeight)
	{ }

	public Sprite(TextureContext texture, int srcX, int srcY, int srcWidth, int srcHeight)
	{
		if (texture == null)
			throw new ArgumentNullException("texture");

		Texture = texture;
		Color = new Color(1f, 1f, 1f, 1f);

		SetRegion(srcX, srcY, srcWidth, srcHeight);
		SetSize(Math.Abs(srcWidth), Math.Abs(srcHeight));
		SetOrigin(Width / 2, Height / 2);
	}

	public Sprite(TextureRegion region)
	{
		Color = new Color(1f, 1f, 1f, 1f);

		SetRegion(region);
		SetSize(region.RegionWidth, region.RegionHeight);
		SetOrigin(Width / 2, Height / 2);
	}

	public Sprite(TextureRegion region, int srcX, int srcY, int srcWidth, int srcHeight)
	{
		Color = new Color(1f, 1f, 1f, 1f);

		SetRegion(region, srcX, srcY, srcWidth, srcHeight);
		SetSize(Math.Abs(srcWidth), Math.Abs(srcHeight));
		SetOrigin(Width / 2, Height / 2);
	}

	public Sprite(Sprite sprite)
	{
		Set(sprite);
	}

	public void Set(Sprite sprite)
	{
		if (sprite == null)
			throw new ArgumentNullException("sprite");

		Texture = sprite.Texture;
		U = sprite.U;
		V = sprite.V;
		U2 = sprite.U2;
		V2 = sprite.V2;
		_x = sprite._x;
		_y = sprite._y;
		_width = sprite._width;
		_height = sprite._height;
		_originX = sprite._originX;
		_originY = sprite._originY;
		_scaleX = sprite._scaleX;
		_scaleY = sprite._scaleY;
		Color = sprite.Color;
		_dirty = sprite._dirty;
	}

	public virtual void SetBounds(float x, float y, float width, float height)
	{
		_x = x;
		_y = y;
		_width = width;
		_height = height;

		if (_dirty)
			return;

		float x2 = x + width;
		float y2 = y + height;

		_computed[0].Position = new Vector3(x, y, 0);
		_computed[1].Position = new Vector3(x, y2, 0);
		_computed[2].Position = new Vector3(x2, y2, 0);
		_computed[3].Position = new Vector3(x2, y, 0);

		if (Rotation != 0 || ScaleX != 1 || ScaleY != 1)
			_dirty = true;
	}

	public virtual void SetSize(float width, float height)
	{
		_width = width;
		_height = height;

		if (_dirty)
			return;

		float x2 = _x + width;
		float y2 = _y + height;

		_computed[0].Position = new Vector3(_x, _y, 0);
		_computed[1].Position = new Vector3(_x, y2, 0);
		_computed[2].Position = new Vector3(x2, y2, 0);
		_computed[3].Position = new Vector3(x2, _y, 0);

		if (Rotation != 0 || ScaleX != 1 || ScaleY != 1)
			_dirty = true;
	}

	public virtual void SetPosition(float x, float y)
	{
		Translate(x - X, y - Y);
	}

	public virtual float X
	{
		get { return _x; }
		set { TranslateX(value - _x); }
	}

	public virtual float Y
	{
		get { return _y; }
		set { TranslateY(value - _y); }
	}

	public void TranslateX(float xAmount)
	{
		_x += xAmount;

		if (_dirty)
			return;

		_computed[0].Position.X += xAmount;
		_computed[1].Position.X += xAmount;
		_computed[2].Position.X += xAmount;
		_computed[3].Position.X += xAmount;
	}

	public void TranslateY(float yAmount)
	{
		_y += yAmount;

		if (_dirty)
			return;

		_computed[0].Position.Y += yAmount;
		_computed[1].Position.Y += yAmount;
		_computed[2].Position.Y += yAmount;
		_computed[3].Position.Y += yAmount;
	}

	public void Translate(float xAmount, float yAmount)
	{
		_x += xAmount;
		_y += yAmount;

		if (_dirty)
			return;

		_computed[0].Position.X += xAmount;
		_computed[0].Position.Y += yAmount;

		_computed[1].Position.X += xAmount;
		_computed[1].Position.Y += yAmount;

		_computed[2].Position.X += xAmount;
		_computed[2].Position.Y += yAmount;

		_computed[3].Position.X += xAmount;
		_computed[3].Position.Y += yAmount;
	}

	/*public void SetColor (float r, float g, float b, float a)
	{
		throw new NotImplementedException();
	}

	public void SetColor (float color)
	{
		throw new NotImplementedException();
	}*/

	public virtual void SetOrigin(float originX, float originY)
	{
		_originX = originX;
		_originY = originY;
		_dirty = true;
	}

	public float Rotation
	{
		get { return _rotation; }
		set
		{
			_rotation = value;
			_dirty = true;
		}
	}

	public void Rotate(float angle)
	{
		_rotation += angle;
		_dirty = true;
	}

	public virtual void Rotate90(bool clockwise)
	{
		if (clockwise)
		{
			Vector2 temp = _computed[0].TextureCoordinate;
			_computed[0].TextureCoordinate = _computed[3].TextureCoordinate;
			_computed[3].TextureCoordinate = _computed[2].TextureCoordinate;
			_computed[2].TextureCoordinate = _computed[1].TextureCoordinate;
			_computed[1].TextureCoordinate = temp;
		}
		else
		{
			Vector2 temp = _computed[0].TextureCoordinate;
			_computed[0].TextureCoordinate = _computed[1].TextureCoordinate;
			_computed[1].TextureCoordinate = _computed[2].TextureCoordinate;
			_computed[2].TextureCoordinate = _computed[3].TextureCoordinate;
			_computed[3].TextureCoordinate = temp;
		}
	}

	public void SetScale(float scale)
	{
		_scaleX = scale;
		_scaleY = scale;
		_dirty = true;
	}

	public void SetScale(float scaleX, float scaleY)
	{
		_scaleX = scaleX;
		_scaleY = scaleY;
		_dirty = true;
	}

	public void Scale(float amount)
	{
		_scaleX += amount;
		_scaleY += amount;
		_dirty = true;
	}

	public VertexPositionColorTexture[] Vertices
	{
		get
		{
			if (_dirty)
			{
				_dirty = false;

				float localX = -_originX;
				float localY = -_originY;
				float localX2 = localX + _width;
				float localY2 = localY + _height;
				float worldOriginX = _x - localX;
				float worldOriginY = _y - localY;

				if (_scaleX != 1 || _scaleY != 1)
				{
					localX *= _scaleX;
					localY *= _scaleY;
					localX2 *= _scaleX;
					localY2 *= _scaleY;
				}

				if (_rotation != 0)
				{
					float cos = (float)Math.Cos(_rotation);
					float sin = (float)Math.Sin(_rotation);
					float localXCos = localX * cos;
					float localXSin = localX * sin;
					float localYCos = localY * cos;
					float localYSin = localY * sin;
					float localX2Cos = localX2 * cos;
					float localX2Sin = localX2 * sin;
					float localY2Cos = localY2 * cos;
					float localY2Sin = localY2 * sin;

					float x1 = localXCos - localYSin + worldOriginX;
					float y1 = localYCos + localXSin + worldOriginY;
					float x2 = localXCos - localY2Sin + worldOriginX;
					float y2 = localY2Cos + localXSin + worldOriginY;
					float x3 = localX2Cos - localY2Sin + worldOriginX;
					float y3 = localY2Cos + localX2Sin + worldOriginY;

					_computed[0].Position = new Vector3(x1, y1, 0);
					_computed[1].Position = new Vector3(x2, y2, 0);
					_computed[2].Position = new Vector3(x3, y3, 0);
					_computed[3].Position = new Vector3(x1 + (x3 - x2), y3 - (y2 - y1), 0);
				}
				else
				{
					float x1 = localX + worldOriginX;
					float y1 = localY + worldOriginY;
					float x2 = localX2 + worldOriginX;
					float y2 = localY2 + worldOriginY;

					_computed[0].Position = new Vector3(x1, y1, 0);
					_computed[1].Position = new Vector3(x1, y2, 0);
					_computed[2].Position = new Vector3(x2, y2, 0);
					_computed[3].Position = new Vector3(x2, y1, 0);
				}
			}

			return _computed;
		}
	}

	public virtual float Width
	{
		get { return _width; }
	}

	public virtual float Height
	{
		get { return _height; }
	}

	public virtual float OriginX
	{
		get { return _originX; }
	}

	public virtual float OriginY
	{
		get { return _originY; }
	}

	public virtual float ScaleX
	{
		get { return _scaleX; }
	}

	public virtual float ScaleY
	{
		get { return _scaleY; }
	}

	public Color Color
	{
		get { return _color; }
		set
		{
			_color = value;

			Color pColor = Color.FromNonPremultiplied(value.R, value.G, value.B, value.A);
			_computed[0].Color = pColor;
			_computed[1].Color = pColor;
			_computed[2].Color = pColor;
			_computed[3].Color = pColor;
		}
	}

	public override void SetRegion(float u, float v, float u2, float v2)
	{
		base.SetRegion(u, v, u2, v2);

		_computed[0].TextureCoordinate = new Vector2(u, v2);
		_computed[1].TextureCoordinate = new Vector2(u, v);
		_computed[2].TextureCoordinate = new Vector2(u2, v);
		_computed[3].TextureCoordinate = new Vector2(u2, v2);
	}

	public override float U
	{
		get { return base.U; }
		set
		{
			base.U = value;
			_computed[0].TextureCoordinate.X = value;
			_computed[1].TextureCoordinate.X = value;
		}
	}

	public override float V
	{
		get { return base.V; }
		set
		{
			base.V = value;
			_computed[1].TextureCoordinate.Y = value;
			_computed[2].TextureCoordinate.Y = value;
		}
	}

	public override float U2
	{
		get { return base.U2; }
		set
		{
			base.U2 = value;
			_computed[2].TextureCoordinate.X = value;
			_computed[3].TextureCoordinate.X = value;
		}
	}

	public override float V2
	{
		get { return base.V2; }
		set
		{
			base.V2 = value;
			_computed[0].TextureCoordinate.Y = value;
			_computed[3].TextureCoordinate.Y = value;
		}
	}

	public override void Flip(bool x, bool y)
	{
		base.Flip(x, y);

		if (x & y)
		{
			Vector2 temp = _computed[0].TextureCoordinate;
			_computed[0].TextureCoordinate = _computed[2].TextureCoordinate;
			_computed[2].TextureCoordinate = temp;

			temp = _computed[1].TextureCoordinate;
			_computed[1].TextureCoordinate = _computed[3].TextureCoordinate;
			_computed[3].TextureCoordinate = temp;
		}
		else if (x)
		{
			float temp = _computed[0].TextureCoordinate.X;
			_computed[0].TextureCoordinate.X = _computed[2].TextureCoordinate.X;
			_computed[2].TextureCoordinate.X = temp;

			temp = _computed[1].TextureCoordinate.X;
			_computed[1].TextureCoordinate.X = _computed[3].TextureCoordinate.X;
			_computed[3].TextureCoordinate.X = temp;
		}
		else if (y)
		{
			float temp = _computed[0].TextureCoordinate.Y;
			_computed[0].TextureCoordinate.Y = _computed[2].TextureCoordinate.Y;
			_computed[2].TextureCoordinate.Y = temp;

			temp = _computed[1].TextureCoordinate.Y;
			_computed[1].TextureCoordinate.Y = _computed[3].TextureCoordinate.Y;
			_computed[3].TextureCoordinate.Y = temp;
		}
	}

	public override void Scroll(float xAmount, float yAmount)
	{
		if (xAmount != 0)
		{
			float u = (_computed[0].TextureCoordinate.X + xAmount) % 1f;
			float u2 = u + _width / Texture.Width;

			U = u;
			U2 = u2;

			_computed[0].TextureCoordinate.X = u;
			_computed[1].TextureCoordinate.X = u;
			_computed[2].TextureCoordinate.X = u2;
			_computed[3].TextureCoordinate.X = u2;
		}

		if (yAmount != 0)
		{
			float v = (_computed[1].TextureCoordinate.Y + yAmount) % 1f;
			float v2 = v + _height / Texture.Height;

			V = v;
			V2 = v2;

			_computed[0].TextureCoordinate.Y = v2;
			_computed[1].TextureCoordinate.Y = v;
			_computed[2].TextureCoordinate.Y = v;
			_computed[3].TextureCoordinate.Y = v2;
		}
	}
}
