using System;
using System.IO;
using System.Windows.Forms;
using Cyotek.Drawing.BitmapFont;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment;
using Menu = Myra.Graphics2D.UI.Menu;
using MenuItem = Myra.Graphics2D.UI.MenuItem;
using Orientation = Myra.Graphics2D.UI.Orientation;

namespace Myra.Samples
{
	public class Notepad: SampleGame
	{
		private string _filePath;
		private bool _dirty = true;
		private Desktop _host;
		private TextField _textField;

		public string FilePath
		{
			get
			{
				return _filePath;
			}

			set
			{
				if (value == _filePath)
				{
					return;
				}

				_filePath = value;

				UpdateTitle();
			}
		}

		public bool Dirty
		{
			get
			{
				return _dirty;
			}

			set
			{
				if (value == _dirty)
				{
					return;
				}

				_dirty = value;
				UpdateTitle();
			}
		}

		protected override void LoadContent()
		{
			base.LoadContent();

			UpdateTitle();

			_host = new Desktop();

			var root = new Grid();
			root.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Auto));
			root.RowsProportions.Add(new Grid.Proportion(Grid.ProportionType.Fill, 1.0f));

			var menuBar = new Menu(Orientation.Horizontal);

			// File
			var fileMenu = new MenuItem
			{
				Text = "File"
			};

			// File/New
			var newItem = new MenuItem
			{
				Text = "New"
			};
			newItem.Down += NewItemOnDown;
			fileMenu.AddMenuItem(newItem);

			// File/Open
			var openItem = new MenuItem
			{
				Text = "Open"
			};
			openItem.Down += OpenItemOnDown;
			fileMenu.AddMenuItem(openItem);

			// File/Save
			var saveItem = new MenuItem
			{
				Text = "Save"
			};
			saveItem.Down += SaveItemOnDown;
			fileMenu.AddMenuItem(saveItem);

			// File/Save As...
			var saveAsItem = new MenuItem
			{
				Text = "Save As..."
			};
			saveAsItem.Down += SaveAsItemOnDown;
			fileMenu.AddMenuItem(saveAsItem);

			fileMenu.AddSeparator();

			// File/Quit
			var quitItem = new MenuItem
			{
				Text = "Quit"
			};
			quitItem.Down += QuitItemOnDown;
			fileMenu.AddMenuItem(quitItem);

			menuBar.AddMenuItem(fileMenu);

			var helpMenu = new MenuItem
			{
				Text = "Help"
			};

			var aboutItem = new MenuItem
			{
				Text = "About"
			};
			aboutItem.Down += AboutItemOnDown;
			helpMenu.AddMenuItem(aboutItem);

			menuBar.AddMenuItem(helpMenu);

			root.Children.Add(menuBar);

			_textField = new TextField
			{
				Text =
					"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum!",
				VerticalSpacing = 0,
				TextColor = Color.White,
				Wrap = true,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			_textField.TextChanged += TextFieldOnTextChanged;

			var pane = new ScrollPane<TextField>
			{
				Widget = _textField,
				GridPosition = {Y = 1}
			};

			root.Children.Add(pane);

			_host.Widgets.Add(root);
		}

		private void UpdateTitle()
		{
			Window.Title = CalculateTitle();
		}

		private string CalculateTitle()
		{
			if (string.IsNullOrEmpty(_filePath))
			{
				return "Notepad";
			}

			if (!Dirty)
			{
				return _filePath;
			}

			return _filePath + " *";
		}

		private void TextFieldOnTextChanged(object sender, EventArgs eventArgs)
		{
			Dirty = true;
		}

		private void Save(bool setFileName)
		{
			if (string.IsNullOrEmpty(FilePath) || setFileName)
			{
				using (var dlg = new SaveFileDialog())
				{
					dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
					if (dlg.ShowDialog() == DialogResult.OK)
					{
						FilePath = dlg.FileName;
					}
					else
					{
						return;
					}
				}
			}

			using (var writer = new StreamWriter(_filePath))
			{
				writer.Write(_textField.Text);
			}

			Dirty = false;
		}

		private void AboutItemOnDown(object sender, EventArgs eventArgs)
		{
			var messageBox = Graphics2D.UI.Window.CreateMessageBox("Notepad", "Myra Notepad Sample");
			messageBox.ShowModal(_host);
		}

		private void SaveAsItemOnDown(object sender, EventArgs eventArgs)
		{
			Save(true);
		}

		private void SaveItemOnDown(object sender, EventArgs eventArgs)
		{
			Save(false);
		}

		private void OpenItemOnDown(object sender, EventArgs eventArgs)
		{
			string filePath;
			using (var dlg = new OpenFileDialog())
			{
				dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					filePath = dlg.FileName;
				}
				else
				{
					return;
				}
			}

			using (var reader = new StreamReader(filePath))
			{
				_textField.Text = reader.ReadToEnd();
			}

			FilePath = _filePath;
			Dirty = false;
		}

		private void NewItemOnDown(object sender, EventArgs eventArgs)
		{
			FilePath = string.Empty;
			_textField.Text = string.Empty;
		}

		private void QuitItemOnDown(object sender, EventArgs genericEventArgs)
		{
			Exit();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GraphicsDevice.Clear(Color.Black);

			_host.Bounds = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			_host.Render();
		}
	}
}
