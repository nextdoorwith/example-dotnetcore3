using System;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace BasicExample.EncodingExamples
{
    public class UnicodeEscapeExamples
    {

        [Theory]
        [InlineData("\\u0041", "A")]
        [InlineData("\\u3042", "あ")]
        [InlineData("\\U0002123D", "𡈽")] // サロゲートペア
        [InlineData("\\u0041\\u3042\\U0002123D", "Aあ𡈽")]
        public void Test_GetUnicodeEscapeFromString(string expected, string test)
        {
            Assert.Equal(expected, GetUnicodeEscapeFromString(test));
        }

        [Theory]
        [InlineData("A", "\\u0041")]
        [InlineData("あ", "\\u3042")]
        [InlineData("𡈽", "\\U0002123D")] // サロゲートペア
        [InlineData("Aあ𡈽", "\\u0041\\u3042\\U0002123D")]
        public void Test_GetStringFromUnicodeEscape(string expected, string test)
        {
            Assert.Equal(expected, GetStringFromUnicodeEscape(test));
        }

        public string GetUnicodeEscapeFromString(string str)
        {
            var sb = new StringBuilder();
            var chars = str.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (Char.IsSurrogate(chars[i]))
                {
                    int codepoint = Char.ConvertToUtf32(chars[i], chars[i + 1]);
                    var codepointStr = "\\U" + codepoint.ToString("X8"); // C#のユニコードエスケープ表記(UTF-32)
                    sb.Append(codepointStr);
                    i++;
                }
                else
                {
                    int codepoint = (int)chars[i];
                    var codepointStr = "\\u" + codepoint.ToString("X4"); // C#のユニコードエスケープ表記(UTF-16)
                    sb.Append(codepointStr);
                }
            }
            return sb.ToString();
        }

        public string GetStringFromUnicodeEscape(string escapedStr)
        {
            var hexes = Regex.Split(escapedStr, "\\\\u", RegexOptions.IgnoreCase);

            var sb = new StringBuilder();
            foreach (var hex in hexes)
            {
                if (string.IsNullOrEmpty(hex)) continue;
                var codepoint = Convert.ToInt32(hex, 16);
                var ch = Char.ConvertFromUtf32(codepoint);
                sb.Append(ch);
            }
            return sb.ToString();
        }

    }
}
