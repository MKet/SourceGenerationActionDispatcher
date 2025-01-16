// See https://aka.ms/new-console-template for more information

using ClassLibrary;

Dictionary<string, Dictionary<string, string>> testData = new()
{
    // Methods with string inputs
    { "Raw", new Dictionary<string, string> { { "input", "SampleInput" } } },
    { "HtmlEncode", new Dictionary<string, string> { { "input", "<div>Hello & welcome!</div>" } } },
    { "UrlEncode", new Dictionary<string, string> { { "input", "https://example.com/test?query=value" } } },
    { "UrlDecode", new Dictionary<string, string> { { "input", "https%3A%2F%2Fexample.com%2Ftest%3Fquery%3Dvalue" } } },
    { "CutString", new Dictionary<string, string>
        {
            { "input", "This is a test string." },
            { "maxLength", "10" },
            { "suffix", "..." }
        }
    },
    { "JsonSafe", new Dictionary<string, string> { { "input", "\"Quote\" and \\backslash\\" } } },
    { "StripHtml", new Dictionary<string, string> { { "input", "<p>This is <b>bold</b>.</p>" } } },
    { "StripInlineStyle", new Dictionary<string, string> { { "input", "<div style=\"color:red;\">Styled text</div>" } } },
    { "Base64", new Dictionary<string, string> { { "input", "Encode this string to Base64" } } },
    { "UppercaseFirst", new Dictionary<string, string> { { "input", "capitalize" } } },
    { "LowercaseFirst", new Dictionary<string, string> { { "input", "CAPITALIZE" } } },
    { "Uppercase", new Dictionary<string, string>
        {
            { "input", "to uppercase" },
            { "useInvariantCulture", "false" }
        }
    },
    { "Lowercase", new Dictionary<string, string>
        {
            { "input", "TO LOWERCASE" },
            { "useInvariantCulture", "true" }
        }
    },
    { "Replace", new Dictionary<string, string>
        {
            { "input", "Hello, world!" },
            { "oldValue", "world" },
            { "newValue", "C#" }
        }
    },
    { "QrCode", new Dictionary<string, string>
        {
            { "input", "https://example.com" },
            { "width", "150" },
            { "height", "150" }
        }
    },

    // Methods with numeric inputs
    { "DoSomething", new Dictionary<string, string>
        {
            { "message", "Hello" },
            { "count", "5" }
        }
    },
    { "DoSomethingElse", new Dictionary<string, string> { { "value", "3.14" } } },
    { "FormatNumber", new Dictionary<string, string>
        {
            { "input", "12345.6789" },
            { "numberFormat", "N3" },
            { "cultureName", "en-US" }
        }
    },
    { "Currency", new Dictionary<string, string>
        {
            { "input", "1234.56" },
            { "includeCurrencySymbol", "true" },
            { "cultureName", "en-GB" }
        }
    },
    { "CurrencySup", new Dictionary<string, string>
        {
            { "input", "9876.54" },
            { "includeCurrencySymbol", "false" },
            { "cultureName", "nl-NL" }
        }
    },

    // Methods with DateTime inputs
    { "DateTime", new Dictionary<string, string>
        {
            { "input", "2024-12-31T23:59:59" },
            { "format", "yyyy-MM-dd HH:mm:ss" },
            { "culture", "en-US" }
        }
    }
};

var generatedDispatcher = new MyExpressionTreeActionDispatcher();

foreach (var pair in testData)
{
    Console.WriteLine(generatedDispatcher.Dispatch(pair.Key, pair.Value));
}

Console.ReadKey(); 