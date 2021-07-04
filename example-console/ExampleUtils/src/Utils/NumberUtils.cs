using System;
using System.Linq;
using System.Text;

namespace ExampleUtils.Utils
{
    /// <summary>
    /// 数値ユーティリティ
    /// </summary>
    public static class NumberUtils
    {
        /// <summary>
        /// ゾーン10進数: ゾーン部(ASCIIコード使用時)
        /// </summary>
        private const char ZonedDecAsciiZonePart = '3'; // 0b_0011

        /// <summary>
        /// ゾーン10進数: 符号部 符号なし・正(ASCIIコード使用時)
        /// </summary>
        private const char ZonedDecAsciiSignPartPlus = '3'; // 0b_0011

        /// <summary>
        /// ゾーン10進数: 符号部 負(ASCIIコード使用時)
        /// </summary>
        private const char ZonedDecAsciiSignPartMinus = '7'; // 0b_0111

        /// <summary>
        /// パック10進数: 符号部 正(ASCIIコード使用時)
        /// </summary>
        private const char PackedDecSignPartPlus = 'C'; // 0b_1100

        /// <summary>
        /// パック10進数: 符号部 負(ASCIIコード使用時)
        /// </summary>
        private const char PackedDecSignPartMinus = 'D'; // 0b_1101

        /// <summary>
        /// 整数からゾーン10進数を生成する。
        /// </summary>
        /// <param name="num">整数</param>
        /// <returns>ゾーン10進数</returns>
        public static byte[] CreateZonedDecimalBytes(long num)
        {
            var isPositive = num >= 0;
            var numstr = num.ToString().TrimStart('-'); // -123 => 123
            var sb = new StringBuilder();

            // 最後を除く16進数2桁の生成
            foreach (var c in numstr[0..^1])
                sb.Append(ZonedDecAsciiZonePart).Append(c);

            // 最後の16進数2桁の生成
            var sign = isPositive ? ZonedDecAsciiSignPartPlus : ZonedDecAsciiSignPartMinus;
            sb.Append(sign).Append(numstr[^1]);

            // 文字列の各2桁を16進数としてバイトに変換する。
            var str = sb.ToString();
            return Enumerable.Range(0, str.Length)
                .Where(i => i % 2 == 0)
                .Select(i => Convert.ToByte(str.Substring(i, 2), 16))
                .ToArray();
        }

        /// <summary>
        /// ゾーン10進数から整数を生成する。
        /// </summary>
        /// <param name="bytes">ゾーン10進数</param>
        /// <returns>整数</returns>
        public static long ParseZonedDecimal(byte[] bytes)
        {
            var hexstr = BitConverter.ToString(bytes).Replace("-", ""); // 1F-34 => 1F34
            var sb = new StringBuilder();
            for (var i = 0; i < hexstr.Length; i+=2)
            {
                // 上位4ビット(ゾーン部or符号部)の検証
                var u4 = hexstr[i];
                if (i < hexstr.Length - 2)
                {
                    // 最終バイト以外は期待するゾーン部であることを検証
                    if (u4 != ZonedDecAsciiZonePart)
                        throw new ArgumentException($"不正なゾーン部: {hexstr}");
                }
                else
                {
                    // 最終バイトは符号部であることを検証
                    if (u4 == ZonedDecAsciiSignPartMinus)
                        sb.Insert(0, "-");
                    else if (u4 != ZonedDecAsciiSignPartPlus)
                        throw new ArgumentException($"不正な符号部: {hexstr}");
                }

                // 下位4ビットは数値(0-9)であることを検証
                var l4 = hexstr[i + 1];
                if (l4 < '0' || '9' < l4)
                    throw new ArgumentException($"不正な数値部: {hexstr}");

                sb.Append(l4);
            }
            return long.Parse(sb.ToString()); // OverflowExceptionの場合あり
        }

        /// <summary>
        /// 整数からパック10進数を生成する。
        /// </summary>
        /// <param name="num">整数</param>
        /// <returns>パック10進数</returns>
        public static byte[] CreatePackedDecimalBytes(long num)
        {
            var isPositive = num >= 0;
            var sb = new StringBuilder(num.ToString().TrimStart('-')); // -123 => 123

            // 偶数桁の16進数文字列になるよう0を追加
            // (後で符号部1桁を追加するので、ここでは奇数である必要がある。)
            if (sb.Length % 2 == 0) sb.Insert(0, '0');

            // 符号部1桁を追加(16進数)
            sb.Append(isPositive ? PackedDecSignPartPlus : PackedDecSignPartMinus);

            // 文字列の各2桁を16進数としてバイトに変換する。
            var str = sb.ToString();
            return Enumerable.Range(0, str.Length)
                .Where(i => i % 2 == 0)
                .Select(i => Convert.ToByte(str.Substring(i, 2), 16))
                .ToArray();
        }

        /// <summary>
        /// パック10進数から整数を生成する。
        /// </summary>
        /// <param name="bytes">パック10進数</param>
        /// <returns>整数</returns>
        public static long ParsePackedDecimal(byte[] bytes)
        {
            // 16進数文字列を生成
            var hexes = BitConverter.ToString(bytes).Replace("-", ""); // 1F-34 => 1F34

            // 数値以外(a-f)が含まれないことを検証
            var numstr = hexes[0..^1];
            foreach (var c in numstr)
                if (c < '0' || '9' < c) throw new ArgumentException($"不正な数値部: {hexes}");

            // 符号部が想定される符号値であることを検証
            var signstr = hexes[^1];
            if (signstr == PackedDecSignPartMinus)
                numstr = "-" + numstr;
            else if (signstr != PackedDecSignPartPlus)
                throw new ArgumentException($"不正な符号部: {hexes}");

            return long.Parse(numstr); // OverflowExceptionの場合あり
        }
    }

}
