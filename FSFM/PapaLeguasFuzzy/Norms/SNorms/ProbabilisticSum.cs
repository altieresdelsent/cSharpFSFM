using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PapaLeguasFuzzy.Norms.SNorms
{
    public class ProbabilisticSum : SNorm
    {
        protected override Expression GenerateAssign(ParameterExpression nameVariableY1, ParameterExpression nameVariableY2, ParameterExpression variableYout)
        {
            return Expression.Assign(variableYout,
                    Expression.Subtract(
                        Expression.Add(nameVariableY1, nameVariableY2),
                        Expression.Multiply(nameVariableY1, nameVariableY2)
                    )
                );
        }
    }
}
