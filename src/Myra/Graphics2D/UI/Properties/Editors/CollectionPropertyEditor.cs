using System;
using System.Collections;
using System.Collections.Generic;

namespace Myra.Graphics2D.UI.Properties
{
	[PropertyEditor(typeof(CollectionPropertyEditor), typeof(IList))]
	public class CollectionPropertyEditor : PropertyEditor<IList>
	{
		private readonly Type collectionKind;
	    
		public CollectionPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
		{
			//if(methodInfo.Type.isg)
			collectionKind = methodInfo.Type.GenericTypeArguments[0];
		}
		
		protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
		{
			creatorDelegate = CreateCollectionPropEditor;
			return true;
		}
		
        private bool CreateCollectionPropEditor(out Widget widget)
        {
            object value = _record.GetValue(_owner.SelectedField);
            IList items = (IList)value;

            var subGrid = new Grid
            {
            	ColumnSpacing = 8,
            	HorizontalAlignment = HorizontalAlignment.Stretch
            };

            subGrid.ColumnsProportions.Add(new Proportion());
            subGrid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

            var label = new Label
            {
            	VerticalAlignment = VerticalAlignment.Center,
            };
            UpdateLabelCount(label, items.Count);

            subGrid.Widgets.Add(label);

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

            button.Click += (sender, args) =>
            {
            	var collectionEditor = new CollectionEditor(items, collectionKind);

            	var dialog = Dialog.CreateMessageBox("Edit", collectionEditor);

            	dialog.ButtonOk.Click += (o, eventArgs) =>
            	{
            		collectionEditor.SaveChanges();
            		UpdateLabelCount(label, items.Count);
            	};

            	dialog.ShowModal(_owner.Desktop);
            };

            subGrid.Widgets.Add(button);

            widget = subGrid;
            return true;
        }
        
        private static void UpdateLabelCount(Label textBlock, int count)
        {
	        textBlock.Text = string.Format("{0} Items", count);
        }
        
        public override void SetWidgetValue(object value)
        {
	        Console.WriteLine("CollectionPropertyEditor SetWidgetValue Not implemented");
        }
	}
}