using Myra.Graphics2D.UI;
using System;
using System.Threading;

namespace MyraPad
{
	internal class AsyncTasksQueue
	{
		private bool _running = true;
		private string _projectXml;
		private string _objectXml;

		private readonly AutoResetEvent _refreshProjectEvent = new AutoResetEvent(false);
		private readonly AutoResetEvent _waitForQuitEvent = new AutoResetEvent(false);

		public AsyncTasksQueue()
		{
			ThreadPool.QueueUserWorkItem(RefreshProc);
		}

		public void QueueLoadProject(string xml)
		{
			_projectXml = xml;
			_refreshProjectEvent.Set();
		}

		public void QueueLoadObject(string xml)
		{
			_objectXml = xml;
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

				if (!string.IsNullOrEmpty(_projectXml))
				{
					try
					{
						Studio.Instance.QueueSetStatusText("Reloading Project...");
						Studio.Instance.NewProject = Project.LoadFromXml(_projectXml, Studio.Instance.AssetManager);
						Studio.Instance.QueueSetStatusText(string.Empty);
					}
					catch (Exception ex)
					{
						Studio.Instance.QueueSetStatusText(ex.Message);
					}

					_projectXml = null;
				}

				if (!string.IsNullOrEmpty(_objectXml))
				{
					if (Studio.Instance.Project != null)
					{
						try
						{
							Studio.Instance.QueueSetStatusText("Reloading Object...");
							Studio.Instance.NewObject = Studio.Instance.Project.LoadObjectFromXml(_objectXml, Studio.Instance.AssetManager);
							Studio.Instance.QueueSetStatusText(string.Empty);
						}
						catch (Exception ex)
						{
							Studio.Instance.QueueSetStatusText(ex.Message);
						}
					}

					_objectXml = null;
				}
			}

			_waitForQuitEvent.Set();
		}
	}
}
