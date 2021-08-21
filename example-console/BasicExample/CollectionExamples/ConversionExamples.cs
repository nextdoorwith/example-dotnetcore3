using BasicExample.Misc;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace BasicExample.CollectionExamples
{
    public class ConversionExamples : BaseTests
    {
        public ConversionExamples(ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName = "コレクション変換")]
        public void Example1()
        {
            // 変換元
            string[] strs = new string[] { "1", "2", "3", "4" };

            // string[] -> List<string> 変換
            var strList = strs.ToList();
            Console.WriteLine("strList: {0}", Dump(strList));

            // string[] -> List<int> 変換
            var intList = strs.Select(s => int.Parse(s)).ToList();
            Console.WriteLine("intList: {0}", Dump(intList));

            // string[] -> List<Person> 変換
            var personList = strs.Select(s => new Person(s)).ToList();
            Console.WriteLine("personList: {0}", Dump(personList));

            // 変則的な変換
            // {"1", "2", "3", "4"} -> {"12", 34"}
            var exList = 
                Enumerable.Range(0, strs.Length)    // 連番リストの生成
                .Where(i => i % 2 == 0)             // 一つ飛ばし(奇数番目のみ処理)
                .Select(i => strs[i] + strs[i + 1]) // 後ろの文字と結合
                .ToList();
            Console.WriteLine("exList: {0}", Dump(exList));

            // string[] -> HashSet<string> 変換
            var set = strs.ToHashSet();
            Console.WriteLine("set: {0}", Dump(set));


            // string[] -> Dictionary<string, string> 変換
            // ※ToDictionaryの引数はstrsの基型string
            var dic = strs.ToDictionary(k => "key" + k, v => "value" + v);
            Console.WriteLine("dic: {0}", Dump(dic));

            // Dictionaryのキーと値をリスト化
            var keyList = dic.Keys.ToList();
            Console.WriteLine("keyList: {0}", Dump(keyList));
            var valueList = dic.Values.ToList();
            Console.WriteLine("valueList: {0}", Dump(valueList));

            // Dictionary間の変換
            // Dictionary<string, string> -> Dictionary<string, Person> 変換
            // ※ToDictionaryの引数はdicの基型KeyValuPair<string, string>
            var personDic = dic.ToDictionary(kv => kv.Key, kv => new Person(kv.Value));
            Console.WriteLine("personDic: {0}", Dump(personDic));
        }

        private string Dump<T>(IEnumerable<T> enums)
            => string.Join(',', enums);

        private string Dump<K, V>(Dictionary<K, V> dic)
            => Dump(dic.Select(p => $"{p.Key}={p.Value}").ToArray());

        class Person
        {
            public string Id { get; set; }
            public Person(string id) => Id = id;
            public override string ToString() => $"Person{{Id={Id}}}";
        }
    }
}
