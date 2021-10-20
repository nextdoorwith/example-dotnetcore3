using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CrnJsonConveter
{
    class Program
    {
        // HTML5でサポートする実体文字参照名
        // https://html.spec.whatwg.org/multipage/named-characters.html#named-character-references
        private const string JsonFileName = @"Download\entities.json"; // 約200KB

        static void Main(string[] args)
        {
            OutputAsTsv(JsonFileName, @"CharacterReference_HTML5.txt");
            OutputUniqueCodepointAsTsv(JsonFileName, @"CharacterReference_HTML5_unique_codepoint.txt");
        }

        /// <summary>
        /// HTML5文字実体参照JSONをタブ区切りファイルに変換する。
        /// </summary>
        /// <param name="inputFilename">JSONファイルパス</param>
        /// <param name="outputFilename">出力先ファイルのパス</param>
        private static void OutputAsTsv(string inputFilename, string outputFilename)
        {
            using var inputStream = File.OpenRead(inputFilename);
            using var outputWriter = new StreamWriter(outputFilename, false);

            int no = 1;
            var document = JsonDocument.Parse(inputStream);
            foreach (var jsonProperty in document.RootElement.EnumerateObject())
            {
                // 文字実体参照名とコードポイント配列の取得
                var cer = jsonProperty.Name;
                var value = jsonProperty.Value;
                var codepoints = value.GetProperty("codepoints")
                    .EnumerateArray().Select(e => e.GetInt32()).ToArray();

                // 出力データの編集
                WriteRecord(outputWriter, no++, codepoints, cer);
            }
        }

        /// <summary>
        /// HTML5文字実体参照JSONをタブ区切りファイルに変換する。(コードポイント値でユニーク化)
        /// </summary>
        /// <param name="inputFilename">JSONファイルパス</param>
        /// <param name="outputFilename">出力先ファイルのパス</param>
        private static void OutputUniqueCodepointAsTsv(string inputFilename, string outputFilename)
        {
            // コードポイント配列・文字実体参照ディクショナリ
            var dic = new Dictionary<int[], string>(new IntArrayEqualityComparer());

            // 重複するコードポイントを除外
            using var inputStream = File.OpenRead(inputFilename);
            var document = JsonDocument.Parse(inputStream);
            foreach (var jsonProperty in document.RootElement.EnumerateObject())
            {
                // 文字実体参照名とコードポイント配列の取得
                var newCer = jsonProperty.Name;
                var value = jsonProperty.Value;
                var codepoints = value.GetProperty("codepoints")
                    .EnumerateArray().Select(e => e.GetInt32()).ToArray();

                if (!dic.ContainsKey(codepoints))
                {
                    dic[codepoints] = newCer;
                    continue;
                }

                // 同一のコードポイントが存在する場合、ルールに基づいて更新
                var oldCer = dic[codepoints];
                var oldCerLower = oldCer.ToLower();
                var newCerLower = newCer.ToLower();

                bool priorNew;
                // "&Xyz;" vs "&xyz" → 左辺を優先
                if (oldCerLower == newCerLower + ";") priorNew = false;
                // "&Xyz" vs "&xyz;" → 右辺を優先
                else if (oldCerLower + ";" == newCerLower) priorNew = true;
                // 大小文字無視で一致の場合、小文字側を優先
                else if (oldCerLower == newCerLower) priorNew = (newCer == newCerLower);
                // 大小文字無視で不一致の場合、桁数が少ない側を優先
                else priorNew = oldCer.Length > newCer.Length;

                // debug
                //Console.WriteLine($"{string.Join(",", codepoints)}: " +
                //    (priorNew ? newCer : oldCer ) + $" <- [ {oldCer} / {newCer} ]");

                if (priorNew) dic[codepoints] = newCer;
            }

            // データ出力
            using var outputWriter = new StreamWriter(outputFilename, false);
            var kvlist = dic.ToList();
            for (var i = 0; i < kvlist.Count; i++)
            {
                var codepoints = kvlist[i].Key;
                var cer = kvlist[i].Value;
                WriteRecord(outputWriter, i + 1, codepoints, cer);
            }
        }

        /// <summary>
        /// 単一レコードを出力する。
        /// </summary>
        /// <param name="writer">出力先ライター</param>
        /// <param name="no">出力項目となるNo.</param>
        /// <param name="codepoints">コードポイント配列</param>
        /// <param name="cer">文字実体参照</param>
        private static void WriteRecord(StreamWriter writer, int no, int[] codepoints, string cer)
        {
            int cp1 = codepoints[0];
            int? cp2 = codepoints.Length > 1 ? codepoints[1] : (int?)null;

            var cp1str = cp1.ToString();
            var cp2str = cp2?.ToString() ?? string.Empty;
            (var ncrDec1, var ncrHex1) = ToNcr(cp1);
            (var ncrDec2, var ncrHex2) = ToNcr(cp2);

            writer.WriteLine($"{no}\t{cer}\t{cp1str}\t{cp2str}" +
                $"\t{ncrDec1}\t{ncrDec2}\t{ncrHex1}\t{ncrHex2}");
        }

        /// <summary>
        /// コードポイント値を数字文字参照(10進数/16進数)に変換する。
        /// </summary>
        /// <param name="codepoint">コードポイント</param>
        /// <returns>10進数と16進数の数字文字参照</returns>
        private static (string dec, string hex) ToNcr(int? codepoint)
        {
            if (!codepoint.HasValue) return (string.Empty, string.Empty);
            return ($"&#{codepoint.Value};" , $"&#x{codepoint.Value:X5};");
        }

        /// <summary>
        /// int配列の等価比較
        /// </summary>
        class IntArrayEqualityComparer : IEqualityComparer<int[]>
        {
            public bool Equals([AllowNull] int[] x, [AllowNull] int[] y)
                => Enumerable.SequenceEqual(x, y);
            public int GetHashCode([DisallowNull] int[] obj)
            {
                var hash = new HashCode();
                foreach (var v in obj) hash.Add(obj[0]);
                return hash.ToHashCode();
            }
        }

    }
}
