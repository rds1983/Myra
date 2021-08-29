using Myra.Utility;
using System;
using System.IO;
using System.Reflection;

namespace Myra.Assets
{
	public class ResourceAssetResolver : IAssetResolver
	{
		private Assembly _assembly;

		public Assembly Assembly
		{
			get { return _assembly; }

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_assembly = value;
			}
		}

		public string Prefix { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="prefix"></param>
		/// <param name="prependAssemblyName"></param>
		public ResourceAssetResolver(Assembly assembly, string prefix, bool prependAssemblyName = true)
		{
			Assembly = assembly;

			if (prependAssemblyName)
			{
				Prefix = assembly.GetName().Name + "." + prefix;
			}
			else
			{
				Prefix = prefix;
			}

			if (!Prefix.EndsWith("."))
			{
				Prefix += ".";
			}
		}

		public Stream Open(string assetName)
		{
			assetName = assetName.Replace(AssetManager.SeparatorSymbol, '.');

			return Res.OpenResourceStream(Assembly, Prefix + assetName);
		}
	}
}
