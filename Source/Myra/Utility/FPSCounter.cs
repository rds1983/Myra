using System;
using System.Collections.Generic;

namespace Myra.Utility
{
	public sealed class FPSCounter
	{
		private const int FPSThreshold = 100;

		private readonly Queue<long> _renderTicksStorage = new Queue<long>();
		private long _renderTicks;
		private long _startTick;

		/// <summary>
		/// FPS
		/// </summary>
		public float FPS { get; private set; }

		public FPSCounter()
		{
			FPS = 0.0f;
			Reset();
		}

		public void Reset()
		{
			_startTick = 0;
			_renderTicks = 0;
			FPS = 0.0f;
		}

		public void Update()
		{
			var tickCount = Environment.TickCount;
			if (_startTick == 0)
			{
				_startTick = tickCount;
			}
			else
			{
				if (_renderTicksStorage.Count >= FPSThreshold)
				{
					_renderTicks -= _renderTicksStorage.Dequeue();
				}
				// Update FPS
				var totalRenderTicks = tickCount - _startTick;
				_renderTicksStorage.Enqueue(totalRenderTicks);
				_renderTicks += totalRenderTicks;

				if (_renderTicks > 0)
				{
					FPS = 1000.0f * _renderTicksStorage.Count / _renderTicks;
				}

				_startTick = tickCount;
			}
		}
	}
}