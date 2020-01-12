using System.Collections.Generic;
using System.Linq;

namespace Myra.Graphics2D.UI.Properties
{
    /*
         Знак для разделения условий :
         ';' - пример : " this.h = &.h * 10% ; this.t = [btn].l * 5% "
         
        (non work)
         Тильда для округления :
         ~M - по правилам математики
         ~D - отбросить знаки после запятой 

         поддержка знаков :
         '*' - умножение 
         '/' - деление
         '-' - вычитание 
         '+' - сложение 
         '()' - порядок выполнения 
         '%' - процент от чего либо Пример 10% -> 0.1
         '=' - знак присвоения

         Получить данные родителя : 
         &.left - позиция X or &.X
         &.top - позиция Y or &.Y
         &.width - ширина родителя 
         &.heigh - высота родителя 
         
         А так же сокращения 
         &.l - позиция Х
         &.t - позиция Y 
         &.w - ширина родителя
         &.h - высота родителя

         Поддержка получения виджета по _id: 
         [_id] - для получения данных о виджете в соответствии с его _id

         Задавать числа в условии :
         '10' -число 10 (float) '10'

         Значение NULL - не задано (не вычеслять)

         Установка позиции, размеров
         this.h = ...
         this.w = ...
         this.t = ...
         this.l = ...
             */
    public class Layout2D
    {
        /// <summary>
        /// Set default layout
        /// </summary>
        /// <returns>Default layout</returns>
        static public Layout2D NullLayout { get { return new Layout2D();} }

        #region Data
        private string _expressionXpoxition = "NULL";
        private string _expressionYpoxition = "NULL";
        private string _expressionXsize = "NULL";
        private string _expressionYsize = "NULL";
        #endregion

        #region Data acessor
        public string PositionXExpression
        {
            get { return _expressionXpoxition; }
            set
            {
                _expressionXpoxition = value.Split('=')[1];
                Nullable = false;
            }
        }
        public string PositionYExpression
        {
            get { return _expressionYpoxition; }
            set
            {
                _expressionYpoxition = value.Split('=')[1];
                Nullable = false;
            }
        }
        public string SizeXExpression
        {
            get { return _expressionXsize; }
            set
            {
                _expressionXsize = value.Split('=')[1];
                Nullable = false;
            }
        }
        public string SizeYExpression
        {
            get { return _expressionYsize; }
            set
            {
                _expressionYsize = value.Split('=')[1];
                Nullable = false;
            }
        }
        public string Expresion { get { return $"this.X = {PositionXExpression} ; this.Y = {PositionYExpression} ; this.w = {SizeXExpression} ; this.h = {SizeYExpression}"; } set { ParseLayoutOnExpressions(value); } }
        #endregion

        #region Props
        public bool Nullable { get; set; } = true;
        public bool Calculated { get; set; } = false;
        #endregion

        public Layout2D(string Expression = "NULL")
        {
            if (Expression != "NULL")
                ParseLayoutOnExpressions(Expression);
        }

        private void ParseLayoutOnExpressions(string expression)
        {
            Nullable = false;

            expression.Split(';').ToList().ForEach(
                i => {
                    //if height size expression
                    if (i.Contains("this.h") || i.Contains("this.height"))
                        SizeYExpression = i;
                    //if width size expression
                    if (i.Contains("this.w") || i.Contains("this.width"))
                        SizeXExpression = i;
                    //if X position expression
                    if (i.Contains("this.X") || i.Contains("this.left") || i.Contains("this.l"))
                        PositionXExpression = i;
                    //if Y position expression
                    if (i.Contains("this.Y") || i.Contains("this.top") || i.Contains("this.t"))
                        PositionYExpression = i;
                }
                );
        }
    }
}
