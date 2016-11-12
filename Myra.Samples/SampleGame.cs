using System;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

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