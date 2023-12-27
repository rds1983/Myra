namespace Myra.Graphics2D.UI.Styles
{
	public class WidgetStyle
	{
		public string Id { get; set; }

		public int? Width { get; set; }
		public int? Height { get; set; }

		public int? MinWidth { get; set; }
		public int? MinHeight { get; set; }

		public int? MaxWidth { get; set; }
		public int? MaxHeight { get; set; }

		public IBrush Background { get; set; }
		public IBrush OverBackground { get; set; }
		public IBrush DisabledBackground { get; set; }
		public IBrush FocusedBackground { get; set; }

		public IBrush Border { get; set; }

		public IBrush OverBorder { get; set; }
		public IBrush DisabledBorder { get; set; }
		public IBrush FocusedBorder { get; set; }

		public Thickness Margin { get; set; }

		public Thickness BorderThickness { get; set; }

		public Thickness Padding { get; set; }

		public WidgetStyle()
		{
		}

		public WidgetStyle(WidgetStyle style)
		{
			Width = style.Width;
			Height = style.Height;
			MinWidth = style.MinWidth;
			MinHeight = style.MinHeight;
			MaxWidth = style.MaxWidth;
			MaxHeight = style.MaxHeight;

			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			FocusedBackground = style.FocusedBackground;

			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;
			FocusedBorder = style.FocusedBorder;

			Margin = style.Margin;
			BorderThickness = style.BorderThickness;
			Padding = style.Padding;
		}

		public virtual WidgetStyle Clone()
		{
			return new WidgetStyle(this);
		}
	}
}
