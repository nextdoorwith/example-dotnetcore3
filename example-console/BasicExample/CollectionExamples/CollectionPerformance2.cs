using BasicExample.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace BasicExample.CollectionExamples
{
    public class CollectionPerformance2 : BaseTests
    {
        /// <summary>
        /// 同一測定の実行回数
        /// </summary>
        /// <remarks>複数回測定した平均時間を採用</remarks>
        private const int RepeatCount = 3;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="testOutputHelper"></param>
        public CollectionPerformance2(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper) { }

        [Theory(DisplayName = "Contains性能比較(列挙体)")]
        [InlineData(0_000_001_000)]
        [InlineData(0_000_010_000)]
        [InlineData(0_000_100_000)]
        [InlineData(0_001_000_000)]
        [InlineData(0_010_000_000)]
        [InlineData(0_100_000_000)]
        //[InlineData(1_000_000_000)] // 時間がかかりすぎるのでスキップ
        public void CompareEnumContains(long bucketSize)
        {
            // (Enum版)コレクションに格納するキーと検索するキー
            var keys = new int[] { 0, 2, 4, 5, 7, 9 }.Select(i => (TestValue)i).ToList();
            var scans = CreateRandomEnumList(
                new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, bucketSize);
            // (int版)コレクションに格納するキーと検索するキー
            var intkeys = keys.Select(e => (int)e).ToList();
            var intScans = scans.Select(e => (int)e).ToList();

            // 列挙体HashSetの評価
            var enumHashSet = new HashSet<TestValue>(keys);
            ScanEnumerable(enumHashSet, scans, "HashSet<TestValue>");
            // 整数型に変換したHashSetの評価
            var intHashSet = new HashSet<int>(intkeys);
            ScanEnumerable(intHashSet, intScans, "HashSet<int>");

            // 列挙体Dictionaryの評価
            var enumDic = keys.ToDictionary(p => p, p => (object)null /* dummy */);
            ScanDictionary(enumDic, scans, "Dictionary<TestValue, object>");
            // 整数型に変換したDictionaryの評価
            var intDic = intkeys.ToDictionary(p => p, p => (object)null /* dummy */);
            ScanDictionary(intDic, intScans, "Dictionary<int, object>");
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
        /// ランダムな列挙体値を生成する。
        /// </summary>
        /// <param name="ints">使用する列挙体値</param>
        /// <param name="count">作成する列挙体値の数</param>
        /// <returns>列挙体値リスト</returns>
        public List<TestValue> CreateRandomEnumList(int[] ints, long count)
        {
            // 整数リストを列挙体リストに変換
            var enums = ints.Select(i => (TestValue)i).ToArray();

            // ランダムな列挙体のリストを生成
            var random = new Random();
            var list = new List<TestValue>();
            for (var i = 0; i < count; i++)
                list.Add(enums[random.Next(0, enums.Length)]);
            return list;
        }

        /// <summary>
        /// テスト列挙体
        /// </summary>
        public enum TestValue
        {
            E1, E2, E3, E4, E5, E6, E7, E8, E9, E10
        }
    }
}
