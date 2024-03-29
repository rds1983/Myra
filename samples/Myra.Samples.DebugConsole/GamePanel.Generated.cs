/* Generated by MyraPad at 11/14/2023 3:31:33 AM */
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

namespace Myra.Samples.DebugConsole
{
	partial class GamePanel: Panel
	{
		private void BuildUI()
		{
			var label1 = new Label();
			label1.Text = "Game Screen";
			label1.Top = 100;
			label1.HorizontalAlignment = Myra.Graphics2D.UI.HorizontalAlignment.Center;

			var label2 = new Label();
			label2.Text = "Radio 1";

			var radioButton1 = new RadioButton();
			radioButton1.Content = label2;

			var label3 = new Label();
			label3.Text = "Radio 2";

			var radioButton2 = new RadioButton();
			radioButton2.Content = label3;

			var label4 = new Label();
			label4.Text = "Radio 3";

			var radioButton3 = new RadioButton();
			radioButton3.Content = label4;

			var label5 = new Label();
			label5.Text = "Radio 4";

			var radioButton4 = new RadioButton();
			radioButton4.Content = label5;

			var horizontalStackPanel1 = new HorizontalStackPanel();
			horizontalStackPanel1.Top = 50;
			horizontalStackPanel1.Widgets.Add(radioButton1);
			horizontalStackPanel1.Widgets.Add(radioButton2);
			horizontalStackPanel1.Widgets.Add(radioButton3);
			horizontalStackPanel1.Widgets.Add(radioButton4);

			var label6 = new Label();
			label6.Text = "Button 1";

			var button1 = new Button();
			button1.Content = label6;

			var label7 = new Label();
			label7.Text = "Button 2";

			var button2 = new Button();
			button2.Content = label7;

			var label8 = new Label();
			label8.Text = "Button 3";

			var button3 = new Button();
			button3.Content = label8;

			var label9 = new Label();
			label9.Text = "Button 4";

			var button4 = new Button();
			button4.Content = label9;

			var horizontalStackPanel2 = new HorizontalStackPanel();
			horizontalStackPanel2.Spacing = 8;
			horizontalStackPanel2.Top = 300;
			horizontalStackPanel2.Widgets.Add(button1);
			horizontalStackPanel2.Widgets.Add(button2);
			horizontalStackPanel2.Widgets.Add(button3);
			horizontalStackPanel2.Widgets.Add(button4);

			var label10 = new Label();
			label10.Text = "Show Debug Panel";

			_buttonDebugPanel = new Button();
			_buttonDebugPanel.Id = "_buttonDebugPanel";
			_buttonDebugPanel.Content = label10;

			var label11 = new Label();
			label11.Text = "Show Debug Panel Modal";

			_buttonModalDebugPanel = new Button();
			_buttonModalDebugPanel.Id = "_buttonModalDebugPanel";
			_buttonModalDebugPanel.Content = label11;

			var horizontalStackPanel3 = new HorizontalStackPanel();
			horizontalStackPanel3.Spacing = 8;
			horizontalStackPanel3.VerticalAlignment = Myra.Graphics2D.UI.VerticalAlignment.Bottom;
			horizontalStackPanel3.Widgets.Add(_buttonDebugPanel);
			horizontalStackPanel3.Widgets.Add(_buttonModalDebugPanel);

			
			Widgets.Add(label1);
			Widgets.Add(horizontalStackPanel1);
			Widgets.Add(horizontalStackPanel2);
			Widgets.Add(horizontalStackPanel3);
		}

		
		public Button _buttonDebugPanel;
		public Button _buttonModalDebugPanel;
	}
}
