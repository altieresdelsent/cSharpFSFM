using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PapaLeguasFuzzy
{
    public static class MathMethods
    {
        public static readonly MethodInfo Abs = typeof(Math).GetMethod("Abs", new Type[] { typeof(double) });
        public static readonly MethodInfo Acos = typeof(Math).GetMethod("Acos", new Type[] { typeof(double) });
        public static readonly MethodInfo Asin = typeof(Math).GetMethod("Asin", new Type[] { typeof(double) });
        public static readonly MethodInfo Atan = typeof(Math).GetMethod("Atan", new Type[] { typeof(double) });
        public static readonly MethodInfo Atan2 = typeof(Math).GetMethod("Atan2", new Type[] { typeof(double), typeof(double) });
        public static readonly MethodInfo Ceiling = typeof(Math).GetMethod("Ceiling", new Type[] { typeof(double) });
        public static readonly MethodInfo Cos = typeof(Math).GetMethod("Cos", new Type[] { typeof(double) });
        public static readonly MethodInfo Cosh = typeof(Math).GetMethod("Cosh", new Type[] { typeof(double) });
        public static readonly MethodInfo Exp = typeof(Math).GetMethod("Exp");
        public static readonly MethodInfo Floor = typeof(Math).GetMethod("Floor", new Type[] { typeof(double) });
        public static readonly MethodInfo IEEERemainder = typeof(Math).GetMethod("IEEERemainder", new Type[] { typeof(double), typeof(double) });
        public static readonly MethodInfo Log10 = typeof(Math).GetMethod("Log10", new Type[] { typeof(double) });
        public static readonly MethodInfo Max = typeof(Math).GetMethod("Max", new Type[] { typeof(double), typeof(double) });
        public static readonly MethodInfo Min = typeof(Math).GetMethod("Min", new Type[] { typeof(double), typeof(double) });
        public static readonly MethodInfo Pow = typeof(Math).GetMethod("Pow", new Type[] { typeof(double), typeof(double) });
        public static readonly MethodInfo Round = typeof(Math).GetMethod("Round", new Type[] { typeof(double) });
        public static readonly MethodInfo Round2 = typeof(Math).GetMethod("Round", new Type[] { typeof(double), typeof(int) });
        public static readonly MethodInfo Sign = typeof(Math).GetMethod("Sign", new Type[] { typeof(double) });
        public static readonly MethodInfo Sin = typeof(Math).GetMethod("Sin", new Type[] { typeof(double) });
        public static readonly MethodInfo Sinh = typeof(Math).GetMethod("Sinh", new Type[] { typeof(double) });
        public static readonly MethodInfo Sqrt = typeof(Math).GetMethod("Sqrt", new Type[] { typeof(double) });
        public static readonly MethodInfo Tan = typeof(Math).GetMethod("Tan", new Type[] { typeof(double) });
        public static readonly MethodInfo Tanh = typeof(Math).GetMethod("Tanh", new Type[] { typeof(double) });

        public static Expression CreateMax(Expression expression1, Expression expression2)
        {
            return Expression.Condition(Expression.GreaterThan(expression1, expression2), expression1, expression2);
            //return Expression.Call(MathMethods.Max, expression1, expression2);
        }
        public static Expression CreateMin(Expression expression1, Expression expression2)
        {
            return Expression.Condition(Expression.LessThan(expression1, expression2), expression1, expression2);
            //return Expression.Call(MathMethods.Min, expression1, expression2);
        }
        public static Expression CreateExp(Expression expression)
        {
            //return Expression.Power(Expression.Constant(Math.E,typeof(double)), expression);
            return Expression.Call(Exp, expression);
        }
        public static Expression CreateRound(Expression expression, int digits)
        {
            //return Expression.Power(Expression.Constant(Math.E,typeof(double)), expression);
            return Expression.Call(Round2, expression,Expression.Constant(digits));
        }
    }
}
