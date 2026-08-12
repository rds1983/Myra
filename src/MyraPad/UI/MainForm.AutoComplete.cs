using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace MyraPad.UI;

partial class MainForm
{
	// Flags to apply auto-indent when Enter is pressed and auto-close when > is typed
	private bool _applyAutoIndent = false;
	private bool _applyAutoClose = false;

	/// <summary>
	/// Sets the flag to apply auto-close when the user types the > character
	/// </summary>
	private void _textSource_Char(object sender, GenericEventArgs<char> e)
	{
		_applyAutoClose = e.Data == '>';
	}

	/// <summary>
	/// Sets the flag to apply auto-indent when the user presses Enter
	/// </summary>
	private void _textSource_KeyDown(object sender, GenericEventArgs<Keys> e)
	{
		_applyAutoIndent = e.Data == Keys.Enter;
	}

	/// <summary>
	/// Automatically indents the next line after pressing Enter based on the XML nesting level
	/// </summary>
	private void ApplyAutoIndent()
	{
		if (!Options.AutoIndent || Options.IndentSpacesSize <= 0 || !_applyAutoIndent)
		{
			return;
		}

		_applyAutoIndent = false;

		var text = _textSource.Text;
		var pos = _textSource.CursorPosition;

		if (string.IsNullOrEmpty(text) || pos == 0 || pos >= text.Length)
		{
			return;
		}

		var indentLevel = _indentLevel;
		// Check if a closing tag immediately follows the cursor
		bool wrapAfterIndent = text.SubstringSafely(pos + 1, 2) == "</";

		if (indentLevel <= 0)
		{
			return;
		}

		// Build the indent string based on the nesting level
		var indent = new string(' ', indentLevel * Options.IndentSpacesSize);
		// If a closing tag follows, add a newline after the indent
		if (wrapAfterIndent)
		{
			indent += '\n';
		}
		_textSource.Insert(pos + 1, indent);

		// Move cursor after the indent
		_textSource.CursorPosition = pos + 2;
	}

	/// <summary>
	/// Automatically adds a closing tag when typing > after an opening tag name
	/// </summary>
	private void ApplyAutoClose()
	{
		if (!Options.AutoClose || !_applyAutoClose)
		{
			return;
		}

		_applyAutoClose = false;

		var pos = _textSource.CursorPosition;
		var currentTag = CurrentTag;

		// Only auto-close non-self-closing tags
		if (string.IsNullOrEmpty(currentTag) || !_needsCloseTag)
		{
			return;
		}

		// Extract the tag name and build the closing tag
		var closeTag = "</" + ExtractTag(currentTag) + ">";
		_textSource.Insert(pos + 1, closeTag);

		// Position cursor between opening and closing tags
		_textSource.CursorPosition = pos;
	}

	/// <summary>
	/// Displays an auto-complete context menu with available widget types based on the parent tag and typed text
	/// </summary>
	private void HandleAutoComplete()
	{
		// Hide existing auto-complete menu if it's open
		if (Desktop.ContextMenu == _autoCompleteMenu)
		{
			Desktop.HideContextMenu();
		}

		// Only show auto-complete when we're inside an incomplete tag in a valid parent
		if (_currentTagStart == null || _currentTagEnd != null || string.IsNullOrEmpty(_parentTag))
		{
			return;
		}

		var cursorPos = _textSource.CursorPosition;
		var text = _textSource.Text;

		// Extract what the user has typed after the opening bracket
		var typed = text.Substring(_currentTagStart.Value, cursorPos - _currentTagStart.Value);
		if (typed.StartsWith("<"))
		{
			typed = typed.Substring(1);

			// Get all available widget types for this parent
			var all = BuildAutoCompleteVariants();

			// Filter to only show matches for what's been typed so far
			if (!string.IsNullOrEmpty(typed))
			{
				all = (from a in all where a.StartsWith(typed, StringComparison.OrdinalIgnoreCase) select a).ToList();
			}

			if (all.Count > 0)
			{
				var lastStartPos = _currentTagStart.Value;
				var lastEndPos = cursorPos;

				// Build the auto-complete menu with all matching types
				_autoCompleteMenu = new VerticalMenu();
				foreach (var a in all)
				{
					var menuItem = new MenuItem
					{
						Text = a
					};

					menuItem.Selected += (s, args) =>
					{
						var result = "<" + menuItem.Text;
						var skip = result.Length;

						// Simple widgets and proportions are self-closing
						if (SimpleWidgets.Contains(menuItem.Text) ||
							Project.IsProportionName(menuItem.Text) ||
							menuItem.Text == MenuItemName ||
							menuItem.Text == ListItemName)
						{
							result += "/>";
							skip += 2;
						}
						else
						{
							// Container widgets need closing tags
							result += ">";
							++skip;

							// Add formatted indentation if auto-indent is enabled
							if (Options.AutoIndent && Options.IndentSpacesSize > 0)
							{
								result += "\n";
								var indentSize = Options.IndentSpacesSize * (_indentLevel + 1);
								result += new string(' ', indentSize);
								skip += indentSize;

								// Add indentation for closing tag
								result += "\n";
								indentSize = Options.IndentSpacesSize * _indentLevel;
								result += new string(' ', indentSize);
							}
							result += "</" + menuItem.Text + ">";
							++skip;
						}

						// Replace the typed text with the completed widget tag
						_textSource.Replace(lastStartPos, lastEndPos - lastStartPos, result);
						_textSource.CursorPosition = lastStartPos + skip;
					};

					_autoCompleteMenu.Items.Add(menuItem);
				}

				// Show menu at the cursor position
				var screen = _textSource.ToGlobal(_textSource.CursorCoords);
				screen.Y += _textSource.Font.LineHeight;

				if (_autoCompleteMenu.Items.Count > 0)
				{
					_autoCompleteMenu.HoverIndex = 0;
				}

				Desktop.ShowContextMenu(_autoCompleteMenu, screen);
				// Keep focus in the text editor
				Desktop.FocusedKeyboardWidget = _textSource;

				_refreshInitiated = null;
			}
		}
	}

	/// <summary>
	/// Builds a list of valid child widget types for auto-complete based on the parent container type
	/// </summary>
	private List<string> BuildAutoCompleteVariants()
	{
		var result = new List<string>();

		if (string.IsNullOrEmpty(_parentTag))
		{
			return result;
		}

		// Add available child types based on parent widget type
		if (_parentTag == "Project")
		{
			// Project can only contain top-level containers
			result.AddRange(Containers.ToStringList());
			result.Add("Window");
			result.Add("Dialog");
		}
		else if (Containers.Contains(_parentTag) || _parentTag == "Window" || _parentTag == "Dialog")
		{
			// General containers can hold any widget type
			result.AddRange(SimpleWidgets.ToStringList());
			result.AddRange(Containers.ToStringList());
			result.AddRange(SpecialContainers.ToStringList());
		}
		else if (_parentTag.EndsWith(RowsProportionsName) || _parentTag.EndsWith(ColumnsProportionsName) || _parentTag.EndsWith(ProportionsName))
		{
			// Proportion containers can only hold proportion definitions
			result.Add(Project.ProportionName);
		}
		else if (_parentTag.EndsWith("Menu"))
		{
			// Menus can only contain menu items
			result.Add("MenuItem");
			result.Add("MenuSeparator");
		}
		else if (_parentTag == "ListBox" || _parentTag == "ComboBox")
		{
			// List containers can only contain list items
			result.Add("ListItem");
		}
		else if (_parentTag == "TabControl")
		{
			// TabControl can only contain TabItems
			result.Add("TabItem");
		}

		// Add proportion definitions for specific container types
		if (_parentTag == "Grid")
		{
			result.Add(_parentTag + "." + ColumnsProportionsName);
			result.Add(_parentTag + "." + RowsProportionsName);
			result.Add(_parentTag + "." + Project.DefaultColumnProportionName);
			result.Add(_parentTag + "." + Project.DefaultRowProportionName);
		}

		if (_parentTag == "VerticalStackPanel" || _parentTag == "HorizontalStackPanel")
		{
			result.Add(_parentTag + "." + Project.DefaultProportionName);
		}

		// Sort: non-nested elements first, then alphabetically
		result = result.OrderBy(s => !s.Contains('.')).ThenBy(s => s).ToList();

		return result;
	}
}
