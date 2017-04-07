using System;
using Microsoft.Xna.Framework.Content;

namespace Myra.Assets
{
	public class ContentLoader<T>: IAssetLoader<T>
	{
		private ContentManager _contentManager;

		public ContentManager ContentManager
		{
			get
			{
				return _contentManager;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_contentManager = value;
			}
		}

		public ContentLoader(ContentManager contentManager)
		{
			ContentManager = contentManager;
		}


		public T Load(AssetLoaderContext context, string assetName)
		{
			return _contentManager.Load<T>(assetName);
		}
	}
}
