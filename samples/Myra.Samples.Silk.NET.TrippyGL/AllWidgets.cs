using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.File;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Samples.AllWidgets
{
	public partial class AllWidgets
	{
		public AllWidgets()
		{
			BuildUI();

			_menuItemOpenFile.Image = Stylesheet.Current.Atlas["icon-folder"];
			_menuItemSaveFile.Image = Stylesheet.Current.Atlas["icon-folder-new"];

			_menuItemOpenFile.Selected += (s, a) => OpenFile();
			_menuItemSaveFile.Selected += (s, a) => SaveFile();
			_menuItemChooseColor.Selected += (s, a) => ChooseColor();
			_menuItemChooseFolder.Selected += (s, a) => ChooseFolder();
			_menuItemQuit.Selected += (s, a) => Quit();

			_buttonOpenFile.Image = Stylesheet.Current.Atlas["icon-star"];
			_buttonOpenFile.Click += (sender, args) => OpenFile();

			_buttonSaveFile.Image = Stylesheet.Current.Atlas["icon-star"];
			_buttonSaveFile.Click += (sender, args) => SaveFile();

			_buttonChooseFolder.Image = Stylesheet.Current.Atlas["icon-star"];
			_buttonChooseFolder.Click += (sender, args) => ChooseFolder();

			_buttonChooseColor.Click += (sender, args) => ChooseColor();

			_imageButton.Image = Stylesheet.Current.Atlas["icon-star-outline"];
			_imageButton.Click += (sender, args) =>
			{
				var debugWindow = new DebugOptionsWindow();
				debugWindow.ShowModal(Desktop);
			};

			_menuItemAbout.Selected += (sender, args) =>
			{
				var messageBox = Dialog.CreateMessageBox("AllWidgets", "Myra AllWidgets Sample " + MyraEnvironment.Version);
				messageBox.ShowModal(Desktop);
			};

			var tree = new Tree
			{
				HasRoot = false,
			};
			Grid.SetColumn(tree, 1);
			Grid.SetRow(tree, 12);
			Grid.SetColumnSpan(tree, 2);
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
			fileDialog.ShowModal(Desktop);

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
			fileDialog.ShowModal(Desktop);

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
			fileDialog.ShowModal(Desktop);

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
			var colorWindow = new ColorPickerDialog();
			colorWindow.Color = _textButtonLabel.TextColor;
			colorWindow.ShowModal(Desktop);

			colorWindow.Closed += (s, a) =>
			{
				if (!colorWindow.Result)
				{
					return;
				}

				_textButtonLabel.TextColor = colorWindow.Color;
			};
		}

		public void Quit()
		{
//			AllWidgetsGame.Instance.Exit();
		}
	}
}