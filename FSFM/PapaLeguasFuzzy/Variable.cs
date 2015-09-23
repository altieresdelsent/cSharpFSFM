using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace PapaLeguasFuzzy
{
    public class Variable : IVariable
    {
        public Variable(string name)
        {
            this.Name = name.ToLowerInvariant();
            this.Terms = new List<ITerm>();
        }
        public Variable(string name, List<Variable> variables)
        {
            this.Name = name.ToLowerInvariant();
            this.Terms = new List<ITerm>();
            if (variables.IndexOf(this) < 0)
                variables.Add(this);
        }
        public string Name { get; private set; }

        public List<ITerm> Terms { get; set; }
    }
}
