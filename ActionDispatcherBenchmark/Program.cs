using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActionDispatcherBenchmark
{
    [MemoryDiagnoser]
    public class GeneratedVsReflection
    {
        private readonly string[] actionNames =
        {
            "Raw",
            "HtmlEncode",
            "UrlEncode",
            "UrlDecode",
            "CutString",
            "JsonSafe",
            "StripHtml",
            "StripInlineStyle",
            "Base64",
            "UppercaseFirst",
            "LowercaseFirst",
            "Uppercase",
            "Lowercase",
            "Replace",
            "QrCode",
            "FormatNumber",
            "Currency",
            "CurrencySup",
            "DateTime"
        };

        private readonly Dictionary<string, string[]> testData = new()
        {
            { "Raw", new[] { "SampleInput" } },
            { "HtmlEncode", new[] { "<div>Hello & welcome!</div>" } },
            { "UrlEncode", new[] { "https://example.com/test?query=value" } },
            { "UrlDecode", new[] { "https%3A%2F%2Fexample.com%2Ftest%3Fquery%3Dvalue" } },
            { "CutString", new[] { "This is a test string.", "10", "..." } },
            { "JsonSafe", new[] { "\"Quote\" and \\backslash\\" } },
            { "StripHtml", new[] { "<p>This is <b>bold</b>.</p>" } },
            { "StripInlineStyle", new[] { "<div style=\"color:red;\">Styled text</div>" } },
            { "Base64", new[] { "Encode this string to Base64" } },
            { "UppercaseFirst", new[] { "capitalize" } },
            { "LowercaseFirst", new[] { "CAPITALIZE" } },
            { "Uppercase", new[] { "to uppercase", "false" } },
            { "Lowercase", new[] { "TO LOWERCASE", "true" } },
            { "Replace", new[] { "Hello, world!", "world", "C#" } },
            { "QrCode", new[] { "https://example.com", "150", "150" } },
            { "FormatNumber", new[] { "12345.6789", "N3", "en-US" } },
            { "Currency", new[] { "1234.56", "true", "en-GB" } },
            { "CurrencySup", new[] { "9876.54", "false", "nl-NL" } },
            { "DateTime", new[] { "2024-12-31T23:59:59", "yyyy-MM-dd HH:mm:ss", "en-US" } }
        };

        private MyGeneratedActionDispatcher? generatedDispatcher;
        private MyReflectionActionDispatcher? reflectionDispatcher;
        private List<(string ActionName, string[] Parameters)>? randomizedCalls;

        [GlobalSetup]
        public void Setup()
        {
            generatedDispatcher = new MyGeneratedActionDispatcher();
            reflectionDispatcher = new MyReflectionActionDispatcher();

            var random = new Random(42);
            randomizedCalls = Enumerable.Range(0, 1000)
                .Select(_ =>
                {
                    var actionName = actionNames[random.Next(actionNames.Length)];
                    var parameters = testData[actionName];
                    return (actionName, parameters);
                })
                .ToList();
        }

        [Benchmark]
        public string Generated()
        {
            string result = string.Empty;
            foreach (var (actionName, parameters) in randomizedCalls!)
            {
                result = generatedDispatcher!.Dispatch(actionName, parameters);
            }
            return result;
        }

        [Benchmark]
        public string Reflection()
        {
            string result = string.Empty;
            foreach (var (actionName, parameters) in randomizedCalls!)
            {
                result = reflectionDispatcher!.Dispatch(actionName, parameters);
            }
            return result;
        }
    }

    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<GeneratedVsReflection>();
        }
    }
}