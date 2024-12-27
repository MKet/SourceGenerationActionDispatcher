using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using ClassLibrary;

namespace ActionDispatcherBenchmark
{
    [SimpleJob(RunStrategy.Throughput, iterationCount: 5)]
    public class GeneratedVsReflection
    {
        private const int iterationCount = 100_000_000;
        private readonly string[] actionNames = 
            [
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
            ];

        private readonly Random random;

        private readonly Dictionary<string, string[]> testData = new()
        {
            // Methods with string inputs
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

            // Methods with numeric inputs
            { "FormatNumber", new[] { "12345.6789", "N3", "en-US" } },
            { "Currency", new[] { "1234.56", "true", "en-GB" } },
            { "CurrencySup", new[] { "9876.54", "false", "nl-NL" } },

            // Methods with DateTime inputs
            { "DateTime", new[] { "2024-12-31T23:59:59", "yyyy-MM-dd HH:mm:ss", "en-US" } }
        };

        private MyGeneratedActionDispatcher? generatedDispatcher;
        private MyReflectionActionDispatcher? reflectionDispatcher;

        public GeneratedVsReflection()
        {
            random = new Random(42);

        }

        [Benchmark]
        public void Generated()
        {
            generatedDispatcher = new MyGeneratedActionDispatcher();
            for (int i = 0; i < iterationCount; i++)
            {
                string name = actionNames[random.Next(0, actionNames.Length)];
                string[] parameters = testData[name];
                var _ = generatedDispatcher.Dispatch(name, parameters);
            }

        }

        [Benchmark]
        public void Reflection()
        {
            reflectionDispatcher = new MyReflectionActionDispatcher();
            for (int i = 0; i < iterationCount; i++)
            {
                string name = actionNames[random.Next(0, actionNames.Length)];
                string[] parameters = testData[name];
                var _ = reflectionDispatcher.Dispatch(name, parameters);
            }
        }
    }

    public class Program
    {
        public static void Main()
        {
            _ = BenchmarkRunner.Run<GeneratedVsReflection>();
        }
    }
}
