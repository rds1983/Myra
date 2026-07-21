using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System;

namespace Myra.Samples;

/// <summary>
/// A custom Myra widget that displays a scrolling log of text messages.
/// Extends <see cref="ScrollViewer"/> with a <see cref="VerticalStackPanel"/> of
/// <see cref="Label"/> widgets. New entries are appended at the bottom and the view
/// smoothly scrolls down over a configurable duration. Excess entries beyond
/// <see cref="MaximumStrings"/> are removed automatically.
/// </summary>
public class LogView : ScrollViewer
{
	private VerticalStackPanel _logStack;
	private DateTime? _logStarted;
	private int _logPosition;

	/// <summary>
	/// Gets or sets the duration in milliseconds over which the view scrolls from
	/// its current position to the bottom after a new log entry is added.
	/// </summary>
	public int LogMoveUpInMs { get; set; } = 300;

	/// <summary>
	/// Gets or sets the maximum number of log entries kept in the view.
	/// Older entries are removed when the count exceeds this value.
	/// </summary>
	public int MaximumStrings { get; set; } = 100;

	/// <summary>
	/// Initialises the log view with a bottom-aligned vertical stack panel as its scrollable content.
	/// </summary>
	public LogView()
	{
		_logStack = new VerticalStackPanel
		{
			VerticalAlignment = VerticalAlignment.Bottom
		};


		Content = _logStack;
	}

	/// <summary>
	/// Appends a text message to the log. The view initiates a smooth scroll animation
	/// toward the bottom and triggers a layout update so the new entry is measured immediately.
	/// </summary>
	/// <param name="message">The text to display. May contain Myra rich-text colour commands (e.g. <c>/c[red]</c>).</param>
	public void Log(string message)
	{
		var oldBounds = _logStack.Bounds;

		// Add to the end
		var textBlock = new Label
		{
			Text = message,
			Wrap = true
		};

		_logStack.Widgets.Add(textBlock);

		// Update sizes of all widgets including LogView
		Desktop.UpdateLayout();

		if (ScrollMaximum.Y == 0)
		{
			// We need to scroll from minus to zero
			var deltaY = oldBounds.Height - _logStack.Bounds.Height;
			ScrollPosition += new Point(0, deltaY);
		}

		// Record the scroll animation start time and position
		_logStarted = DateTime.Now;
		_logPosition = ScrollPosition.Y;
	}

	/// <summary>
	/// Formats and appends a message using <see cref="string.Format(string, object[])"/> syntax.
	/// If the format string is invalid or no arguments are supplied the raw message is logged.
	/// </summary>
	/// <param name="message">A composite format string.</param>
	/// <param name="args">Optional format arguments.</param>
	public void LogFormat(string message, params object[] args)
	{
		string str;
		try
		{
			if (args != null && args.Length > 0)
			{
				str = string.Format(message, args);
			}
			else
			{
				str = message;
			}
		}
		catch (FormatException)
		{
			str = message;
		}

		Log(str);
	}

	/// <summary>
	/// Removes all log entries and resets the scroll position.
	/// </summary>
	public void ClearLog()
	{
		_logStarted = null;
		_logStack.Widgets.Clear();
	}

	/// <summary>
	/// Called each frame during rendering. Once the scroll animation delay has elapsed,
	/// trims excess entries and snaps the scroll position to the bottom.
	/// During the animation window the scroll position is interpolated linearly.
	/// </summary>
	private void ProcessLog()
	{
		if (_logStarted == null)
		{
			return;
		}

		var now = DateTime.Now;
		var elapsed = now - _logStarted.Value;

		if (elapsed.TotalMilliseconds >= LogMoveUpInMs)
		{
			while (_logStack.Widgets.Count > MaximumStrings)
			{
				_logStack.Widgets.RemoveAt(0);
			}

			Desktop.UpdateLayout();

			ScrollPosition = new Point(0, ScrollMaximum.Y);
			_logStarted = null;
			return;
		}

		var y = _logPosition + (int)(elapsed.TotalMilliseconds * (ScrollMaximum.Y - _logPosition) / LogMoveUpInMs);
		ScrollPosition = new Point(0, y);
	}

	/// <inheritdoc/>
	public override void InternalRender(RenderContext context)
	{
		base.InternalRender(context);

		ProcessLog();
	}
}