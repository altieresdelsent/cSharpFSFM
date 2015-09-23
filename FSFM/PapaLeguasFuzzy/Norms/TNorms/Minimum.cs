using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using PapaLeguasFuzzy;


namespace PapaLeguasFuzzy.Norms.TNorms
{
    public class Minimum : TNorm
    {
        protected override Expression GenerateAssign(ParameterExpression nameVariableY1, ParameterExpression nameVariableY2, ParameterExpression variableYout)
        {
            var call = MathMethods.CreateMin(nameVariableY1, nameVariableY2);
            return Expression.Assign(variableYout, call);
        }
    }
}
