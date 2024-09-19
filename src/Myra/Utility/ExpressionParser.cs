using Myra.Graphics2D.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Utility
{
	internal static class ExpressionParser
	{
		static public int DepthOfCalculations { get; set; } = 10;
		static private List<Widget> _widgets;
		public static void Parse(Widget widget, List<Widget> widgets, int level = 0)
		{
			_widgets = widgets;
			#region Check
			///check on depths
			if (level > DepthOfCalculations)
			{
				return;
			}
			///check parent on dynamic layout
			if (widget.Parent != null && !widget.Parent.Layout2d.Nullable)
			{
				Parse(widget.Parent, widgets, ++level);
			}
			#endregion
			///expresion parser
			info.lundin.math.ExpressionParser parser = new info.lundin.math.ExpressionParser();
			parser.ImplicitMultiplication = false;

			#region Calculation
			/// if X exp not null
			if (widget.Layout2d.PositionXExpression != "NULL")
				widget.Left = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.PositionXExpression, parser, widget));
			/// if Y exp not null
			if (widget.Layout2d.PositionYExpression != "NULL")
				widget.Top = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.PositionYExpression, parser, widget));
			/// if H exp not null
			if (widget.Layout2d.SizeYExpression != "NULL")
				widget.Height = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.SizeYExpression, parser, widget));
			/// if W exp not null
			if (widget.Layout2d.SizeXExpression != "NULL")
				widget.Width = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.SizeXExpression, parser, widget));
			#endregion
			widget.Layout2d.Calculated = true;
		}

		static private string CheckAndAddRef(string Expression, info.lundin.math.ExpressionParser parser, Widget widget)
		{
			string expression = Expression;

			/*replace to values*/
			///add this. values pos
			expression = expression.Replace("this.X", $"{widget.Left}");
			expression = expression.Replace("this.Y", $"{widget.Top}");
			expression = expression.Replace("this.l", $"{widget.Left}");
			expression = expression.Replace("this.t", $"{widget.Top}");
			expression = expression.Replace("this.left", $"{widget.Left}");
			expression = expression.Replace("this.top", $"{widget.Top}");
			///add this.size
			expression = expression.Replace("this.w", $"{widget.Width ?? 0}");
			expression = expression.Replace("this.h", $"{widget.Height ?? 0}");
			expression = expression.Replace("this.width", $"{widget.Width ?? 0}");
			expression = expression.Replace("this.height", $"{widget.Height ?? 0}");
			if (widget.Parent != null)
			{
				///add parent values pos
				expression = expression.Replace("&.X", GerParrentValue(widget, "&.X"));
				expression = expression.Replace("&.Y", GerParrentValue(widget, "&.Y"));
				expression = expression.Replace("&.l", GerParrentValue(widget, "&.X"));
				expression = expression.Replace("&.t", GerParrentValue(widget, "&.Y"));
				expression = expression.Replace("&.left", GerParrentValue(widget, "&.X"));
				expression = expression.Replace("&.top", GerParrentValue(widget, "&.Y"));
				///add parent values size
				expression = expression.Replace("&.w", GerParrentValue(widget, "&.w"));
				expression = expression.Replace("&.h", GerParrentValue(widget, "&.h"));
				expression = expression.Replace("&.width", GerParrentValue(widget, "&.w"));
				expression = expression.Replace("&.height", GerParrentValue(widget, "&.h"));
			}
			///add window walues
			expression = expression.Replace("W.h", $"{CrossEngineStuff.ViewSize.Y}");
			expression = expression.Replace("W.height", $"{CrossEngineStuff.ViewSize.Y}");
			expression = expression.Replace("W.w", $"{CrossEngineStuff.ViewSize.X}");
			expression = expression.Replace("W.width", $"{CrossEngineStuff.ViewSize.X}");
			///
			if (expression.Contains("["))
			{
				Regex regex = new Regex(@"\[.+].\w*");
				MatchCollection matches = regex.Matches(expression);
				foreach (var item in matches)
				{
					string Id = item.ToString().Split('[')[1].ToString().Split(']')[0].ToString();
					var w = GetByID(Id, _widgets);
					var valueForReg = GetValue(item.ToString().Substring(item.ToString().IndexOf(']') + 1, 1), w.Bounds);
					expression = expression.Replace(item as string, $"{valueForReg ?? 0}");
				}
			}
			return expression;
		}
		/*[btn]*/
		static private Widget GetByID(string Id, List<Widget> widgets)
		{
			return widgets.Find(i => { return i.Id == Id; });
		}
		/*.h*/

		static private string GerParrentValue(Widget widget, string ValueKey)
		{
			if (widget.Parent == null)
			{
				switch (ValueKey)
				{
					case "&.X":
					case "&.Y":
						return "0";
					case "&.w":
						return $"{CrossEngineStuff.ViewSize.X}";
					case "&.h":
						return $"{CrossEngineStuff.ViewSize.Y}";
				}
			}
			else
			{
				switch (ValueKey)
				{
					case "&.X":
						return $"{widget.Parent.Left}";
					case "&.Y":
						return $"{widget.Parent.Top}";
					case "&.w":
						return $"{widget.Parent.Width}";
					case "&.h":
						return $"{widget.Parent.Height}";
				}
			}
			return "0";
		}
		static private int? GetValue(string WalueName, Rectangle widgets)
		{
			switch (WalueName)
			{
				case "l":
				case "left":
				case "X":
					if (widgets.IsEmpty)
					{
						return 0;
					}
					return widgets.Left;
				case "t":
				case "top":
				case "Y":
					if (widgets.IsEmpty)
					{
						return 0;
					}
					return widgets.Top;
				case "w":
				case "width":
					if (widgets.IsEmpty)
					{
						return CrossEngineStuff.ViewSize.X;
					}
					return widgets.Width;
				case "h":
				case "height":
					if (widgets.IsEmpty)
					{
						return CrossEngineStuff.ViewSize.Y;
					}
					return widgets.Height;
				default:
					return 0;
			}
		}
	}
}
