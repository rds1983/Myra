using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.IO;

namespace Myra.UIEditor
{
	public class Viewer : Game
	{
		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private Desktop _desktop;
		private Grid _statisticsGrid;
		private TextBlock _gcMemoryLabel;
		private TextBlock _fpsLabel;
		private TextBlock _widgetsCountLabel;
		private TextBlock _drawCallsLabel;
		//		private readonly FramesPerSecondCounter _fpsCounter = new FramesPerSecondCounter();
		private string _filePath;
		private Project _project;

		public bool ShowDebugInfo
		{
			get
			{
				return _statisticsGrid.Visible;
			}

			set
			{
				_statisticsGrid.Visible = value;
			}
		}

		public Viewer(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				throw new ArgumentNullException("filePath");
			}

			_filePath = filePath;

			_graphicsDeviceManager = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1024,
				PreferredBackBufferHeight = 768
			};
		}

		protected override void Initialize()
		{
			base.Initialize();

			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			MyraEnvironment.Game = this;

			BuildUI();

			var data = File.ReadAllText(_filePath);
			_project = Project.LoadFromXml(data);

			_desktop.Widgets.Add(_project.Root);
		}

		private void BuildUI()
		{
			_desktop = new Desktop();

			_statisticsGrid = new Grid
			{
				Visible = false
			};

			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());
			_statisticsGrid.RowsProportions.Add(new Grid.Proportion());

			_gcMemoryLabel = new TextBlock
			{
				Text = "GC Memory: ",
				Font = DefaultAssets.FontSmall
			};
			_statisticsGrid.Widgets.Add(_gcMemoryLabel);

			_fpsLabel = new TextBlock
			{
				Text = "FPS: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 1
			};

			_statisticsGrid.Widgets.Add(_fpsLabel);

			_widgetsCountLabel = new TextBlock
			{
				Text = "Total Widgets: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 2
			};

			_statisticsGrid.Widgets.Add(_widgetsCountLabel);

			_drawCallsLabel = new TextBlock
			{
				Text = "Draw Calls: ",
				Font = DefaultAssets.FontSmall,
				GridRow = 3
			};

			_statisticsGrid.Widgets.Add(_drawCallsLabel);

			_statisticsGrid.HorizontalAlignment = HorizontalAlignment.Left;
			_statisticsGrid.VerticalAlignment = VerticalAlignment.Bottom;
			_statisticsGrid.Left = 10;
			_statisticsGrid.Top = -10;

			_desktop.Widgets.Add(_statisticsGrid);
		}

		private void DebugOptionsItemOnSelected(object sender1, EventArgs eventArgs)
		{
			var dlg = new DebugOptionsDialog();

			dlg.AddOption("Show debug info",
						() => { ShowDebugInfo = true; },
						() => { ShowDebugInfo = false; });

			dlg.ShowModal(_desktop);
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			//			_fpsCounter.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			_gcMemoryLabel.Text = string.Format("GC Memory: {0} kb", GC.GetTotalMemory(false) / 1024);
			//			_fpsLabel.Text = string.Format("FPS: {0}", _fpsCounter.FramesPerSecond);
			_widgetsCountLabel.Text = string.Format("Visible Widgets: {0}", _desktop.CalculateTotalWidgets(true));

			GraphicsDevice.Clear(Color.Black);

			_desktop.Bounds = new Rectangle(0, 0,
				GraphicsDevice.PresentationParameters.BackBufferWidth,
				GraphicsDevice.PresentationParameters.BackBufferHeight);
			_desktop.Render();

#if !FNA
			_drawCallsLabel.Text = string.Format("Draw Calls: {0}", GraphicsDevice.Metrics.DrawCount);
#else
			_drawCallsLabel.Text = "Draw Calls: ?";
#endif

			//			_fpsCounter.Draw(gameTime);
		}
	}
}