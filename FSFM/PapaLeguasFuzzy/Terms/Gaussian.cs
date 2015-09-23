using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PapaLeguasFuzzy.Terms
{
    public class Gaussian : Term
    {
        public enum GaussianType
        {
            Normal,
            Left,
            Right
        }
        public Gaussian(double sd, double mean, string name, Variable Variable,GaussianType type = GaussianType.Normal)
            : base(name, Variable)
        {
            this.Sd = sd;
            this.Mean = mean;
            this.Type = type;
        }

        public double Mean { get; private set; }
        public double Sd { get; private set; }
        public GaussianType Type { get; private set; }

        protected override Expression GenerateAssign(ParameterExpression nameVariableX, ParameterExpression nameVariableY)
        {
            var divisor = Expression.Constant(-(2.0d * Sd * Sd), typeof(double));

            var subtractor = Expression.Constant(-(2.0d * Mean), typeof(double));
            var meanSquared = Expression.Constant(Mean * Mean, typeof(double));
            var xSquared = Expression.Multiply(nameVariableX, nameVariableX);
            var dividend = Expression.Add(xSquared, Expression.Add(Expression.Multiply(nameVariableX, subtractor),meanSquared));

            var expressionExp = Expression.Divide(dividend, divisor);

            var call = MathMethods.CreateExp(expressionExp);
            if (this.Type == GaussianType.Normal)
                return Expression.Assign(nameVariableY, call);
            else if (this.Type == GaussianType.Left)
                return Expression.Condition(Expression.LessThanOrEqual(nameVariableX, Expression.Constant(this.Mean)), Expression.Constant(1.0d), call);
            else
                return Expression.Condition(Expression.GreaterThanOrEqual(nameVariableX, Expression.Constant(this.Mean)), Expression.Constant(1.0d), call);
        }
    }
}
