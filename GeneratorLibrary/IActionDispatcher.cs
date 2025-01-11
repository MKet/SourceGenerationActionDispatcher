using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface IActionDispatcher
    {
        string Dispatch(string actionName, Dictionary<string, string> args);
    }
}
