using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PapaLeguasFuzzy
{
    public abstract class Term : ITerm
    {
        public Term(string  Name, Variable Variable)
        {
            this.Name = Name.ToLowerInvariant();
            this.Variable = Variable;
            if (this.Variable.Terms.IndexOf(this) < 0)
                this.Variable.Terms.Add(this);
        }

        private Func<double, double> _pertinency;
        public Func<double, double> Pertinency
        {
            get
            {
                if (_pertinency == null)
                    _pertinency = this.GeneratePertinencyFunc().Compile();
                return _pertinency;
            }
        }

        public Expression<Func<double, double>> GeneratePertinencyFunc()
        {
            var x = Expression.Parameter(typeof(double), "x");
            var pertinency = Expression.Parameter(typeof(double), "y");

            Expression body = GeneratePertinencyCode(x, pertinency, true, true);
            return Expression.Lambda<Func<double, double>>(body, new ParameterExpression[] {
                x
           });
        }

        public Expression GeneratePertinencyCode(ParameterExpression nameVariableX, ParameterExpression nameVariableY, bool createVariable = true, bool returnValue = true)
        {
            Expression assign = this.GenerateAssign(nameVariableX, nameVariableY);

            List<ParameterExpression> variablesToCreate = new List<ParameterExpression>();
            if (createVariable)
                variablesToCreate.Add(nameVariableY);
            List<Expression> runCode = new List<Expression>();
            runCode.Add(assign);
            if (returnValue)
                runCode.Add(nameVariableY);

            Expression body = (variablesToCreate.Count > 0) ? Expression.Block(typeof(double), variablesToCreate, runCode) : Expression.Block(runCode);
            while (body.CanReduce)
                body = body.Reduce();
            return body;
        }

        protected abstract Expression GenerateAssign(ParameterExpression nameVariableX, ParameterExpression nameVariableY);

        protected Variable Variable { get; set; }
        public string Name { get; private set;}
        private string _fullName = null;
        public string FullName
        {
            get
            {
                if (_fullName == null)
                    _fullName = (Variable.Name + "_" + Name).ToLowerInvariant();
                return _fullName;
            }
        }
    }
}
