// See https://aka.ms/new-console-template for more information

using ClassLibrary;

Dictionary<string, string[]> testData = new()
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

var generatedDispatcher = new MyGeneratedActionDispatcher();

foreach (var pair in testData)
{
    Console.WriteLine(generatedDispatcher.Dispatch(pair.Key, pair.Value)); // Output: Hello was repeated 3 times
}

Console.ReadKey(); 