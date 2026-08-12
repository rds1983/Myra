using AssetManagementBase;
using DigitalRiseModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System;
using System.ComponentModel;
using System.IO;

namespace Myra.Samples.CustomWidgets
{
	/// <summary>
	/// A custom Myra widget that renders a 3D mesh using XNA's <see cref="BasicEffect"/>.
	/// Hooks into the Myra render pipeline to set up a perspective projection, apply
	/// directional lighting and texturing, and draw indexed primitives within the widget's bounds.
	/// Properties exposed via <see cref="Myra.Graphics2D.UI.Properties.PropertyGrid"/> allow
	/// real-time tweaking of colour, specular power, scale, and rotation speed.
	/// </summary>
	public class Scene3D : Widget
	{
		private const float NearPlaneDistance = 0.1f;
		private const float FarPlaneDistance = 1000.0f;
		private const float ViewAngle = 60.0f;

		private BasicEffect _basicEffect;
		private DateTime? _lastDt = null;

		/// <summary>
		/// Gets or sets the diffuse colour applied to the mesh via <see cref="BasicEffect.DiffuseColor"/>.
		/// </summary>
		[Category("3D")]
		public Color Color { get; set; } = Color.Green;

		/// <summary>
		/// Gets or sets the specular exponent used by <see cref="BasicEffect.SpecularPower"/>.
		/// Higher values produce a tighter, more focused highlight.
		/// </summary>
		[Category("3D")]
		public float SpecularPower { get; set; } = 50.0f;

		/// <summary>
		/// Gets or sets the uniform scale applied to the mesh before rendering.
		/// </summary>
		[Category("3D")]
		public float MeshScale { get; set; } = 2.0f;

		/// <summary>
		/// Gets or sets the rotation speed in degrees per second around the Y axis.
		/// </summary>
		[Category("3D")]
		public float DegreesPerSecond { get; set; } = 10.0f;

		/// <summary>
		/// Gets or sets the mesh geometry to render. Set to <c>null</c> to disable rendering.
		/// </summary>
		[Browsable(false)]
		public DrMeshPart Mesh { get; set; }

		private float RotationAngle { get; set; } = 0;

		private Texture2D Texture { get; set; }

		/// <summary>
		/// Initialises the widget, stretches to fill the parent container, and loads
		/// the checker-board texture used for mesh texturing.
		/// </summary>
		public Scene3D()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			var assetManager = AssetManager.CreateFileAssetManager(Path.Combine(AppContext.BaseDirectory, "Assets"));
			Texture = assetManager.LoadTexture2D(MyraEnvironment.GraphicsDevice, "Textures/checker.dds");
		}

		/// <summary>
		/// Renders the 3D mesh inside the widget's bounds. Temporarily suspends Myra's
		/// render context, saves and replaces all relevant GPU device states (viewport,
		/// depth-stencil, rasteriser, blend, sampler), sets up a perspective camera with
		/// directional lighting, draws the indexed primitives, then restores every device
		/// state before returning control to Myra.
		/// </summary>
		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Mesh == null)
			{
				return;
			}

			var device = MyraEnvironment.GraphicsDevice;

			// Lazily create the BasicEffect with a single directional light
			if (_basicEffect == null)
			{
				_basicEffect = new BasicEffect(device)
				{
					LightingEnabled = true
				};

				_basicEffect.DirectionalLight0.Enabled = true;
				_basicEffect.DirectionalLight0.Direction = new Vector3(-1, -1, -1);
				_basicEffect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
				_basicEffect.DirectionalLight0.SpecularColor = Color.White.ToVector3();
			}

			// Suspend the Myra render context so we can issue raw GPU commands
			context.End();

			// Save current device state
			var oldViewPort = device.Viewport;
			var oldDepthStencilState = device.DepthStencilState;
			var oldRasterizerState = device.RasterizerState;
			var oldBlendState = device.BlendState;
			var oldSamplesState = device.SamplerStates[0];

			// Set the new one
			var screenPosition = ToGlobal(Point.Zero);
			device.Viewport = new Viewport(screenPosition.X, screenPosition.Y, ActualBounds.Width, ActualBounds.Height);

			device.DepthStencilState = DepthStencilState.Default;
			device.RasterizerState = RasterizerState.CullCounterClockwise;
			device.BlendState = BlendState.AlphaBlend;
			device.SamplerStates[0] = SamplerState.LinearWrap;

			// Bind the mesh vertex and index buffers to the device
			device.SetVertexBuffer(Mesh.VertexBuffer);
			device.Indices = Mesh.IndexBuffer;

			// Build view, projection, and world matrices for the 3D scene
			var view = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up);
			var projection = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.ToRadians(ViewAngle),
				device.Viewport.AspectRatio,
				NearPlaneDistance, FarPlaneDistance
			);

			// World matrix combines the current rotation and uniform scale
			var world = Matrix.CreateRotationY(MathHelper.ToRadians(RotationAngle)) * Matrix.CreateScale(MeshScale);

			_basicEffect.View = view;
			_basicEffect.World = world;
			_basicEffect.Projection = projection;

			_basicEffect.DiffuseColor = Color.ToVector3();
			_basicEffect.SpecularPower = SpecularPower;

			_basicEffect.Texture = Texture;
			_basicEffect.TextureEnabled = true;

			// Draw the mesh for every effect pass (typically one pass for BasicEffect)
			foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
			{
				pass.Apply();

#if FNA
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
				0,
				_mesh.VertexBuffer.VertexCount,
				0,
				_mesh.PrimitiveCount);
#else
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Mesh.PrimitiveCount);
#endif
			}

			// Advance the rotation angle based on elapsed real time
			var now = DateTime.Now;
			if (_lastDt != null)
			{
				var passed = now - _lastDt.Value;
				var degrees = (float)passed.TotalSeconds * DegreesPerSecond;
				RotationAngle += degrees;
			}

			_lastDt = now;

			// Restore the device state so Myra continues rendering correctly
			device.Viewport = oldViewPort;
			device.DepthStencilState = oldDepthStencilState;
			device.RasterizerState = oldRasterizerState;
			device.BlendState = oldBlendState;
			device.SamplerStates[0] = oldSamplesState;

			// Restart the Myra render context
			context.Begin();
		}
	}
}
