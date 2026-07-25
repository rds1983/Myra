using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Properties;
using System;
using System.Collections.Generic;
using System.Xml;

namespace MyraPad.UI;

partial class MainForm
{
	// Whether the current XML tag at cursor position needs a closing tag
	private bool _needsCloseTag;
	// Name of the parent XML tag at the current cursor position
	private string _parentTag;
	// Start and end positions of the current XML tag being edited
	private int? _currentTagStart, _currentTagEnd;
	private bool _suppressProjectRefresh = false;

	/// <summary>
	/// The complete XML opening tag string at the current cursor position
	/// </summary>
	private string CurrentTag
	{
		get
		{
			if (_currentTagStart == null || _currentTagEnd == null || _currentTagEnd.Value <= _currentTagStart.Value)
			{
				return null;
			}

			return _textSource.Text.Substring(_currentTagStart.Value, _currentTagEnd.Value - _currentTagStart.Value + 1);
		}
	}

	private void LoadObject(string objectXml)
	{
		var newObject = Project.LoadObjectFromXml(objectXml, AssetManager, Project.Stylesheet);
		if (newObject == null)
		{
			return;
		}

		PropertyGrid.ParentType = ParentType;
		PropertyGrid.Object = newObject;

		// Automatically select the corresponding node in the explorer tree
		try
		{
			_suppressExplorerRefresh = true;

			// Find the node by matching line/column position in the XML
			object selectedItem = null;
			foreach (var pair in Project.ObjectsNodes)
			{
				var lineInfo = (IXmlLineInfo)pair.Item2;
				if (lineInfo.LineNumber - 1 > _line ||
					(lineInfo.LineNumber - 1 == _line && lineInfo.LinePosition - 1 > _col))
				{
					break;
				}

				selectedItem = pair.Item1;
			}

			if (selectedItem != null)
			{
				var node = _treeViewExplorer.FindNode(n => n.Tag == selectedItem);
				_treeViewExplorer.SelectedNode = node;
			}
		}
		finally
		{
			_suppressExplorerRefresh = false;
		}

		_propertyGridPane.ResetScroll();
	}

	/// <summary>
	/// Updates cursor position state including line/column info, parent tag, and current tag bounds
	/// </summary>
	private void UpdatePositions()
	{
		var lastStart = _currentTagStart;
		var lastEnd = _currentTagEnd;

		// Reset all position tracking variables
		_line = _col = _indentLevel = 0;
		_parentTag = null;
		_currentTagStart = null;
		_currentTagEnd = null;
		_needsCloseTag = false;

		if (string.IsNullOrEmpty(_textSource.Text))
		{
			return;
		}

		var cursorPos = _textSource.CursorPosition;
		var text = _textSource.Text;

		int? tagOpen = null;
		var isOpenTag = true;
		var length = text.Length;

		string currentTag = null;
		// Stack to track nested tags and their nesting level
		Stack<string> parentStack = new Stack<string>();

		// Parse the XML character by character up to the cursor position
		for (var i = 0; i < length; ++i)
		{
			// Stop parsing if we're past the cursor and not in an open tag
			if (tagOpen == null)
			{
				if (i >= cursorPos)
				{
					break;
				}

				currentTag = null;
				_currentTagStart = null;
				_currentTagEnd = null;
			}

			// Count columns before the cursor
			if (i < cursorPos)
			{
				++_col;
			}

			var c = text[i];
			if (c == '\n')
			{
				// Track line breaks and reset column counter
				++_line;
				_col = 0;
			}

			// Handle opening bracket: start of a tag
			if (c == '<')
			{
				// Check if we have an unclosed tag after the cursor
				if (tagOpen != null && isOpenTag && i >= cursorPos + 1)
				{
					_currentTagStart = tagOpen;
					_currentTagEnd = null;
					break;
				}

				// Start tracking a tag (skip XML declarations like <?xml>)
				if (i < length - 1 && text[i + 1] != '?')
				{
					tagOpen = i;
					isOpenTag = text[i + 1] != '/';
				}
			}

			// Handle closing bracket: end of a tag
			if (tagOpen != null && i > tagOpen.Value && c == '>')
			{
				if (isOpenTag)
				{
					// Check if this tag is self-closing (ends with />)
					var needsCloseTag = text[i - 1] != '/';
					_needsCloseTag = needsCloseTag;

					currentTag = text.Substring(tagOpen.Value, i - tagOpen.Value + 1);
					_currentTagStart = tagOpen;
					_currentTagEnd = i;

					// Add to parent stack if this is an opening tag before the cursor
					if (needsCloseTag && i <= cursorPos)
					{
						parentStack.Push(currentTag);
					}
				}
				else
				{
					// Closing tag: pop from parent stack
					if (parentStack.Count > 0)
					{
						parentStack.Pop();
					}
				}

				tagOpen = null;
			}
		}

		// The indent level is determined by the nesting depth
		_indentLevel = parentStack.Count;
		if (parentStack.Count > 0)
		{
			_parentTag = parentStack.Pop();
		}

		// Update the status bar with position information
		_textLocation.Text = string.Format("Line: {0}, Col: {1}, Indent: {2}", _line + 1, _col + 1, _indentLevel);

		if (!string.IsNullOrEmpty(_parentTag))
		{
			_parentTag = ExtractTag(_parentTag);
			_textLocation.Text += ", Parent: " + _parentTag;
		}

		// If the current tag changed, load its widget object in the property grid
		if ((lastStart != _currentTagStart || lastEnd != _currentTagEnd))
		{
			PropertyGrid.Object = null;
			_propertyGridPane.ResetScroll();
			if (!string.IsNullOrEmpty(currentTag))
			{
				var xml = currentTag;

				// Add the closing tag for complete XML
				if (_needsCloseTag)
				{
					var tag = ExtractTag(currentTag);
					xml += "</" + tag + ">";
				}

				LoadObject(xml);
			}
		}

		HandleAutoComplete();
	}

	private void OnObjectPropertyChanged()
	{
		// Project property changed
		// Serialize the modified widget object back to XML
		var xml = _project.SaveObjectToXml(PropertyGrid.Object, ExtractTag(CurrentTag), ParentType);

		// If the original tag needs a closing tag, ensure the new XML has one too
		if (_needsCloseTag)
		{
			xml = xml.Replace("/>", ">");
		}

		// Replace the old XML tag with the new serialized XML
		if (_currentTagStart != null && _currentTagEnd != null)
		{
			try
			{
				_suppressProjectRefresh = true;

				// Replace the current tag with the updated XML
				_textSource.Replace(_currentTagStart.Value,
					_currentTagEnd.Value - _currentTagStart.Value + 1,
					xml);
				QueueRefreshProject();
			}
			finally
			{
				_suppressProjectRefresh = false;
			}

			// Update the end position of the current tag after replacement
			_currentTagEnd = _currentTagStart.Value + xml.Length - 1;
		}
	}

	// Updates cursor-related state and applies auto-indent/auto-close features
	private void UpdateCursor()
	{
		try
		{
			UpdatePositions();
			ApplyAutoIndent();
			ApplyAutoClose();
		}
		catch (Exception)
		{
		}
	}

	// Handles cursor position changes in the text editor
	private void _textSource_CursorPositionChanged(object sender, MyraEventArgs e)
	{
		UpdateCursor();
	}

	/// <summary>
	/// Handles text changes in the XML editor; marks as dirty and queues project refresh
	/// </summary>
	private void _textSource_TextChanged(object sender, ValueChangedEventArgs<string> e)
	{
		try
		{
			IsDirty = true;

			// Skip refresh if suppressed (e.g., during programmatic text updates)
			if (_suppressProjectRefresh)
			{
				return;
			}

			UpdateCursor();

			// Decide whether to refresh immediately or after a short delay
			var newLength = string.IsNullOrEmpty(e.NewValue) ? 0 : e.NewValue.Length;
			var oldLength = string.IsNullOrEmpty(e.OldValue) ? 0 : e.OldValue.Length;

			// Large changes or auto-close actions should refresh immediately to keep preview in sync
			if (Math.Abs(newLength - oldLength) > 1 || _applyAutoClose)
			{
				QueueRefreshProject();
			}
			else
			{
				// Small changes (single character edits) are queued after a short delay to batch multiple edits
				_refreshInitiated = DateTime.Now;
			}
		}
		catch (Exception)
		{
		}
	}
}
