using info.lundin.math;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Myra.Utility
{
    static class ExpressionParser
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
            if (widget.Parent!=null&&!widget.Parent.Layout2d.Nullable)
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
                widget.Left = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.PositionXExpression,parser, widget));
            /// if Y exp not null
            if (widget.Layout2d.PositionYExpression != "NULL")
                widget.Top = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.PositionYExpression, parser, widget));
            /// if H exp not null
            if (widget.Layout2d.SizeYExpression != "NULL")
                widget.Height = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.SizeYExpression, parser, widget));
            /// if W exp not null
            if (widget.Layout2d.SizeXExpression != "NULL")
                widget.Width = (int)parser.Parse(CheckAndAddRef(widget.Layout2d.SizeXExpression, parser,widget));
            #endregion
            widget.Layout2d.Calculated = true;
        }

        static private string CheckAndAddRef(string Expression, info.lundin.math.ExpressionParser parser,Widget widget) {
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
            ///add parent values pos
            expression = expression.Replace("&.X", $"{widget.Parent.Left}");
            expression = expression.Replace("&.Y", $"{widget.Parent.Top}");
            expression = expression.Replace("&.l", $"{widget.Parent.Left}");
            expression = expression.Replace("&.t", $"{widget.Parent.Top}");
            expression = expression.Replace("&.left", $"{widget.Parent.Left}");
            expression = expression.Replace("&.top", $"{widget.Parent.Top}");
            ///add parent values size
            expression = expression.Replace("&.w", $"{widget.Parent.Width ?? 0}");
            expression = expression.Replace("&.h", $"{widget.Parent.Height ?? 0}");
            expression = expression.Replace("&.width", $"{widget.Parent.Width ?? 0}");
            expression = expression.Replace("&.height", $"{widget.Parent.Height ?? 0}");
            ///add window walues
            expression = expression.Replace("W.h", $"{MyraEnvironment.Game.Window.ClientBounds.Height}");
            expression = expression.Replace("W.height", $"{MyraEnvironment.Game.Window.ClientBounds.Height}");
            expression = expression.Replace("W.w", $"{MyraEnvironment.Game.Window.ClientBounds.Width}");
            expression = expression.Replace("W.width", $"{MyraEnvironment.Game.Window.ClientBounds.Width}");
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
                        return MyraEnvironment.Game.GraphicsDevice.Viewport.Width;
                    }
                    return widgets.Width;
                case "h":
                case "height":
                    if (widgets.IsEmpty)
                    {
                        return MyraEnvironment.Game.GraphicsDevice.Viewport.Height;
                    }
                    return widgets.Height;
                default:
                    return 0;
            }
        }
    }
}
