﻿using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace ClassLibrary
{
    public static partial class MyClass
    {

        /// <summary>
        /// This just returns the input as is. This can be used to bypass the default HtmlEncode formatter if needed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string Raw(this string input)
        {
            return input;
        }

        /// <summary>
        /// Encodes special characters to make the string safe for HTML.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string HtmlEncode(this string input)
        {
            return String.IsNullOrEmpty(input) ? input : System.Text.Encodings.Web.HtmlEncoder.Default.Encode(input);
        }

        /// <summary>
        /// Encodes the string to make it URL-safe. The string is escaped according to RFC 2396.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string UrlEncode(this string input)
        {
            return input == null ? null : Uri.EscapeDataString(input);
        }

        /// <summary>
        /// Decodes the string from an URL-safe string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string UrlDecode(this string input)
        {
            return input == null ? null : Uri.UnescapeDataString(input);
        }

        /// <summary>
        /// Cuts a string to a maximum amount of characters.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLength"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        [MyAction]
        public static string CutString(this string input, int maxLength, string suffix = "")
        {
            if (String.IsNullOrEmpty(input) || input.Length <= maxLength)
            {
                return input;
            }

            if (String.IsNullOrEmpty(suffix))
            {
                return input[..maxLength];
            }

            if (suffix.Length > maxLength)
            {
                throw new ArgumentException($"The length of the suffix cannot exceed {nameof(maxLength)}.", nameof(suffix));
            }

            var output = new StringBuilder(input[..(maxLength - suffix.Length)]);
            output.Append(suffix);

            return output.ToString();
        }

        /// <summary>
        /// Converts a decimal to a numeric representation using the given number format and culture.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="numberFormat">The number format. Defaults to N2.</param>
        /// <param name="cultureName">The culture name. Defaults to nl-NL.</param>
        /// <returns></returns>
        [MyAction]
        public static string FormatNumber(this decimal input, string numberFormat = "N2", string cultureName = "nl-NL")
        {
            return input.ToString(numberFormat, CultureInfo.CreateSpecificCulture(cultureName));
        }

        /// <summary>
        /// Converts a <see cref="decimal"/> to a currency representation. This will the use default culture of the application instance.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeCurrencySymbol"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        [MyAction]
        public static string Currency(this decimal input, bool includeCurrencySymbol = true, string cultureName = null)
        {
            var culture = !String.IsNullOrWhiteSpace(cultureName) ? new CultureInfo(cultureName) : CultureInfo.CurrentCulture;
            var output = input.ToString(includeCurrencySymbol ? "C" : $"N{culture.NumberFormat.CurrencyDecimalDigits}", culture);

            if (culture.Name.Equals("nl", StringComparison.OrdinalIgnoreCase) || culture.Name.Equals("nl-NL", StringComparison.OrdinalIgnoreCase))
            {
                output = output.Replace(",00", ",-");
            }

            return output;
        }

        /// <summary>
        /// Converts a <see cref="decimal"/> to a currency representation, and will wrap the decimal digits inside a &lt;sup&gt; tag. This will the use default culture of the application instance.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="includeCurrencySymbol"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        [MyAction]
        public static string CurrencySup(this decimal input, bool includeCurrencySymbol = true, string cultureName = null)
        {
            var culture = !String.IsNullOrWhiteSpace(cultureName) ? new CultureInfo(cultureName) : CultureInfo.CurrentCulture;
            var output = input.ToString(includeCurrencySymbol ? "C" : $"N{culture.NumberFormat.CurrencyDecimalDigits}", culture);

            var decimalSeparator = culture.NumberFormat.CurrencyDecimalSeparator;
            var outputParts = output.Split(decimalSeparator);

            return $"{outputParts[0]}{decimalSeparator}<sup>{outputParts[1]}</sup>";
        }

        /// <summary>
        /// Returns a string with the first character in uppercase.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string UppercaseFirst(this string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            // Only one character; simply return the string in uppercase.
            if (input.Length == 1)
            {
                return input.ToUpper();
            }

            // Use a StringBuilder to optimize string manipulations.
            var stringBuilder = new StringBuilder(input);
            stringBuilder.Remove(0, 1);
            stringBuilder.Insert(0, input[0].ToString().ToUpper());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns a string with the first character in lowercase.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string LowercaseFirst(this string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            // Only one character; simply return the string in lowercase.
            if (input.Length == 1)
            {
                return input.ToLower();
            }

            // Use a StringBuilder to optimize string manipulations.
            var stringBuilder = new StringBuilder(input);
            stringBuilder.Remove(0, 1);
            stringBuilder.Insert(0, input[0].ToString().ToLower());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Strips HTML tags to leave plain text. Any script tags are removed entirely, including the content.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string StripHtml(this string input)
        {
            var tagRegex = TagRegex();
            var scriptRegex = ScriptRegex();
            return tagRegex.Replace(scriptRegex.Replace(input, ""), "");
        }

        /// <summary>
        /// Makes the string safe to be used in a JSON string by escaping all quotes.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string JsonSafe(this string input)
        {
            return String.IsNullOrEmpty(input) ? input : HttpUtility.JavaScriptStringEncode(input);
        }

        /// <summary>
        /// Strips the "style=..." attribute from an HTML string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string StripInlineStyle(this string input)
        {
            var regex = new Regex(" ?style=\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(2000));
            return String.IsNullOrEmpty(input) ? input : regex.Replace(input, "");
        }

        /// <summary>
        /// Converts a string value to its Base64 equivalent.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [MyAction]
        public static string Base64(this string input)
        {
            return input == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }

        /// <summary>
        /// Converts a date time to a string with the specified format.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="format">The format of the <see cref="DateTime"/>. Can be a default format or a custom format.</param>
        /// <param name="culture">The culture to format the <see cref="DateTime"/> with.</param>
        /// <returns></returns>
        [MyAction]
        public static string DateTime(this DateTime input, string format, string culture = null)
        {
            if (String.IsNullOrWhiteSpace(culture))
            {
                return input.ToString(format);
            }
            return input.ToString(format, new CultureInfo(culture));
        }

        /// <summary>
        /// Converts this string to uppercase.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="useInvariantCulture">Whether to use the casing rules of the invariant culture instead of those of the current culture.</param>
        /// <returns>The string converted to uppercase.</returns>
        [MyAction]
        public static string Uppercase(this string input, bool useInvariantCulture = false)
        {
            return useInvariantCulture ? input?.ToUpperInvariant() : input?.ToUpper();
        }

        /// <summary>
        /// Replace occurances of the given string value with the new value
        /// </summary>
        /// <param name="input"></param>
        /// <param name="oldValue">The value to be replaced</param>
        /// <param name="newValue">The value to replace the old value with</param>
        /// <returns>New string with replaced values</returns>
        [MyAction]
        public static string Replace(this string input, string oldValue, string newValue = "")
        {
            return input.Replace(oldValue, newValue);
        }

        /// <summary>
        /// Converts this string to lowercase.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="useInvariantCulture">Whether to use the casing rules of the invariant culture instead of those of the current culture.</param>
        /// <returns>The string converted to lowercase.</returns>
        [MyAction]
        public static string Lowercase(this string input, bool useInvariantCulture = false)
        {
            return useInvariantCulture ? input?.ToLowerInvariant() : input?.ToLower();
        }

        /// <summary>
        /// Converts an input string to an image URL. Note that the URL is always relative, starting with a '<c>/</c>'.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>The image URL.</returns>
        [MyAction]
        public static string QrCode(this string input, int width, int height)
        {
            if (String.IsNullOrWhiteSpace(input) || width <= 0 || height <= 0)
            {
                return String.Empty;
            }

            var url = $"/barcodes/generate?input={Uri.EscapeDataString(input)}&format=qr_code&width={width}&height={height}";

            return url;
        }

        [MyAction]
        public static string DoSomething(string message = "Default", int count = 1)
        {
            return $"{message} was repeated {count} times.";
        }

        [MyAction]
        public static string DoSomethingElse(float value = 0f)
        {
            return $"Value is {value}";
        }

        [GeneratedRegex("<script.*?/script>", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-NL")]
        private static partial Regex ScriptRegex();

        [GeneratedRegex("<(.|\\n)*?>", RegexOptions.IgnoreCase, "en-NL")]
        private static partial Regex TagRegex();
    }
}
