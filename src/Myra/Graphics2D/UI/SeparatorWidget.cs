using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;

#if !STRIDE
using Microsoft.Xna.Framework;
#else
using Stride.Core.Mathematics;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class SeparatorWidget : Image
	{
		public int Thickness
		{
			get; set;
		}

		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		protected SeparatorWidget(string styleName)
		{
			SetStyle(styleName);
		}

		public void ApplySeparatorStyle(SeparatorStyle style)
		{
			ApplyWidgetStyle(style);

			Renderable = style.Image;
			Thickness = style.Thickness;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Point.Zero;

			if (Orientation == Orientation.Horizontal)
			{
				result.Y = Thickness;
			}
			else
			{
				result.X = Thickness;
			}

			return result;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);
		}
	}
}