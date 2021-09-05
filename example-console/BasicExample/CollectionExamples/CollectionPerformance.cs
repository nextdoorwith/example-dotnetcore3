using BasicExample.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace BasicExample.CollectionExamples
{
    public class CollectionPerformance : BaseTests
    {
        /// <summary>
        /// 同一測定の実行回数
        /// </summary>
        /// <remarks>複数回測定した平均時間を採用</remarks>
        private const int RepeatCount = 3;

        /// <summary>
        /// キーで使用する文字
        /// </summary>
        private const string KeyChars =
            "0123456789" +
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// キーで使用する文字
        /// </summary>
        private readonly char[] chars = KeyChars.ToCharArray();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="testOutputHelper"></param>
        public CollectionPerformance(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper) { }

        [Theory(DisplayName = "Contains性能比較")]
        [InlineData(0_000_100)]
        [InlineData(0_001_000)]
        [InlineData(0_010_000)]
        [InlineData(0_100_000)]
        // [InlineData(8, 1_000_000)] // 時間がかかりすぎるのでスキップ
        public void CompareContains(long bucketSize)
        {
            // 検索するキー一覧(各テストで共通)
            var keySize = 4;
            var scans = CreateRandomKeyList(keySize, bucketSize);
            var keys = CreateRandomKeyList(keySize, bucketSize);
            scans[scans.Count / 2] = keys[keys.Count / 2]; // 少なくても1件は合致させる

            // Listの評価
            var list = new List<string>(keys);
            ScanEnumerable(list, scans, "List<string>");

            // HashSetの評価
            var enumHashSet = new HashSet<string>(keys);
            ScanEnumerable(enumHashSet, scans, "HashSet<string>");

            // Dictionaryの評価
            var dictionary = keys.ToDictionary(p => p, p => (object)null /* dummy */);
            ScanDictionary(dictionary, scans, "Dictionary<string, object>");
        }

        /// <summary>
        /// IEnumerableを使ったキー検索時間を測定する。
        /// </summary>
        /// <typeparam name="T">検索対象コレクションの型</typeparam>
        /// <param name="enums">検索対象コレクション</param>
        /// <param name="scans">検索するキーのリスト</param>
        /// <param name="prefix">実行結果出力時の接頭文言</param>
        private void ScanEnumerable<T>(IEnumerable<T> enums, List<T> scans, string prefix)
        {
            var watcher = new Stopwatch();
            var elapsed = 0L;
            var hitCount = 0L;
            for (var i = 0; i < RepeatCount; i++)
            {
                GC.Collect(); // テスト前にメモリ状態の初期化を試行

                watcher.Restart();
                foreach (var key in scans) if (enums.Contains(key)) hitCount++;
                watcher.Stop();
                elapsed += watcher.ElapsedMilliseconds;
            }
            var avgElapsed = Math.Ceiling((double)(elapsed / RepeatCount));
            var avgHitCount = Math.Ceiling((double)(hitCount / RepeatCount));
            Console.WriteLine(
                $"{prefix}({enums.Count()}x{scans.Count}) -> elapsed={avgElapsed:#,0}[ms], hit={avgHitCount:#,0}");
        }

        /// <summary>
        /// ディクショナリを使ったキー検索時間を測定する。
        /// </summary>
        /// <typeparam name="K">検索対象ディクショナリのキー型</typeparam>
        /// <typeparam name="V">検索対象ディクショナリのキー型</typeparam>
        /// <param name="dic">検索対象ディクショナリ</param>
        /// <param name="scans">検索するキーのリスト</param>
        /// <param name="prefix">実行結果出力時の接頭文言</param>        
        private void ScanDictionary<K, V>(Dictionary<K, V> dic, List<K> scans, string prefix)
        {
            var watcher = new Stopwatch();
            var elapsed = 0L;
            var hitCount = 0L;
            for (var i = 0; i < RepeatCount; i++)
            {
                GC.Collect(); // テスト前にメモリ状態の初期化を試行

                watcher.Restart();
                foreach (var key in scans) if (dic.ContainsKey(key)) hitCount++;
                watcher.Stop();
                elapsed += watcher.ElapsedMilliseconds;
            }
            var avgElapsed = Math.Ceiling((double)(elapsed / RepeatCount));
            var avgHitCount = Math.Ceiling((double)(hitCount / RepeatCount));
            Console.WriteLine(
                $"{prefix}({dic.Count}x{scans.Count}) -> elapsed={avgElapsed:#,0}[ms], hit={avgHitCount:#,0}");
        }

        /// <summary>
        /// ランダムなキーを生成する。
        /// </summary>
        /// <param name="keySize">作成するキーの長さ</param>
        /// <param name="count">作成するキーの数</param>
        /// <returns></returns>
        private List<string> CreateRandomKeyList(int keySize, long count)
        {
            var keyset = new HashSet<string>();
            var charbuf = new char[keySize];
            var random = new Random();
            for (var i = 0; i < count; )
            {
                // ランダムなキーを生成
                for(var j=0; j<keySize; j++)
                    charbuf[j] = chars[random.Next(0, chars.Length)];
                var key = new string(charbuf);

                // 既存キーと重複する場合は再作成
                if (!keyset.Contains(key))
                {
                    keyset.Add(key);
                    i++;
                }
            }
            return keyset.ToList();
        }
    }
}
