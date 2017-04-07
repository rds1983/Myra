using System;
using System.IO;

namespace Myra.Assets
{
	public class AssetLoaderContext
	{
		private readonly AssetManager _assetManager;
		private readonly string _baseFolder;

		internal AssetLoaderContext(AssetManager assetManager, string baseFolder)
		{
			if (assetManager == null)
			{
				throw new Exception("assetManager");
			}

			_assetManager = assetManager;
			_baseFolder = baseFolder;
		}

		/// <summary>
		/// Opens a stream specified by asset path
		/// Throws an exception on failure
		/// </summary>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public Stream Open(string assetName)
		{
			return _assetManager.Open(Path.Combine(_baseFolder, assetName));
		}

		/// <summary>
		/// Reads specified asset to string
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string ReadAsText(string path)
		{
			string result;
			using (var input = Open(path))
			{
				using (var textReader = new StreamReader(input))
				{
					result = textReader.ReadToEnd();
				}
			}

			return result;
		}

		public T Load<T>(string assetName)
		{
			return _assetManager.Load<T>(Path.Combine(_baseFolder, assetName));
		}
	}
}
