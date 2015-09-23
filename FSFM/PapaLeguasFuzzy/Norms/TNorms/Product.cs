using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PapaLeguasFuzzy.Norms.TNorms
{
    public class Product : TNorm
    {
        protected override Expression GenerateAssign(ParameterExpression nameVariableY1, ParameterExpression nameVariableY2, ParameterExpression variableYout)
        {
            return Expression.Assign(variableYout, Expression.Multiply(nameVariableY1, nameVariableY2));
        }
    }
}
