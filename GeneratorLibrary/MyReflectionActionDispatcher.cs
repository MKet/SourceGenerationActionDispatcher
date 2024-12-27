using System;
using System.Collections.Generic;
using System.Globalization;
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

        public string Dispatch(string actionName, params string[] args)
        {
            var action = actions.FirstOrDefault(act => act.Name == actionName);

            if (action != null)
            {
                var parameters = action.GetParameters();

                if (parameters.Length != args.Length)
                {
                    throw new ArgumentException($"Expected {parameters.Length} arguments for {actionName}, but got {args.Length}.");
                }


                var convertedParameters = parameters.Select((p, i) => ConvertValue(args[i], p.ParameterType)).ToArray();
                return action.Invoke(null, convertedParameters).ToString();
            }
            else
            {
                throw new ArgumentException($"No action found for: {actionName}");
            }
        }

        /// <summary>
        /// Converts the type of a string value to the given <see cref="Type"/>.
        /// </summary>
        /// <param name="input">The string that needs conversion.</param>
        /// <param name="type">The <see cref="Type"/> the string needs be converted to.</param>
        /// <returns></returns>
        private static object ConvertValue(string input, Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(decimal))
            {
                return Convert.ToDecimal(input, CultureInfo.InvariantCulture);
            }
            if (type == typeof(double))
            {
                return Convert.ToDouble(input, CultureInfo.InvariantCulture);
            }
            if (type == typeof(DateTime))
            {
                return Convert.ToDateTime(input, CultureInfo.InvariantCulture);
            }

            return Convert.ChangeType(input, type);
        }
    }
}
