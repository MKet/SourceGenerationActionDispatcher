using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ClassLibrary
{
    public class MyReflectionActionDispatcher : IActionDispatcher
    {
        private readonly FrozenDictionary<string, MethodInfo> actions;

        public MyReflectionActionDispatcher()
        {
            actions = typeof(MyClass).GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(m => m.GetCustomAttributes(typeof(MyActionAttribute), false).Length != 0)
                        .ToFrozenDictionary(m => m.Name);
        }

        public string Dispatch(string actionName, Dictionary<string, string> args)
        {
            if (!actions.TryGetValue(actionName, out var action))
                throw new ArgumentException($"No action found for: {actionName}");

            var parameters = action.GetParameters();
            if (parameters.Length != args.Count)
                throw new ArgumentException($"Expected {parameters.Length} arguments for {actionName}, but got {args.Count}.");

            var convertedParameters = ConvertParameters(args, parameters);
            return action.Invoke(null, convertedParameters)?.ToString();
        }


        private static object[] ConvertParameters(Dictionary<string, string> args, ParameterInfo[] parameters)
        {
            var converted = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                if (args.TryGetValue(parameter.Name, out var value)) 
                {
                    converted[i] = ConvertValue(value, parameter.ParameterType);
                } 
                else if (parameter.HasDefaultValue)
                {
                    converted[i] = parameter.DefaultValue;
                } 
                else
                {
                    throw new ArgumentException($"Missing required argument: {parameter.Name}");
                }
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
