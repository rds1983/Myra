using System;
using System.Collections.Generic;
using System.IO;
using Myra.Utility;

namespace Myra.Assets
{
	public class AssetManager: IAssetManager
	{
		public static readonly AssetManager Default = new AssetManager(new FileAssetResolver(PathUtils.ExecutingAssemblyDirectory));

		private readonly static Dictionary<Type, LoaderInfo> _loaders = new Dictionary<Type, LoaderInfo>();
		private readonly Dictionary<Type, Dictionary<string, object>> _cache = new Dictionary<Type, Dictionary<string, object>>();
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

		public Dictionary<Type, LoaderInfo> Loaders
		{
			get { return _loaders; }
		}

		static AssetManager()
		{
			RegisterDefaultLoaders();
		}

		public AssetManager(IAssetResolver assetResolver)
		{
			AssetResolver = assetResolver ?? throw new ArgumentNullException(nameof(assetResolver));
		}

		private static void RegisterDefaultLoaders()
		{
#if !XENKO
			SetAssetLoader(new SoundEffectLoader());
#endif
			SetAssetLoader(new StringLoader());
			SetAssetLoader(new Texture2DLoader());
			SetAssetLoader(new SpriteFontLoader());
		}

		public static void SetAssetLoader<T>(IAssetLoader<T> loader, bool storeInCache = true)
		{
			_loaders[typeof(T)] = new LoaderInfo(loader, storeInCache);
		}

		public void ClearCache()
		{
			// TODO: resources disposal
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

		public T Load<T>(string assetName)
		{
			var type = typeof(T);
			assetName = assetName.Replace('\\', '/');

			Dictionary<string, object> cache;
			if (_cache.TryGetValue(type, out cache))
			{
				object cached;
				if (cache.TryGetValue(assetName, out cached))
				{
					// Found in cache
					return (T)cached;
				}
			}

			LoaderInfo loaderBase;
			if (!_loaders.TryGetValue(type, out loaderBase))
			{
				// Try determine it using AssetLoader attribute
				var attr = type.FindAttribute<AssetLoaderAttribute>();
				if (attr == null)
				{
					throw new Exception(string.Format("Unable to resolve AssetLoader for type {0}", type.Name));
				}

				// Create loader
				loaderBase = new LoaderInfo(Activator.CreateInstance(attr.AssetLoaderType),
					attr.StoreInCache);

				// Save in the _loaders
				_loaders[type] = loaderBase;
			}

			var loader = (IAssetLoader<T>)loaderBase.Loader;

			var baseFolder = string.Empty;
			var assetFileName = assetName;

			var separatorIndex = assetName.LastIndexOf('/');
			if (separatorIndex != -1)
			{
				baseFolder = assetName.Substring(0, separatorIndex);
				assetFileName = assetName.Substring(separatorIndex + 1);
			}

			var context = new AssetLoaderContext(this, baseFolder);
			var result = loader.Load(context, assetFileName);

			if (loaderBase.StoreInCache)
			{
				// Store in cache
				if (cache == null)
				{
					cache = new Dictionary<string, object>();
					_cache[type] = cache;
				}

				cache[assetName] = result;
			}

			return result;
		}
	}
}