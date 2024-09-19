using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	public abstract class SeparatorWidget : Image
	{
		public int Thickness { get; set; }

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
			var result = Mathematics.PointZero;

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

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var separator = (SeparatorWidget)w;
			Thickness = separator.Thickness;
		}
	}
}