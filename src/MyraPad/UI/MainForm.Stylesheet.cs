using AssetManagementBase;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System;
using System.Collections;
using System.IO;

namespace MyraPad.UI;

partial class MainForm
{
	private Stylesheet _stylesheetAtStyleTree;
	private readonly TreeView _treeViewStylesheet;
	private readonly TreeView _treeViewStyleExplorer;

	private bool HasCustomStylesheet => Project != null && !string.IsNullOrEmpty(Project.StylesheetPath);

	private void AddStylesheetTab()
	{
		if (!_tabControlLeft.Items.Contains(_tabStylesheet))
		{
			_tabControlLeft.Items.Add(_tabStylesheet);
		}

		RefreshStyles();
	}

	private void RemoveStylesheetTab()
	{
		_tabControlLeft.Items.Remove(_tabStylesheet);
		_tabControlLeft.SelectedIndex = 0;
	}

	private void RefreshStyles()
	{
		if (Project.Stylesheet == _stylesheetAtStyleTree)
		{
			return;
		}

		_treeViewStylesheet.RemoveAllSubNodes();
		if (Project == null || Project.Stylesheet == null)
		{
			_stylesheetAtStyleTree = Project.Stylesheet;
			return;
		}

		var allProperties = typeof(Stylesheet).GetProperties();
		foreach (var property in allProperties)
		{
			if (property.GetMethod == null ||
				!property.GetMethod.IsPublic ||
				property.GetMethod.IsStatic ||
				!typeof(IDictionary).IsAssignableFrom(property.PropertyType) ||
				!property.Name.EndsWith("Styles"))
			{
				continue;
			}

			var dict = (IDictionary)property.GetValue(Project.Stylesheet);
			if (dict.Count == 0)
			{
				continue;
			}

			var subNode = _treeViewStylesheet.AddSubNode(new Label
			{
				Text = property.Name.Substring(0, property.Name.Length - 6)
			});


			subNode.IsExpanded = true;

			foreach (var key in dict.Keys)
			{
				var name = key.ToString();
				var style = dict[key];
				if (name == Stylesheet.DefaultStyleName)
				{
					subNode.Tag = style;
					continue;
				}

				var styleNode = subNode.AddSubNode(new Label
				{
					Text = name
				});

				styleNode.Tag = style;
			}
		}

		_stylesheetAtStyleTree = Project.Stylesheet;
		RefreshStyleExplorer();
	}

	private void AddStylesExplorerRecursive(ITreeViewNode rootNode, object style)
	{
		var newNode = rootNode.AddSubNode(new Label
		{
			Text = style.GetType().Name
		});

		newNode.Tag = style;
		newNode.IsExpanded = true;

		var allProperties = style.GetType().GetProperties();
		foreach (var property in allProperties)
		{
			if (property.GetMethod == null ||
				!property.GetMethod.IsPublic ||
				property.GetMethod.IsStatic ||
				!typeof(WidgetStyle).IsAssignableFrom(property.PropertyType))
			{
				continue;
			}

			var styleValue = property.GetValue(style);
			if (styleValue == null)
			{
				styleValue = Activator.CreateInstance(property.PropertyType);
				property.SetValue(style, styleValue);
			}

			AddStylesExplorerRecursive(newNode, styleValue);
		}
	}

	private void RefreshStyleExplorer()
	{
		_treeViewStyleExplorer.RemoveAllSubNodes();
		if (_treeViewStylesheet.SelectedNode == null)
		{
			return;
		}

		AddStylesExplorerRecursive(_treeViewStyleExplorer, _treeViewStylesheet.SelectedNode.Tag);
	}

	private void _treeViewStylesheet_SelectionChanged(object sender, MyraEventArgs e)
	{
		RefreshStyleExplorer();

		if (_treeViewStyleExplorer.SubNodesCount > 0)
		{
			_treeViewStyleExplorer.SelectedNode = _treeViewStyleExplorer.GetSubNode(0);
		}
	}

	private void _treeViewStyleExplorer_SelectionChanged(object sender, MyraEventArgs e)
	{
		_propertyGrid.Object = _treeViewStyleExplorer.SelectedNode?.Tag;
	}

	// Displays a dialog to load a custom stylesheet file for the project
	private void OnMenuFileLoadStylesheet(object sender, MyraEventArgs e)
	{
		AssetManager.Cache.Clear();

		var dlg = new FileDialog(FileDialogMode.OpenFile)
		{
			Filter = "*.xmms|*.xml"
		};

		try
		{
			if (!string.IsNullOrEmpty(Project.StylesheetPath))
			{
				var stylesheetPath = Project.StylesheetPath;
				if (!Path.IsPathRooted(stylesheetPath))
				{
					// Prepend folder path
					stylesheetPath = Path.Combine(Path.GetDirectoryName(FilePath), stylesheetPath);
				}

				dlg.Folder = Path.GetDirectoryName(stylesheetPath);
			}
			else if (!string.IsNullOrEmpty(FilePath))
			{
				dlg.Folder = Path.GetDirectoryName(FilePath);
			}
		}
		catch (Exception)
		{
		}

		dlg.Closed += (s, a) =>
		{
			if (!dlg.Result)
			{
				return;
			}

			var filePath = dlg.FilePath;

			// Check whether stylesheet could be loaded
			try
			{
				var stylesheet = AssetManager.LoadStylesheet(filePath);
			}
			catch (Exception ex)
			{
				var msg = Dialog.CreateMessageBox("Stylesheet Error", ex.Message);
				msg.ShowModal(Desktop);
				return;
			}

			// Try to make stylesheet path relative to project folder
			filePath = PathUtils.TryToMakePathRelativeTo(filePath, Path.GetDirectoryName(FilePath));

			Project.StylesheetPath = filePath;
			UpdateSource();
			UpdateMenuFile();
		};

		dlg.ShowModal(Desktop);
	}

	// Resets the project to use the default stylesheet
	private void OnMenuFileResetStylesheetSelected(object sender, MyraEventArgs e)
	{
		AssetManager.Cache.Clear();
		Project.StylesheetPath = null;
		UpdateSource();
		UpdateMenuFile();
	}

	private void SaveStylesheet()
	{
		if (!HasCustomStylesheet)
		{
			return;
		}

		var stylesheetPath = Project.StylesheetPath;
		if (!Path.IsPathRooted(stylesheetPath))
		{
			var folder = Path.GetDirectoryName(FilePath);
			stylesheetPath = Path.Combine(folder, stylesheetPath);
		}

		var stylesheetData = Project.Stylesheet.ToXml();
		File.WriteAllText(stylesheetPath, stylesheetData);
	}
}
