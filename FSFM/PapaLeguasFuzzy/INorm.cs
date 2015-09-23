using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PapaLeguasFuzzy
{
    public interface INorm
    {
        Func<double, double, double> Pertinency{get;}
        Expression GeneratePertinencyCode(ParameterExpression nameVariableY1, ParameterExpression nameVariableY2, ParameterExpression variableYout = null, bool createVariable = true, bool returnValue = true);
        Expression<Func<double, double, double>> GeneratePertinencyFunc();
    }
}
