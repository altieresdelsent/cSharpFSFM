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
        public string Name { get; private set; }

        public List<ITerm> Terms { get; set; }
    }
}
