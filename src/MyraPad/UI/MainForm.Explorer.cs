using Microsoft.Xna.Framework.Input;
using Myra.Attributes;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;
using Myra.MML;
using Myra.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace MyraPad.UI;

partial class MainForm
{
	// Tree view widget for displaying the widget hierarchy in the explorer panel
	private readonly TreeView _treeViewExplorer;
	private bool _suppressExplorerRefresh = false;

	// The index in the explorer tree of the node to select after project refresh
	public int? NewProjectSelectedNodeIndex { get; set; }

	/// <summary>
	/// Creates a new widget of the specified type and adds it to the parent container
	/// </summary>
	private void DefaultCreate(object parent, Type t)
	{
		try
		{
			IItemWithId child;

			// Try to instantiate the widget using either parameterless or stylename constructor
			var constructor = t.GetConstructor(Type.EmptyTypes);
			if (constructor != null)
			{
				child = (IItemWithId)Activator.CreateInstance(t);
			}
			else
			{
				// Fallback to constructor that takes style name
				child = (IItemWithId)Activator.CreateInstance(t, Stylesheet.DefaultStyleName);
			}

			// Add the new widget to the parent using the appropriate collection/property
			do
			{
				var asContentControl = parent as IContent;
				if (asContentControl != null)
				{
					asContentControl.Content = (Widget)child;
					break;
				}

				var asContainer = parent as IContainer;
				if (asContainer != null)
				{
					asContainer.Widgets.Add((Widget)child);
					break;
				}

				var asMenu = parent as Menu;
				if (asMenu != null)
				{
					asMenu.Items.Add((IMenuItem)child);
					break;
				}

				var asTabControl = parent as TabControl;
				if (asTabControl != null)
				{
					asTabControl.Items.Add((TabItem)child);
					break;
				}
			}
			while (false);

			// Refresh the explorer tree to show the new widget
			RefreshExplorer();

			// Find and schedule selection of the new item in the explorer tree
			for (var i = 0; i < _treeViewExplorer.TotalNodesCount; ++i)
			{
				var node = _treeViewExplorer.GetNodeByAbsoluteIndex(i);
				if (node.Tag == child)
				{
					NewProjectSelectedNodeIndex = i;
					break;
				}
			}

			// Synchronize the text editor with the updated project structure
			_textSource.Text = _project.ToXml();
		}
		catch (Exception ex)
		{
			var msg = Dialog.CreateMessageBox("Error", ex.Message);
			msg.ShowModal(Desktop);
		}
	}

	private ChildCreator CreateNewItemAction(Widget parent, Type childType) => new ChildCreator(childType.Name, () => DefaultCreate(parent, childType));

	private ChildCreator[] CreateNewItemActions(Widget parent, IEnumerable<Type> childTypes)
	{
		var result = new List<ChildCreator>();

		foreach (var childType in childTypes)
		{
			result.Add(CreateNewItemAction(parent, childType));
		}

		return result.ToArray();
	}

	/// <summary>
	/// Builds a list of available child widget types that can be added to the specified parent widget
	/// </summary>
	private List<ChildCreator> BuildAddActions(Widget parent)
	{
		var result = new List<ChildCreator>();
		if (parent == null)
		{
			return result;
		}

		var widgetTypeName = parent.GetType().Name;

		// Add different widget types based on the parent's capabilities
		if (Containers.Contains(widgetTypeName) || widgetTypeName == "Window" || widgetTypeName == "Dialog")
		{
			// Containers can hold any type of child widget
			result.AddRange(CreateNewItemActions(parent, SimpleWidgets));
			result.AddRange(CreateNewItemActions(parent, Containers));
			result.AddRange(CreateNewItemActions(parent, SpecialContainers));
		}
		else if (widgetTypeName.EndsWith("Menu"))
		{
			// Menus can only contain menu items and separators
			result.Add(CreateNewItemAction(parent, typeof(MenuItem)));
			result.Add(CreateNewItemAction(parent, typeof(MenuSeparator)));
		}
		else if (widgetTypeName == "TabControl")
		{
			// TabControl can only contain TabItems
			result.Add(CreateNewItemAction(parent, typeof(TabItem)));
		}

		// Sort the results alphabetically by name
		result = result.OrderBy(s => s.Name).ToList();

		return result;
	}

	// Records whether the mouse down event was a right-click
	private void _treeViewExplorer_TouchDown(object sender, MyraEventArgs e)
	{
		var state = Mouse.GetState();

		_rightClick = state.RightButton == ButtonState.Pressed;
	}

	/// <summary>
	/// Handles right-click on the explorer tree to show a context menu for adding new widgets
	/// </summary>
	private void _treeViewExplorer_TouchUp(object sender, MyraEventArgs e)
	{
		if (!_rightClick || Desktop.ContextMenu != null)
		{
			// Don't show if a menu is already displayed
			return;
		}

		try
		{
			var selectedWidget = (Widget)_treeViewExplorer.SelectedNode.Tag;

			// Get the list of widgets that can be added to this widget
			var addActions = BuildAddActions(selectedWidget);
			if (addActions.Count == 0)
			{
				return;
			}

			var asContent = selectedWidget as IContent;

			var verticalMenu = new VerticalMenu();
			// If there are few options, show them directly in the context menu
			if (addActions.Count < 5)
			{
				var prefix = "Add ";
				if (asContent != null && asContent.Content != null)
				{
					prefix = "Replace Content With ";
				}

				// Add each action directly as a menu item
				foreach (var addAction in addActions)
				{
					var menuItem = new MenuItem
					{
						Text = prefix + addAction.Name
					};

					menuItem.Selected += (s, a) => addAction.Creator();
					verticalMenu.Items.Add(menuItem);
				}
			}
			else
			{
				// If there are many options, show a dialog to search for the desired widget
				var prefix = "Add New Widget";

				if (asContent != null && asContent.Content != null)
				{
					prefix = "Replace Content With New Widget";
				}
				var menuItem = new MenuItem
				{
					Text = prefix + "..."
				};

				menuItem.Selected += (sender, args) =>
				{
					// Display a searchable dialog for selecting the widget type
					var addNewWidgetDialog = new AddNewWidgetDialog();
					addNewWidgetDialog.Title = prefix;

					addNewWidgetDialog.SetNames((from a in addActions select a.Name).ToArray());

					addNewWidgetDialog.Closed += (s, a) =>
					{
						if (!addNewWidgetDialog.Result)
						{
							// Dialog was cancelled
							return;
						}

						// User confirmed a selection
						var addAction = addActions[addNewWidgetDialog.SelectedIndex];
						addAction.Creator();
					};

					addNewWidgetDialog.ShowModal(Desktop);
				};

				verticalMenu.Items.Add(menuItem);
			}

			Desktop.ShowContextMenu(verticalMenu, Desktop.MousePosition);
		}
		catch (Exception ex)
		{
			var msg = Dialog.CreateMessageBox("Error", ex.Message);
			msg.ShowModal(Desktop);
		}
	}

	// Recursively builds the visual tree hierarchy in the explorer from a widget object and its children
	private void BuildExplorerTreeRecursive(ITreeViewNode node, IItemWithId root)
	{
		if (root == null)
		{
			return;
		}

		// Create a label showing the widget type and its ID (if set)
		var id = root.GetType().Name;
		if (!string.IsNullOrEmpty(root.Id))
		{
			id += $" (#{root.Id})";
		}

		var newNode = node.AddSubNode(new Label
		{
			Text = id
		});

		// Store the widget object as the node's tag for later reference
		newNode.Tag = root;
		newNode.IsExpanded = true;

		// Find the content property that holds child widgets
		var props = root.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		var contentProperty = (from p in props where p.FindAttribute<ContentAttribute>() != null select p).FirstOrDefault();
		if (contentProperty == null)
		{
			return;
		}

		var content = contentProperty.GetValue(root);

		// Recursively add child widgets (either a single widget or a list of widgets)
		var asList = content as IList;
		if (asList != null)
		{
			// Multiple children (Widgets collection)
			foreach (IItemWithId child in asList)
			{
				BuildExplorerTreeRecursive(newNode, child);
			}
		}
		else
		{
			// Single child widget
			BuildExplorerTreeRecursive(newNode, (IItemWithId)content);
		}
	}

	// Rebuilds the explorer tree from the project hierarchy while preserving the current selection
	private void RefreshExplorer()
	{
		// Save the currently selected node so we can restore it after rebuild
		int? selectedIndex = null;
		if (_treeViewExplorer.SelectedNode != null)
		{
			selectedIndex = _treeViewExplorer.AllNodes.IndexOf(_treeViewExplorer.SelectedNode);
		}

		// Clear all existing nodes from the tree
		_treeViewExplorer.RemoveAllSubNodes();

		if (Project == null || Project.Root == null)
		{
			return;
		}

		// Rebuild the tree structure from the project hierarchy
		BuildExplorerTreeRecursive(_treeViewExplorer, Project.Root);

		// Restore the selection if it still exists
		if (selectedIndex != null && selectedIndex.Value < _treeViewExplorer.AllNodes.Count)
		{
			try
			{
				_suppressExplorerRefresh = true;

				_treeViewExplorer.SelectedNode = _treeViewExplorer.AllNodes[selectedIndex.Value];
			}
			finally
			{
				_suppressExplorerRefresh = false;
			}
		}
	}

	// Handles explorer tree node selection by moving the cursor to the corresponding position in the XML editor
	private void _treeViewExplorer_SelectionChanged(object sender, MyraEventArgs e)
	{
		// Don't respond to selection changes made by programmatic updates
		if (_suppressExplorerRefresh || _treeViewExplorer.SelectedNode == null || Project.ObjectsNodes == null)
		{
			return;
		}

		// Find the XML element corresponding to the selected tree node
		Tuple<object, XElement> find = null;
		foreach (var pair in Project.ObjectsNodes)
		{
			if (pair.Item1 == _treeViewExplorer.SelectedNode.Tag)
			{
				find = pair;
				break;
			}
		}

		if (find == null)
		{
			return;
		}

		var lineInfo = (IXmlLineInfo)find.Item2;

		// Calculate the text position corresponding to the XML element's line and column
		var currentLineNumber = 0;
		var currentLinePosition = 0;
		for (var pos = 0; pos < _textSource.Text.Length; ++pos)
		{
			// Check if we've reached the target line and column
			if (currentLineNumber > lineInfo.LineNumber - 1 ||
				(currentLineNumber == lineInfo.LineNumber - 1 && currentLinePosition >= lineInfo.LinePosition - 1))
			{
				// Move cursor to this position and focus the text editor
				_textSource.CursorPosition = pos;
				Desktop.FocusedKeyboardWidget = _textSource;
				break;
			}

			var c = _textSource.Text[pos];
			switch (c)
			{
				case '\n':
					// Track line breaks
					++currentLineNumber;
					currentLinePosition = 0;
					break;

				case '\r':
					// Ignore carriage returns
					break;

				default:
					// Track character position in the line
					++currentLinePosition;
					break;
			}
		}
	}
}
