using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myra.Utility
{
    static class ExpressionParser
    {
        static public int DepthOfCalculations { get; set; } = 10;
        static private Widget cWidget;
        public static void Parse(Widget widget, List<Widget> widgets, int level = 0)
        {
            if (level > DepthOfCalculations)
            {
                return;
            }
            cWidget = widget;
            var Parent = widget.Parent;

            if (widget.Layout2d.PositionXExpression != "NULL")
                widget.Left = GetValueByExpression(widget.Layout2d.PositionXExpression, widgets, Parent, level);
            if (widget.Layout2d.PositionYExpression != "NULL")
                widget.Top = GetValueByExpression(widget.Layout2d.PositionYExpression, widgets, Parent, level);
            if (widget.Layout2d.SizeYExpression != "NULL")
                widget.Height = GetValueByExpression(widget.Layout2d.SizeYExpression, widgets, Parent, level);
            if (widget.Layout2d.SizeXExpression != "NULL")
                widget.Width = GetValueByExpression(widget.Layout2d.SizeXExpression, widgets, Parent, level);
            widget.Layout2d.Calculated = true;
        }

        static private int GetValueByExpression(string Expression, List<Widget> widgets, Widget parent, int level)
        {
            /*Example (&.h * 10% - '10' + [btn].l * 5% )*/

            List<string> operators = Expression.Split(' ').ToList();

            float val = ParseOperator(operators[1], widgets, parent, level);

            for (int i = 0; i < operators.Count; i++)
            {
                if (operators[i].Contains("*") || operators[i].Contains("/") || operators[i].Contains("+") || operators[i].Contains("-"))
                    val = Calc(val, ParseOperator(operators[i + 1], widgets, parent, level), operators[i]);
            }

            return (int)val;
        }

        static private float ParseOperator(string Expression, List<Widget> widgets, Widget parent, int level)
        {
            //получить значение у родителя
            if (Expression.Contains("&."))
            {
                if (parent!=null)
                {

                    if (!parent.Layout2d.Nullable && !parent.Layout2d.Calculated)
                    {
                        Parse(parent, widgets, level++);
                    }
                    return (float)GetValue(Expression.Split('.')[1], parent.Bounds);
                }
                return (float)GetValue(Expression.Split('.')[1], new Rectangle());
            }
            if (Expression.Contains("this."))
                return (float)GetValue(Expression.Split('.')[1], cWidget.Bounds);
            //замена знака процента
            if (Expression.Contains("%"))
                return float.Parse(Expression.Replace("%", "")) / 100;
            //получение цифрового значения
            if (Expression.Contains("'"))
                return float.Parse(Expression.Replace("'", ""));
            //получить по ID
            if (Expression.Contains("["))
            {
                var widget = GetByID(Expression.Replace("[", "").Replace("]", "").Split('.')[0], widgets);
                if (!widget.Layout2d.Nullable && !widget.Layout2d.Calculated)
                {
                    Parse(parent, widgets, level++);
                }
                return (float)GetValue(Expression.Split('.')[1], widget.Bounds);
            }
            return 0;
        }

        /*поддержка знаков :
         '*' - умножение 
         '/' - деление
         '-' - вычитание 
         '+' - сложение 
         '()' - порядок выполнения 
         '%' - процент от чего либо Пример 10% -> 0.1
         '=' - знак присвоения*/
        static private float Calc(float val1, float val2, string act)
        {
            switch (act)
            {
                case "*":
                    return val1 * val2;
                case "/":
                    return val1 / val2;
                case "-":
                    return val1 - val2;
                case "+":
                    return val1 + val2;

                default:
                    return 0;
            }
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
        static private float Round(float val, string ex)
        {
            if (ex.Contains("~D"))
                return (float)Math.Round(val, MidpointRounding.AwayFromZero);
            if (ex.Contains("~M"))
                return (float)Math.Round(val, MidpointRounding.ToEven);
            return 0;
        }
    }
}
