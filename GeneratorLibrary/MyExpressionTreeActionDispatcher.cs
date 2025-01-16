using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Reflection;

namespace ClassLibrary
{
    public class MyExpressionTreeActionDispatcher : IActionDispatcher
    {
        private readonly FrozenDictionary<string, Func<Dictionary<string, string>, string>> actions;

        public MyExpressionTreeActionDispatcher()
        {
            actions = typeof(MyClass).GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .Where(m => m.GetCustomAttributes(typeof(MyActionAttribute), false).Length != 0)
                        .ToFrozenDictionary(
                            m => m.Name,
                            BuildActionInvoker
                        );
        }

        public string Dispatch(string actionName, Dictionary<string, string> args)
        {
            if (!actions.TryGetValue(actionName, out var action))
                throw new ArgumentException($"No action found for: {actionName}");

            return action(args);
        }

        private static Func<Dictionary<string, string>, string> BuildActionInvoker(MethodInfo method)
        {
            // The dictionary parameter to the lambda
            var argsParam = Expression.Parameter(typeof(Dictionary<string, string>), "args");

            // Array to hold converted parameters
            var parameters = method.GetParameters();
            var convertedParameters = new Expression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramInfo = parameters[i];

                // args["paramName"]
                var keyAccess = Expression.Constant(paramInfo.Name);
                var dictionaryAccess = Expression.Property(argsParam, "Item", keyAccess);

                // Convert value from string to the target type
                var targetType = paramInfo.ParameterType;
                var convertedValue = BuildConversionExpression(dictionaryAccess, targetType);

                // Handle optional parameters with default values
                if (paramInfo.HasDefaultValue)
                {
                    var defaultValue = Expression.Constant(paramInfo.DefaultValue, targetType);
                    convertedValue = Expression.Condition(
                        Expression.Call(argsParam, typeof(Dictionary<string, string>).GetMethod("ContainsKey")!, keyAccess),
                        convertedValue,
                        defaultValue
                    );
                }

                convertedParameters[i] = convertedValue;
            }

            // Method call: MyClass.Method(convertedParameters)
            var methodCall = Expression.Call(method, convertedParameters);

            // Wrap the result as a string
            var result = Expression.Convert(methodCall, typeof(object));
            var toStringCall = Expression.Call(result, typeof(object).GetMethod("ToString")!);

            // Compile the expression into a delegate
            return Expression.Lambda<Func<Dictionary<string, string>, string>>(toStringCall, argsParam).Compile();
        }

        private static Expression BuildConversionExpression(Expression valueExpression, Type targetType)
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Conversion logic
            if (underlyingType == typeof(int))
                return Expression.Call(typeof(Convert).GetMethod(nameof(Convert.ToInt32), new[] { typeof(string) })!, valueExpression);

            if (underlyingType == typeof(double))
                return Expression.Call(typeof(Convert).GetMethod(nameof(Convert.ToDouble), new[] { typeof(string) })!, valueExpression);

            if (underlyingType == typeof(decimal))
                return Expression.Call(typeof(Convert).GetMethod(nameof(Convert.ToDecimal), new[] { typeof(string) })!, valueExpression);

            if (underlyingType == typeof(DateTime))
                return Expression.Call(typeof(Convert).GetMethod(nameof(Convert.ToDateTime), new[] { typeof(string) })!, valueExpression);

            if (underlyingType.IsEnum)
                return Expression.Convert(
                    Expression.Call(typeof(Enum).GetMethod(nameof(Enum.Parse), new[] { typeof(Type), typeof(string) })!,
                        Expression.Constant(underlyingType),
                        valueExpression
                    ),
                    underlyingType
                );

            // Default conversion
            var generalConvertCall = Expression.Call(typeof(Convert).GetMethod(nameof(Convert.ChangeType), new[] { typeof(object), typeof(Type) })!,
                Expression.Convert(valueExpression, typeof(object)),
                Expression.Constant(underlyingType)
            );

            return Expression.Convert(generalConvertCall, underlyingType);
        }
    }
}
