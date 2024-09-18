using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;
using MyraPad.UI;
using Myra;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace MyraPad
{
	public class Studio : Game
	{
		private static Studio _instance;

		private readonly GraphicsDeviceManager _graphicsDeviceManager;
		private readonly State _state;
		private Desktop _desktop;
		private StudioWidget _ui;

		public static Studio Instance => _instance;

		public static StudioWidget MainForm => Instance._ui;

		public Project Project => _ui.Project;

		public string FilePath => _ui.FilePath;

		public Studio(string[] args)
		{
			_instance = this;

			// Restore state
			_state = State.Load();

			//Load via program argument
			if (args.Length > 0)
			{
				string filePathArg = args[0];
				if (!string.IsNullOrEmpty(filePathArg))
				{
					_state.EditedFile = filePathArg;
					_state.LastFolder = Path.GetDirectoryName(filePathArg);
				}
			}

			_graphicsDeviceManager = new GraphicsDeviceManager(this);

			if (_state != null)
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = _state.Size.X;
				_graphicsDeviceManager.PreferredBackBufferHeight = _state.Size.Y;

				if (_state.UserColors != null)
				{
					for (var i = 0; i < Math.Min(ColorPickerPanel.UserColors.Length, _state.UserColors.Length); ++i)
					{
						ColorPickerPanel.UserColors[i] = _state.UserColors[i];
					}
				}
			}
			else
			{
				_graphicsDeviceManager.PreferredBackBufferWidth = 1280;
				_graphicsDeviceManager.PreferredBackBufferHeight = 800;
			}
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
			MyraEnvironment.EnableModalDarkening = true;

			_desktop = new Desktop();

			_ui = new StudioWidget(_state);
			_desktop.Root = _ui;

			if (_state != null && !string.IsNullOrEmpty(_state.EditedFile) && File.Exists(_state.EditedFile))
			{
				_ui.Load(_state.EditedFile);
			}

#if MONOGAME

			// Inform Myra that external text input is available
			// So it stops translating Keys to chars
			_desktop.HasExternalTextInput = true;

			// Provide that text input
			Window.TextInput += (s, a) =>
			{
				_desktop.OnChar(a.Character);
			};

#endif
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			_ui.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			GraphicsDevice.Clear(Color.Black);

			_desktop.Render();
		}

		protected override void EndRun()
		{
			base.EndRun();

			_ui.Quit();

			var state = new State
			{
				Size = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth,
					GraphicsDevice.PresentationParameters.BackBufferHeight),
				TopSplitterPosition1 = _ui._topSplitPane.GetSplitterPosition(0),
				TopSplitterPosition2 = _ui._topSplitPane.GetSplitterPosition(1),
				CenterSplitterPosition = _ui._leftSplitPane.GetSplitterPosition(0),
				EditedFile = FilePath,
				LastFolder = _ui.LastFolder,
				UserColors = (from c in ColorPickerPanel.UserColors select c).ToArray()
			};

			state.Save();
		}
	}
}