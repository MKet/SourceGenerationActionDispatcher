using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public interface IActionDispatcher
    {
        void Dispatch(string actionName, params string[] args);
    }
}
