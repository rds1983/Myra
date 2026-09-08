using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Samples;

public partial class AllWidgets
{
	public AllWidgets()
	{
		BuildUI();

		var tree = new TreeView();
		Grid.SetColumn(tree, 1);
		Grid.SetRow(tree, 12);
		Grid.SetColumnSpan(tree, 2);

		var node1 = tree.AddSubNode(new Label
		{
			Text = "node1"
		});
		var node2 = node1.AddSubNode(new Label
		{
			Text = "node2"
		});

		var node4 = node2.AddSubNode(new Label
		{
			Text = "node6"
		});
		node4.AddSubNode(new Label
		{
			Text = "node11"
		});
		node4.AddSubNode(new Label
		{
			Text = "node12"
		});
		node4.AddSubNode(new Label
		{
			Text = "node13"
		});
		node4.AddSubNode(new Label
		{
			Text = "node14"
		});
		node4.AddSubNode(new Label
		{
			Text = "node15"
		});
		node4.AddSubNode(new Label
		{
			Text = "node16"
		});
		node4.AddSubNode(new Label
		{
			Text = "node17"
		});
		node4.AddSubNode(new Label
		{
			Text = "node18"
		});

		var node3 = node2.AddSubNode(new CheckButton
		{
			Content = new Label
			{
				Text = "CheckButton node"
			}
		});
		node3.AddSubNode(new Label
		{
			Text = "node4"
		});
		node3.AddSubNode(new CheckButton
		{
			Content = new Label { Text = "CheckButton node2" },
			CheckPosition = CheckPosition.Right,
			CheckContentSpacing = 8
		});

		var imageButtonContent = new HorizontalStackPanel
		{
			Spacing = 4
		};

		imageButtonContent.Widgets.Add(new Image
		{
			Renderable = Stylesheet.Current.Atlas["icon-star"]
		});

		imageButtonContent.Widgets.Add(new Label
		{
			Text = "Button node"
		});
		node3.AddSubNode(new Button
		{
			Content = imageButtonContent
		});
		node3.AddSubNode(new HorizontalSlider());
		node3.AddSubNode(new SpinButton());

		var imageButtonContent2 = new HorizontalStackPanel
		{
			Spacing = 4
		};

		imageButtonContent2.Widgets.Add(new Label
		{
			Text = "ToggleButton node"
		});
		imageButtonContent2.Widgets.Add(new Image
		{
			Renderable = Stylesheet.Current.Atlas["icon-star"]
		});

		node3.AddSubNode(new ToggleButton
		{
			Content = imageButtonContent2
		});

		_gridRight.Widgets.Add(tree);
	}

	public override void InternalRender(RenderContext context)
	{
		base.InternalRender(context);

		_horizontalProgressBar.Value += 0.5f;
		if (_horizontalProgressBar.Value > _horizontalProgressBar.Maximum)
		{
			_horizontalProgressBar.Value = _horizontalProgressBar.Minimum;
		}

		_verticalProgressBar.Value += 0.5f;
		if (_verticalProgressBar.Value > _verticalProgressBar.Maximum)
		{
			_verticalProgressBar.Value = _verticalProgressBar.Minimum;
		}
	}
}