using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace PapaLeguasFuzzy
{
    public interface IVariable
    {
        string Name{get;}
        List<ITerm> Terms{get; set;}
    }
}
