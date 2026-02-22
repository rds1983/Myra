using System;
using Microsoft.Xna.Framework;
using FontStashSharp.RichText;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.ColorPicker;
using Myra.Graphics2D.UI.Styles;
using Myra.MML;

namespace Myra.Graphics2D.UI.Properties
{
	[PropertyEditor(typeof(BrushPropertyEditor), typeof(IBrush))]
	public class BrushPropertyEditor : PropertyEditor<IBrush>
	{
		private Image _imgDisplay;
		public BrushPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
		{
			
		}
		
		protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
		{
			creatorDelegate = CreateBrushEditor;
			return true;
		}
		
        private bool CreateBrushEditor(out Widget widget)
        {
			var value = GetValue(_owner.SelectedField) as SolidBrush;

			var subGrid = new Grid
			{
				ColumnSpacing = 8,
				HorizontalAlignment = HorizontalAlignment.Stretch
			};

			subGrid.ColumnsProportions.Add(new Proportion());
			subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			var color = Color.Transparent;
			if (value != null)
			{
				color = value.Color;
			}

			var image = new Image
			{
				Renderable = Stylesheet.Current.WhiteRegion,
				VerticalAlignment = VerticalAlignment.Center,
				Width = 32,
				Height = 16,
				Color = color
			};

			subGrid.Widgets.Add(image);

			var button = new Button
			{
				Tag = value,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Content = new Label
				{
					Text = "Change...",
					HorizontalAlignment = HorizontalAlignment.Center,
				}
			};
			Grid.SetColumn(button, 1);

			subGrid.Widgets.Add(button);

			if (_record.HasSetter)
			{
				button.Click += (sender, args) =>
				{
					var dlg = new ColorPickerDialog()
					{
						Color = image.Color
					};

					dlg.Closed += (s, a) =>
					{
						if (!dlg.Result)
						{
							return;
						}

						image.Color = dlg.Color;
						SetValue(_owner.SelectedField, new SolidBrush(dlg.Color));
						var baseObject = _owner.SelectedField as BaseObject;
						if (baseObject != null)
						{
							baseObject.Resources[_record.Name] = dlg.Color.ToHexString();
						}
						_owner.FireChanged(_record.Type.Name);
					};

					dlg.ShowModal(_owner.Desktop);
				};
			}
			else
			{
				button.Enabled = false;
			}

			_imgDisplay = image;
			Widget = widget = subGrid;
			return true;
        }
        
        public override void SetWidgetValue(object value)
        {
	        if(_imgDisplay == null || value == null)
		        return;
	        
	        if (value is Color color)
		        _imgDisplay.Color = color;
	        else if (value is SolidBrush sBrush)
		        _imgDisplay.Color = sBrush.Color;
	        else if(value is IBrush otherBrush)
		        Console.WriteLine($"Can't display IBrush type: {otherBrush.GetType()}");
        }
	}
}