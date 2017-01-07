using System;
using Microsoft.Xna.Framework;

namespace Myra.Samples
{
	public class SampleGame : Game
	{
		private readonly GraphicsDeviceManager graphics;

		public static readonly Type[] AllSampleTypes;

		static SampleGame()
		{
			AllSampleTypes = new[]
			{
				typeof (HelloWorldSample),
				typeof (FormattedTextSample),
				typeof (TextBlocksSample),
				typeof (GridSample),
				typeof (SplitPaneSample),
				typeof (ScrollPaneSample),
				typeof (Notepad)
			};
		}

		public SampleGame()
		{
			graphics = new GraphicsDeviceManager(this);
			IsMouseVisible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;
		}
	}
}