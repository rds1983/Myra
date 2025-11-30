/**
 * Copyright 2013 See AUTHORS file.
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
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace GdxSkinImport.MonoGdx;

public class TextureContext
{
	private static Dictionary<int, SamplerState> _samplerCache = new Dictionary<int, SamplerState>();

	private TextureFilter _filter = TextureFilter.Point;
	private TextureAddressMode _wrapU = TextureAddressMode.Clamp;
	private TextureAddressMode _wrapV = TextureAddressMode.Clamp;
	private SamplerState _samplerState;

	public string TexturePath { get; }

	public TextureFilter Filter
	{
		get { return _filter; }
		set
		{
			if (_filter != value)
			{
				_filter = value;
				_samplerState = null;
			}
		}
	}

	public TextureAddressMode WrapU
	{
		get { return _wrapU; }
		set
		{
			if (_wrapU != value)
			{
				_wrapU = value;
				_samplerState = null;
			}
		}
	}

	public TextureAddressMode WrapV
	{
		get { return _wrapV; }
		set
		{
			if (_wrapV != value)
			{
				_wrapV = value;
				_samplerState = null;
			}
		}
	}

	public SamplerState SamplerState
	{
		get
		{
			if (_samplerState == null)
			{
				if (!_samplerCache.TryGetValue(Key, out _samplerState))
				{
					_samplerState = new SamplerState()
					{
						Filter = Filter,
						AddressU = WrapU,
						AddressV = WrapV,
					};
					_samplerCache[Key] = _samplerState;
				}
			}

			return _samplerState;
		}
	}

	public Texture2D Texture { get; }

	public int Width => Texture.Width;

	public int Height => Texture.Height;

	private int Key
	{
		get { return (byte)Filter << 16 | (byte)WrapU << 8 | (byte)WrapV; }
	}

	public TextureContext(GraphicsDevice device, string texturePath)
	{
		TexturePath = texturePath;
		Texture = Texture2D.FromFile(device, texturePath);
	}
}
