using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Properties;

namespace Myra.Samples.Inspector
{
    // All the widgets for this sample project are children of this Widget.
    public class RootWidgets : HorizontalStackPanel
    {
        private Label headerLabel;
        private Label infoLabel;
        private PropertyGrid propertyGrid;

        private bool _init;
        private int select_index, info_index;
        public readonly List<object> inspectables;
        public readonly List<StringGenerator> infos;
        private readonly StringGenerator headerInfo;

        private readonly SomeChangingValues _changingValues;
        
        public RootWidgets()
        {
            BuildUI();

            _changingValues = new SomeChangingValues();
            inspectables = BuildInspectables();
            infos = BuildInfoGenerators();
            headerInfo = new StringGenerator(() =>
            {
                Type inspectedType = inspectables[select_index].GetType();
                return $"\nInspecting object [{select_index+1}] of [{inspectables.Count}]:\n\nType: {inspectedType.Name}\nin: {inspectedType.Namespace}\nBaseType: {inspectedType.BaseType?.Name}\n\nAssembly:\n{inspectedType.Assembly.GetName().Name}\n";
            });
            
            Inspect(inspectables[0]);
        }

        /// <summary>
        /// Build the list of objects whose properties can be viewed
        /// </summary>
        private List<object> BuildInspectables()
        {
            return new List<object>()
            {
                new SomeTypesInAClass(),
                new SomeNumerics(),
                new SomeNullableNumerics(),
                _changingValues,
                new SpinButton<int>(),
                this,
                propertyGrid,
                InspectGame.Instance
            };
        }

        /// <summary>
        /// Build the list of info-outputs which generate strings
        /// </summary>
        private List<StringGenerator> BuildInfoGenerators()
        {
            return new List<StringGenerator>()
            {
                new StringGenerator(() =>
                {
                    return "\n=== Controls ===\nArrowUp/ArrowDown:\n  Cycle inspected object\n\nArrowLeft/ArrowRight:\n  Cycle this info display";
                }),
                new StringGenerator(() =>
                {
                    // List loaded assemblies
                    List<Assembly> assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
                    assemblies.Sort(
                        (a, b) => Comparer<string>.Default.Compare(a.GetName().Name, b.GetName().Name)
                    );
                    StringBuilder builder = new StringBuilder();
                    int i = 0;
                    foreach (Assembly asm in assemblies)
                    {
                        builder.AppendLine($" [{i}]: {asm.GetName().Name}");
                        i++;
                    }
                    return $"\n=== Loaded Assemblies ===\n{builder}";
                }),
            };
        }

        private void BuildUI()
        {
            ScrollViewer scroll;
            int scrollBarWidth = 30;

            int uiWidth = 500;
            headerLabel = new Label()
            {
                Width = uiWidth - scrollBarWidth,
                SingleLine = false,
            };
            infoLabel = new Label()
            {
                Width = uiWidth - scrollBarWidth,
                SingleLine = false,
            };

            var vert = new VerticalStackPanel()
            {
                Width = uiWidth - scrollBarWidth,
            };
            vert.Widgets.Add(headerLabel);
            vert.Widgets.Add(new HorizontalSeparator());
            vert.Widgets.Add(infoLabel);

            scroll = new ScrollViewer()
            {
                ShowHorizontalScrollBar = false,
                Content = vert,
                Padding = new Thickness(8),
                Width = uiWidth,
            };
            Widgets.Add(scroll);

            uiWidth = 700;
            propertyGrid = new PropertyGrid()
            {
                Width = uiWidth - scrollBarWidth,
            };
            scroll = new ScrollViewer()
            {
                ShowHorizontalScrollBar = false,
                Content = propertyGrid,
                Padding = new Thickness(8),
                Width = uiWidth,
            };
            Widgets.Add(scroll);
        }
        
        public void Inspect(object obj)
        {
            propertyGrid.Object = obj;
        }

        public void SetSelectedIndex(int index)
        {
            select_index = index;
            propertyGrid.Object = inspectables[index];
            headerInfo.MarkDirty();
        }
        public void SetInfoIndex(int index)
        {
            info_index = index;
            infos[info_index].MarkDirty();
        }
        
        public void OnPreRender()
        {
            headerLabel.Text = headerInfo.Text;
            infoLabel.Text = infos[info_index].Text;
        }

        public void Update(GameTime time)
        {
            _changingValues.Update(time);
        }
    }
}