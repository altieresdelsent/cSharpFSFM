using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace PapaLeguasFuzzy.Terms
{
    public class Sigmoid : Term
    {
        public Sigmoid(double slope, double inflection, string name, Variable variable)
            : base(name, variable)
        {
            this.Slope = slope;
            this.Inflection = inflection;
        }
        public double Slope { get; private set; }
        public double Inflection { get; private set; }

        protected override Expression GenerateAssign(ParameterExpression nameVariableX, ParameterExpression nameVariableY)
        {
            var yMinusInflection = Expression.Add(nameVariableX, Expression.Constant(-Inflection, typeof(double)));
            var expressionExp = Expression.Multiply(Expression.Constant(-Slope, typeof(double)), yMinusInflection);
            var call = MathMethods.CreateExp(expressionExp);
            var constant1 = Expression.Constant(1.0d);
            var formulaSigmoid = Expression.Divide(constant1, Expression.Add(constant1, call));
            return Expression.Assign(nameVariableY, formulaSigmoid);
        }
    }
}
