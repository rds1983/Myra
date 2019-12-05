using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.File;

namespace Myra.Samples.AllWidgets
{
	public partial class AllWidgets
	{
		public AllWidgets()
		{
			BuildUI();

			_menuItemOpenFile.Image = DefaultAssets.UISpritesheet["icon-folder"];
			_menuItemSaveFile.Image = DefaultAssets.UISpritesheet["icon-folder-new"];

			_menuItemOpenFile.Selected += (s, a) => OpenFile();
			_menuItemSaveFile.Selected += (s, a) => SaveFile();
			_menuItemChooseColor.Selected += (s, a) => ChooseColor();
			_menuItemChooseFolder.Selected += (s, a) => ChooseFolder();
			_menuItemQuit.Selected += (s, a) => Quit();

			_buttonOpenFile.Image = DefaultAssets.UISpritesheet["icon-star"];
			_buttonOpenFile.Click += (sender, args) => OpenFile();

			_buttonSaveFile.Image = DefaultAssets.UISpritesheet["icon-star"];
			_buttonSaveFile.Click += (sender, args) => SaveFile();

			_buttonChooseFolder.Image = DefaultAssets.UISpritesheet["icon-star"];
			_buttonChooseFolder.Click += (sender, args) => ChooseFolder();

			_buttonChooseColor.Click += (sender, args) => ChooseColor();

			_imageButton.Image = DefaultAssets.UISpritesheet["icon-star-outline"];
			_imageButton.Click += (sender, args) =>
			{
				var debugWindow = new DebugOptionsDialog();
				debugWindow.ShowModal();
			};

			_menuItemAbout.Selected += (sender, args) =>
			{
				var messageBox = Dialog.CreateMessageBox("AllWidgets", "Myra AllWidgets Sample " + MyraEnvironment.Version);
				messageBox.ShowModal();
			};

			var tree = new Tree
			{
				HasRoot = false,
				GridColumn = 1,
				GridRow = 12,
				GridColumnSpan = 2
			};
			var node1 = tree.AddSubNode("node1");
			var node2 = node1.AddSubNode("node2");
			var node3 = node2.AddSubNode("node3");
			node3.AddSubNode("node4");
			node3.AddSubNode("node5");
			node3.AddSubNode("node7");
			node3.AddSubNode("node8");
			node3.AddSubNode("node9");
			node3.AddSubNode("node10");

			var node4 = node2.AddSubNode("node6");
			node4.AddSubNode("node11");
			node4.AddSubNode("node12");
			node4.AddSubNode("node13");
			node4.AddSubNode("node14");
			node4.AddSubNode("node15");
			node4.AddSubNode("node16");
			node4.AddSubNode("node17");
			node4.AddSubNode("node18");
			_gridRight.Widgets.Add(tree);
		}

		public void OpenFile()
		{
			var fileDialog = new FileDialog(FileDialogMode.OpenFile);
			fileDialog.ShowModal();

			fileDialog.Closed += (s, a) =>
			{
				if (!fileDialog.Result)
				{
					return;
				}

				_textOpenFile.Text = fileDialog.FilePath;
			};
		}

		public void SaveFile()
		{
			var fileDialog = new FileDialog(FileDialogMode.SaveFile);
			fileDialog.ShowModal();

			fileDialog.Closed += (s, a) =>
			{
				if (!fileDialog.Result)
				{
					return;
				}

				_textSaveFile.Text = fileDialog.FilePath;
			};
		}

		public void ChooseFolder()
		{
			var fileDialog = new FileDialog(FileDialogMode.ChooseFolder);
			fileDialog.ShowModal();

			fileDialog.Closed += (s, a) =>
			{
				if (!fileDialog.Result)
				{
					return;
				}

				_textChooseFolder.Text = fileDialog.FilePath;
			};
		}

		public void ChooseColor()
		{
			var debugWindow = new ColorPickerDialog();
			debugWindow.ShowModal();

			debugWindow.Closed += (s, a) =>
			{
				if (!debugWindow.Result)
				{
					return;
				}

				_textButtonLabel.TextColor = debugWindow.Color;
			};
		}

		public void Quit()
		{
			AllWidgetsGame.Instance.Exit();
		}
	}
}