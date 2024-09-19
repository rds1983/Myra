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
						Studio.MainForm.QueueSetStatusText("Reloading Project...");
						Studio.MainForm.NewProject = Project.LoadFromXml(_projectXml, Studio.MainForm.AssetManager);
						Studio.MainForm.QueueSetStatusText(string.Empty);
					}
					catch (Exception ex)
					{
						Studio.MainForm.QueueClearExplorer();
						Studio.MainForm.QueueSetStatusText(ex.Message);
					}

					_projectXml = null;
				}

				if (!string.IsNullOrEmpty(_objectXml))
				{
					if (Studio.MainForm.Project != null)
					{
						try
						{
							Studio.MainForm.QueueSetStatusText("Reloading Object...");
							Studio.MainForm.NewObject = Studio.MainForm.Project.LoadObjectFromXml(_objectXml, Studio.MainForm.AssetManager);
							Studio.MainForm.QueueSetStatusText(string.Empty);
						}
						catch (Exception ex)
						{
							Studio.MainForm.QueueClearExplorer();
							Studio.MainForm.QueueSetStatusText(ex.Message);
						}
					}

					_objectXml = null;
				}
			}

			_waitForQuitEvent.Set();
		}
	}
}
