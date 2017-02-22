using System;
using Microsoft.Xna.Framework;
using Myra.Edit;
using Myra.Graphics3D.Materials;
using Newtonsoft.Json;

namespace Myra.Graphics3D.Modeling
{
	public class ModelInstance: ItemWithId
	{
		private readonly Model _model;

		public Model Model
		{
			get
			{
				return _model;
			}
		}

		public float RotationX { get; set; }
		public float RotationY { get; set; }
		public float RotationZ { get; set; }

		public Vector3 Scale { get; set; }
		public Vector3 Translate { get; set; }

		[HiddenInEditor]
		[JsonIgnore]
		public Matrix Transform
		{
			get
			{
				return Matrix.CreateRotationY(MathHelper.ToRadians(RotationX)) *
					   Matrix.CreateRotationX(MathHelper.ToRadians(RotationY)) *
					   Matrix.CreateRotationZ(MathHelper.ToRadians(RotationZ)) *
					   Matrix.CreateScale(Scale) *
					   Matrix.CreateTranslation(Translate);
			}
		}

		[HiddenInEditor]
		public BaseMaterial Material { get; set; }

		public ModelInstance(Model model)
		{
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}

			_model = model;
			Scale = new Vector3(1, 1, 1);
		}
	}
}
