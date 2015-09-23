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
        public Gaussian(double sd, double mean, string name, Variable Variable)
            : base(name, Variable)
        {
            this.Sd = sd;
            this.Mean = mean;
        }
        public double Mean { get; private set; }
        public double Sd { get; private set; }

        protected override Expression GenerateAssign(ParameterExpression nameVariableX, ParameterExpression nameVariableY)
        {
            var divisor = Expression.Constant(-(2.0d * Sd * Sd), typeof(double));

            var subtractor = Expression.Constant(-(2.0d * Mean), typeof(double));
            var meanSquared = Expression.Constant(Mean * Mean, typeof(double));
            var xSquared = Expression.Multiply(nameVariableX, nameVariableX);
            var dividend = Expression.Add(xSquared, Expression.Add(Expression.Multiply(nameVariableX, subtractor),meanSquared));

            var expressionExp = Expression.Divide(dividend, divisor);

            var call = MathMethods.CreateExp(expressionExp);
            return Expression.Assign(nameVariableY, call);
        }
    }
}
