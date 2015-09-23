using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
namespace PapaLeguasFuzzy
{
    public abstract class Norm : INorm
    {
        
        public Norm()
        {
            var expression = this.GeneratePertinencyFunc();
            _pertinency = this.GeneratePertinencyFunc().Compile();
        }

        private Func<double, double, double> _pertinency;
        public Func<double, double, double> Pertinency
        {
            get
            {
                if (_pertinency == null)
                    _pertinency = this.GeneratePertinencyFunc().Compile();
                return _pertinency;
            }
        }

        public Expression<Func<double, double, double>> GeneratePertinencyFunc()
        {
            var y1 = Expression.Parameter(typeof(double),"y1");
            var y2 = Expression.Parameter(typeof(double),"y2");
            var pertinency = Expression.Parameter(typeof(double),"pertinency");

            Expression body = GeneratePertinencyCode(y1,y2,pertinency, true, true);
            return Expression.Lambda<Func<double, double, double>>(body, new ParameterExpression[] {
                y1,
                y2
           });
        }
        protected abstract Expression GenerateAssign(ParameterExpression nameVariableY1, ParameterExpression nameVariableY2, ParameterExpression variableYout);
        public Expression GeneratePertinencyCode(ParameterExpression nameVariableY1, ParameterExpression nameVariableY2, ParameterExpression variableYout = null, bool createVariable = true, bool returnValue = true)
        {
            variableYout = variableYout ?? nameVariableY1;

            Expression assign = this.GenerateAssign(nameVariableY1, nameVariableY2, variableYout);

            List<ParameterExpression> variablesToCreate = new List<ParameterExpression>();
            if(createVariable)
                variablesToCreate.Add(variableYout);
            List<Expression> runCode = new List<Expression>();
            runCode.Add(assign);
            if (returnValue)
                runCode.Add(variableYout);
            Expression body = (variablesToCreate.Count > 0)? Expression.Block(typeof(double), variablesToCreate, runCode):Expression.Block(runCode);
            while (body.CanReduce)
                body = body.Reduce();
            return body;
        }
    }
}
