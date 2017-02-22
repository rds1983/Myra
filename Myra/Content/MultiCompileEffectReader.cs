using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics;

namespace Myra.Content
{
	public class MultiCompileEffectReader : ContentTypeReader<MultiCompileEffect>
	{
		protected override MultiCompileEffect Read(ContentReader input, MultiCompileEffect existingInstance)
		{
			var graphicsDeviceService = input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
			if (graphicsDeviceService == null)
			{
				throw new InvalidOperationException("No Graphics Device Service");
			}

			return MultiCompileEffect.CreateFromReader(graphicsDeviceService.GraphicsDevice, input);
		}
	}
}
