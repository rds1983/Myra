/* Generated by MyraPad at 10/23/2023 9:52:44 AM */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp.RichText;
using AssetManagementBase;

#if STRIDE
using Stride.Core.Mathematics;
#elif PLATFORM_AGNOSTIC
using System.Drawing;
using System.Numerics;
using Color = FontStashSharp.FSColor;
#else
// MonoGame/FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace Myra.Samples.AssetManagement
{
	partial class MainForm: Panel
	{
		private void BuildUI()
		{
			var label1 = new Label();
			label1.Text = "My Game";
			label1.Font = MyraEnvironment.DefaultAssetManager.LoadFont("fonts/arial64.fnt");
			label1.TextColor = Color.LightBlue;
			label1.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;

			_menuItemStartNewGame = new MenuItem();
			_menuItemStartNewGame.Text = "Start New Game";
			_menuItemStartNewGame.Id = "_menuItemStartNewGame";

			_menuItemOptions = new MenuItem();
			_menuItemOptions.Text = "Options";
			_menuItemOptions.Id = "_menuItemOptions";

			_menuItemQuit = new MenuItem();
			_menuItemQuit.Text = "Quit";
			_menuItemQuit.Id = "_menuItemQuit";

			_mainMenu = new VerticalMenu();
			_mainMenu.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;
			_mainMenu.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Center;
			_mainMenu.LabelFont = MyraEnvironment.DefaultAssetManager.LoadFont("fonts/comicSans48.fnt");
			_mainMenu.LabelColor = Color.Indigo;
			_mainMenu.SelectionHoverBackground = new SolidBrush("#808000FF");
			_mainMenu.SelectionBackground = new SolidBrush("#FFA500FF");
			_mainMenu.LabelHorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;
			_mainMenu.HoverIndexCanBeNull = false;
			_mainMenu.Border = new SolidBrush("#00000000");
			_mainMenu.Background = new SolidBrush("#00000000");
			_mainMenu.Id = "_mainMenu";
			_mainMenu.Items.Add(_menuItemStartNewGame);
			_mainMenu.Items.Add(_menuItemOptions);
			_mainMenu.Items.Add(_menuItemQuit);

			var image1 = new Image();
			image1.Renderable = MyraEnvironment.DefaultAssetManager.LoadTextureRegion("images/LogoOnly_64px.png");
			image1.Left = 10;
			image1.Top = -10;
			image1.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Bottom;

			var label2 = new Label();
			label2.Text = "Version 0.6";
			label2.Font = MyraEnvironment.DefaultAssetManager.LoadFont("fonts/calibri32.fnt");
			label2.Left = -10;
			label2.Top = -10;
			label2.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Right;
			label2.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Bottom;

			
			Background = new SolidBrush("#C78100FF");
			Widgets.Add(label1);
			Widgets.Add(_mainMenu);
			Widgets.Add(image1);
			Widgets.Add(label2);
		}

		
		public MenuItem _menuItemStartNewGame;
		public MenuItem _menuItemOptions;
		public MenuItem _menuItemQuit;
		public VerticalMenu _mainMenu;
	}
}
