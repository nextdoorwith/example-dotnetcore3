using System;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace BasicExample.EncodingExamples
{
    public class CodepointExamples
    {

        [Theory]
        [InlineData("U+0041", "A")]
        [InlineData("U+3042", "あ")]
        [InlineData("U+2123D", "𡈽")] // サロゲートペア
        [InlineData("U+0041U+3042U+2123D", "Aあ𡈽")]
        public void Test_GetCodePointsFromString(string expected, string test)
        {
            Assert.Equal(expected, GetCodePointsFromString(test));
        }

        [Theory]
        [InlineData("A", "U+0041")]
        [InlineData("あ", "U+3042")]
        [InlineData("𡈽", "U+2123D")] // サロゲートペア
        [InlineData("Aあ𡈽", "U+0041U+3042U+2123D")]
        public void Test_GetStringFromCodePoints(string expected, string test)
        {
            Assert.Equal(expected, GetStringFromCodePoints(test));
        }

        public string GetCodePointsFromString(string str)
        {
            var sb = new StringBuilder();
            var chars = str.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                int codepoint;
                if (Char.IsSurrogate(chars[i]))
                {
                    // TODO: 2文字目有無やサロゲートペアかの検証が必要
                    codepoint = Char.ConvertToUtf32(chars[i], chars[i + 1]);
                    i++;
                }
                else
                {
                    codepoint = (int)chars[i];
                }
                var codepointStr = "U+" + codepoint.ToString("X4"); // 4桁～5桁
                sb.Append(codepointStr);
            }
            return sb.ToString();
        }

        public string GetStringFromCodePoints(string codepoints)
        {
            var hexes = Regex.Split(codepoints, "U\\+", RegexOptions.IgnoreCase);

            var sb = new StringBuilder();
            foreach (var hex in hexes)
            {
                if (string.IsNullOrEmpty(hex)) continue;
                var codepoint = Convert.ToInt32(hex, 16);
                var chstr = Char.ConvertFromUtf32(codepoint);
                sb.Append(chstr);
            }
            return sb.ToString();
        }

    }
}
