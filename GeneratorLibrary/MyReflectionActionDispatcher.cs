using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class MyReflectionActionDispatcher : IActionDispatcher
    {
        private readonly MethodInfo[] actions;

        public MyReflectionActionDispatcher() {
            actions = typeof(MyClass).GetMethods(BindingFlags.Static | BindingFlags.Public)
                            .Where(m => m.GetCustomAttributes(typeof(MyActionAttribute), false).Length != 0).ToArray();
        }

        public void Dispatch(string actionName)
        {
            var action = actions.FirstOrDefault(act => act.Name == actionName);

            if (action != null)
            {
                action.Invoke(null, null);
            } 
            else
            {
                throw new ArgumentException($"No action found for: {actionName}");
            }
        }
    }
}
