using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PapaLeguasFuzzy.Norms.SNorms
{
    public class Maximum : SNorm
    {
        protected override Expression GenerateAssign(ParameterExpression nameVariableY1, ParameterExpression nameVariableY2, ParameterExpression variableYout)
        {
            var call = MathMethods.CreateMax(nameVariableY1, nameVariableY2);
            return Expression.Assign(variableYout, call);
        }
    }
}
