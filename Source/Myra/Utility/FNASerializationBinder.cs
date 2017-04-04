using System;
using Newtonsoft.Json;

namespace Myra.Utility
{
	public class FNASerializationBinder : SerializationBinder
	{
		public override Type BindToType(string assemblyName, string typeName)
		{
			var fullName = typeName + ", " + assemblyName;
			fullName = fullName.Replace(", Myra", ", Myra.FNA");
			fullName = fullName.Replace("MonoGame.Framework", "FNA");

			return Type.GetType(fullName, true);
		}
	}
}