using Myra.Graphics2D.UI;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyraPad
{
	internal class AsyncTasksQueue
	{
		private bool _running = true;
		private readonly ConcurrentQueue<string> _projectXmls = new ConcurrentQueue<string>();
		private readonly ConcurrentQueue<string> _objectXmls = new ConcurrentQueue<string>();

		private readonly AutoResetEvent _refreshProjectEvent = new AutoResetEvent(false);
		private readonly AutoResetEvent _waitForQuitEvent = new AutoResetEvent(false);

		public AsyncTasksQueue()
		{
			ThreadPool.QueueUserWorkItem(RefreshProc);
		}

		public void QueueLoadProject(string xml)
		{
			_projectXmls.Enqueue(xml);
			_refreshProjectEvent.Set();
		}

		public void QueueLoadObject(string xml)
		{
			_objectXmls.Enqueue(xml);
			_refreshProjectEvent.Set();
		}

		public void Quit()
		{
			_running = false;
			_refreshProjectEvent.Set();
			_waitForQuitEvent.WaitOne();
		}

		private void RefreshProc(object state)
		{
			while (_running)
			{
				_refreshProjectEvent.WaitOne();

				// Get last project xml
				var projectXml = string.Empty;
				while (_projectXmls.Count > 0)
				{
					_projectXmls.TryDequeue(out projectXml);
				}
				if (!string.IsNullOrEmpty(projectXml))
				{
					try
					{
						Studio.MainForm.QueueSetStatusText("Reloading Project...");
						Studio.MainForm.NewProject = Project.LoadFromXml(projectXml, Studio.MainForm.AssetManager);
						Studio.MainForm.QueueSetStatusText(string.Empty);
					}
					catch (Exception ex)
					{
						Studio.MainForm.QueueClearExplorer();
						Studio.MainForm.QueueSetStatusText(ex.Message);
					}
				}

				var objectXml = string.Empty;
				while (_objectXmls.Count > 0)
				{
					_objectXmls.TryDequeue(out objectXml);
				}
				if (!string.IsNullOrEmpty(objectXml))
				{
					if (Studio.MainForm.Project != null)
					{
						try
						{
							Studio.MainForm.QueueSetStatusText("Reloading Object...");
							Studio.MainForm.NewObject = Studio.MainForm.Project.LoadObjectFromXml(objectXml, Studio.MainForm.AssetManager);
							Studio.MainForm.QueueSetStatusText(string.Empty);
						}
						catch (Exception ex)
						{
							Studio.MainForm.QueueClearExplorer();
							Studio.MainForm.QueueSetStatusText(ex.Message);
						}
					}
				}
			}

			_waitForQuitEvent.Set();
		}
	}
}
