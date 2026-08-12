using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using System;

namespace Myra.Samples;

/// <summary>
/// Sample game demonstrating the <see cref="LogView"/> custom widget.
/// Periodically appends colour-coded combat log messages (Gandalf vs. kobold)
/// to show how the log scrolls and trims entries.
/// </summary>
class LogViewGame : Game
{
	private readonly GraphicsDeviceManager _graphics;
	private LogView _logView;
	private DateTime _dt;
	private Desktop _desktop;
	private bool _isPaused = false;

	/// <summary>
	/// Shared <see cref="Random"/> instance for generating randomised log messages.
	/// </summary>
	public Random Random { get; } = new Random();

	public LogViewGame()
	{
		_graphics = new GraphicsDeviceManager(this)
		{
			PreferredBackBufferWidth = 1200,
			PreferredBackBufferHeight = 800
		};
		Window.AllowUserResizing = true;

		IsMouseVisible = true;
	}

	/// <summary>
	/// Builds the UI: a full-screen panel with a pause toggle button at the top-left
	/// and a <see cref="LogView"/> pinned to the bottom with a blue border.
	/// </summary>
	protected override void LoadContent()
	{
		base.LoadContent();

		MyraEnvironment.Game = this;

		_desktop = new Desktop();

		var topPanel = new Panel();
		_desktop.Root = topPanel;

		var pauseButton = ToggleButton.CreateTextButton("Pause");
		pauseButton.Click += (s, a) =>
		{
			_isPaused = pauseButton.IsToggled;
		};
		topPanel.Widgets.Add(pauseButton);

		var logViewPanel = new Panel
		{
			Border = new SolidBrush(Color.Blue),
			BorderThickness = new Thickness(2),
			VerticalAlignment = VerticalAlignment.Bottom,
			Height = 300
		};
		topPanel.Widgets.Add(logViewPanel);

		_logView = new LogView();
		logViewPanel.Widgets.Add(_logView);


		_logView.ClearLog();

		_dt = DateTime.Now;
	}

	/// <summary>
	/// Every second (unless paused), generates 1-3 randomised combat messages with
	/// colour tags and appends them to the log view.
	/// </summary>
	protected override void Update(GameTime gameTime)
	{
		base.Update(gameTime);

		if (_isPaused)
		{
			return;
		}

		var passed = DateTime.Now - _dt;
		if (passed.TotalSeconds < 1.0)
		{
			return;
		}

		var messagesCount = Random.Next(1, 4);

		var damage = Random.Next(1, 10);
		_logView.LogFormat(@"/c[lightBlue]Gandalf/c[white] hits /c[green]a kobold/c[white] with his staff for /c[red]{0}/c[white] damage.", damage);

		if (messagesCount > 1)
		{
			damage = Random.Next(1, 5);
			_logView.LogFormat(@"/c[green]A kobold/c[white] claws /c[lightBlue]Gandalf/c[white] for /c[red]{0}/c[white] damage.", damage);
		}

		if (messagesCount > 2)
		{
			damage = Random.Next(1, 15);
			_logView.LogFormat(@"/c[lightBlue]Gandalf/c[white] heals himself for /c[lightgreen]{0}/c[white] hit points.", damage);
		}


		_dt = DateTime.Now;
	}

	/// <summary>
	/// Clears the screen and renders the Myra desktop.
	/// </summary>
	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);

		GraphicsDevice.Clear(Color.Black);

		_desktop.Render();
	}
}
