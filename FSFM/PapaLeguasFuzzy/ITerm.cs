using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PapaLeguasFuzzy
{
    public interface ITerm
    {
        Func<double, double> Pertinency { get; }
        string Name { get; }
        string FullName { get; }
        Expression GeneratePertinencyCode(ParameterExpression nameVariableX, ParameterExpression nameVariableY, bool createVariable = true, bool returnValue = true);
        Expression<Func<double, double>> GeneratePertinencyFunc();

    }
}
