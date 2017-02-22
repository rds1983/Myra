using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics;
using Myra.Graphics3D.Modeling;
using DirectionalLight = Myra.Graphics3D.Environment.DirectionalLight;

namespace Myra.Graphics3D.Rendering
{
	public class ModelBatch
	{
		private readonly MultiCompileEffect _effect;

		private BasicEffect _basicEffect;

		private DirectionalLight[] _lights;
		private readonly GraphicsDevice _device;
		private readonly List<ModelInstance> _items = new List<ModelInstance>();

		public Camera Camera { get; private set; }

		public List<ModelInstance> Items
		{
			get { return _items; }
		}

		public ModelBatch(GraphicsDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}

			_device = device;
			_effect = DefaultAssets.DefaultEffect;
		}

		public Effect GetEffect(bool hasLight, bool hasTexture)
		{
			var defines = new List<string>();

			if (hasLight)
			{
				defines.Add("LIGHTNING");
			}

			if (hasTexture)
			{
				defines.Add("TEXTURE");
			}

			return _effect.GetEffect(defines.ToArray());
		}

		public void Begin(Camera camera, DirectionalLight[] lights = null)
		{
			if (camera == null)
			{
				throw new ArgumentNullException("camera");
			}

			Camera = camera;
			this._lights = lights;
		}

		public void Add(ModelInstance mesh)
		{
			_items.Add(mesh);
		}

		public void End()
		{
			var oldState = _device.DepthStencilState;

			try
			{
				_device.DepthStencilState = DepthStencilState.Default;

				// Set the View matrix which defines the camera and what it's looking at
				Camera.Viewport = new Vector2(_device.Viewport.Width, _device.Viewport.Height);

				var viewProjection = Camera.View*Camera.Projection;

				// Apply the effect and render items
				foreach (var item in _items)
				{
					var material = item.Material;
					if (material == null)
					{
						continue;
					}

					var hasLight = material.HasLight && _lights != null && _lights.Length > 0;

					var effect = GetEffect(hasLight, item.Material.Texture != null);

					var mesh = item.Model;
					_device.SetVertexBuffer(mesh.VertexBuffer);
					_device.Indices = mesh.IndexBuffer;

					var worldViewProj = item.Transform*viewProjection;

					effect.Parameters["_worldViewProj"].SetValue(worldViewProj);
					effect.Parameters["_diffuseColor"].SetValue(material.DiffuseColor.ToVector4());

					if (item.Material.Texture != null)
					{
						effect.Parameters["_texture"].SetValue(material.Texture);
					}

					if (hasLight)
					{
						var worldInverseTranspose = Matrix.Transpose(Matrix.Invert(item.Transform));
						effect.Parameters["_world"].SetValue(item.Transform);
						effect.Parameters["_eyePosition"].SetValue(Camera.Position);
						effect.Parameters["_worldInverseTranspose"].SetValue(worldInverseTranspose);

						_device.BlendState = BlendState.AlphaBlend;
						for (var i = 0; i < _lights.Length; ++i)
						{
							if (i == 1)
							{
								_device.BlendState = BlendState.Additive;
							}

							var dl = _lights[i];

							effect.Parameters["_lightDir"].SetValue(dl.NormalizedDirection);
							effect.Parameters["_lightColor"].SetValue(dl.Color.ToVector3());
							effect.Parameters["_specularPower"].SetValue(16.0f);

							foreach (var pass in effect.CurrentTechnique.Passes)
							{
								pass.Apply();
								_device.DrawIndexedPrimitives(mesh.PrimitiveType, 0, 0, 0, 0, mesh.PrimitiveCount);
							}
						}
					}
					else
					{
						foreach (var pass in effect.CurrentTechnique.Passes)
						{
							pass.Apply();

							_device.DrawIndexedPrimitives(mesh.PrimitiveType, 0, 0, 0, 0, mesh.PrimitiveCount);
						}
					}
				}

				_items.Clear();
			}
			finally
			{
				_device.DepthStencilState = oldState;
			}
		}


		private static void SetMonoGameDirectionalLight(Microsoft.Xna.Framework.Graphics.DirectionalLight mgLight,
			DirectionalLight light)
		{
			mgLight.DiffuseColor = light.Color.ToVector3();
			mgLight.SpecularColor = mgLight.DiffuseColor;
			mgLight.Direction = light.Direction;
			mgLight.Enabled = true;
		}

		public void End2()
		{
			var oldState = _device.DepthStencilState;

			try
			{
				_device.DepthStencilState = DepthStencilState.Default;

				if (_basicEffect == null)
				{
					_basicEffect = new BasicEffect(_device);
				}

				// Set the View matrix which defines the camera and what it's looking at
				Camera.Viewport = new Vector2(_device.Viewport.Width, _device.Viewport.Height);

				_basicEffect.View = Camera.View;
				_basicEffect.Projection = Camera.Projection;

				// Apply the effect and render items
				foreach (var item in _items)
				{
					if (item.Material == null)
					{
						continue;
					}

					if (item.Material.HasLight && _lights != null && _lights.Length > 0)
					{
						_basicEffect.LightingEnabled = true;

						if (_lights.Length > 0)
						{
							SetMonoGameDirectionalLight(_basicEffect.DirectionalLight0, _lights[0]);
						}
						else
						{
							_basicEffect.DirectionalLight0.Enabled = false;
						}

						if (_lights.Length > 1)
						{
							SetMonoGameDirectionalLight(_basicEffect.DirectionalLight1, _lights[1]);
						}
						else
						{
							_basicEffect.DirectionalLight1.Enabled = false;
						}

						if (_lights.Length > 2)
						{
							SetMonoGameDirectionalLight(_basicEffect.DirectionalLight2, _lights[2]);
						}
						else
						{
							_basicEffect.DirectionalLight2.Enabled = false;
						}
					}
					else
					{
						_basicEffect.LightingEnabled = false;
					}

					if (item.Material.Texture != null)
					{
						_basicEffect.Texture = item.Material.Texture;
						_basicEffect.TextureEnabled = true;
					}
					else
					{
						_basicEffect.TextureEnabled = false;
					}

					_basicEffect.DiffuseColor = item.Material.DiffuseColor.ToVector3();
					_basicEffect.World = item.Transform;
					var mesh = item.Model;
					_device.SetVertexBuffer(mesh.VertexBuffer);
					_device.Indices = mesh.IndexBuffer;
					foreach (var pass in _basicEffect.CurrentTechnique.Passes)
					{
						pass.Apply();

						_device.DrawIndexedPrimitives(mesh.PrimitiveType, 0, 0, mesh.VertexBuffer.VertexCount, 0,
							mesh.PrimitiveCount);
					}
				}

				_items.Clear();
			}
			finally
			{
				_device.DepthStencilState = oldState;
			}
		}
	}
}