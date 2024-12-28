using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ClassLibrary
{
    public class MyReflectionActionDispatcher : IActionDispatcher
    {
        private readonly Dictionary<string, MethodInfo> actions;

        public MyReflectionActionDispatcher()
        {
            actions = typeof(MyClass).GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(m => m.GetCustomAttributes(typeof(MyActionAttribute), false).Length != 0)
                        .ToDictionary(m => m.Name);
        }

        public string Dispatch(string actionName, params string[] args)
        {
            if (!actions.TryGetValue(actionName, out var action))
                throw new ArgumentException($"No action found for: {actionName}");

            var parameters = action.GetParameters();
            if (parameters.Length != args.Length)
                throw new ArgumentException($"Expected {parameters.Length} arguments for {actionName}, but got {args.Length}.");

            var convertedParameters = ConvertParameters(args, parameters);
            return action.Invoke(null, convertedParameters)?.ToString();
        }


        private static object[] ConvertParameters(string[] args, ParameterInfo[] parameters)
        {
            var converted = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameterType = parameters[i].ParameterType;
                converted[i] = ConvertValue(args[i], parameterType);
            }

            return converted;
        }

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
