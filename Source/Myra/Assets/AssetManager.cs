using System;
using System.Collections.Generic;
using System.IO;
using Myra.Attributes;
using Myra.Utility;

namespace Myra.Assets
{
	public class AssetManager
	{
		private readonly Dictionary<Type, object> _loaders = new Dictionary<Type, object>();
		private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
		private IAssetResolver _assetResolver;

		public IAssetResolver AssetResolver
		{
			get { return _assetResolver; }

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_assetResolver = value;
			}
		}

		public Dictionary<Type, object> Loaders
		{
			get { return _loaders; }
		}

		public AssetManager(IAssetResolver assetResolver)
		{
			AssetResolver = assetResolver;
			RegisterDefaultLoaders();
		}

		private void RegisterDefaultLoaders()
		{
			SetAssetLoader(new Texture2DLoader());
			SetAssetLoader(new BitmapFontLoader());
			SetAssetLoader(new TextureRegion2DLoader());
			SetAssetLoader(new TextureAtlasLoader());
		}

		public void SetAssetLoader<T>(IAssetLoader<T> loader)
		{
			_loaders[typeof (T)] = loader;
		}

		public void ClearCache()
		{
			_cache.Clear();
		}

		/// <summary>
		/// Opens a stream specified by asset path
		/// Throws an exception on failure
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public Stream Open(string path)
		{
			var stream = _assetResolver.Open(path);
			if (stream == null)
			{
				throw new Exception(string.Format("Can't open asset {0}", path));
			}
	
			return stream;
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

		public T Load<T>(string name)
		{
			object cached;
			if (_cache.TryGetValue(name, out cached))
			{
				return (T) cached;
			}

			var type = typeof (T);
			object loaderBase;
			if (!_loaders.TryGetValue(type, out loaderBase))
			{
				// Try determine it using AssetLoader attribute
				var attr = type.FindAttribute<AssetLoaderAttribute>();
				if (attr == null)
				{
					throw new Exception(string.Format("Unable to resolve AssetLoader for type {0}", type.Name));
				}

				// Create loader
				loaderBase = Activator.CreateInstance(attr.AssetLoaderType);

				// Save in the _loaders
				_loaders[type] = loaderBase;
			}

			var loader = (IAssetLoader<T>) loaderBase;
			var result = loader.Load(this, name);
			_cache[name] = result;

			return result;
		}
	}
}