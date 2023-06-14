using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace BasicExample.XunitExamples
{
    public class AssertExamples
    {
        [Fact(DisplayName = "基本的な検証")]
        public void AssertBasic()
        {
            // 同値
            Assert.Equal(1, 1);
            Assert.Equal(1, int.Parse("1"));
            Assert.Equal("123", "123");
            Assert.Equal("123", 123.ToString());
            Assert.Equal(new string[] { "a", "b" }, "a,b".Split(','));

            // オブジェクト同値
            var obj = new string("test");
            var obj1 = new string("test");
            var obj2 = obj;
            // ※深い比較
            Assert.Equal(obj, obj1);   // 同一インスタンス
            Assert.Equal(obj, obj2);   // 異なるインスタンス(内部値は同一)
            // ※浅い比較
            Assert.NotSame(obj, obj1); // 同一インスタンス
            Assert.Same(obj, obj2);    // 異なるインスタンス(内部値は同一)

            // Null値
            Assert.Null(null);
            Assert.NotNull(new object());

            // bool値
            Assert.True(true);
            Assert.False(false);

            // 文字列
            Assert.StartsWith("abc", "abcdefg");
            Assert.EndsWith("efg", "abcdefg");
        }

        [Fact(DisplayName = "型の検証")]
        public void AssertType()
        {
            // - IsAssignableFrom()とIsType()の違い
            //   IsAssignableFrom(): 任意のクラス・インターフェイスを継承・実装しているかを判定
            //   IsType(): 実行時の型を判定(抽象クラスやインターフェイスを対象とした検証は失敗)
            // - どちらも成功時に、検証した型にキャストしたオブジェクトが返却される。

            // テスト対象: HttpWebRequest
            // - 継承: Object -> MarshalByRefObject -> WebRequest -> HttpWebRequest
            // - 実装: ISerializable
            object obj = WebRequest.Create("http://www.contoso.com/"); // HttpWebRequestを返却

            // 任意のクラス・インターフェイスを継承・実装しているかを判定
            Assert.IsAssignableFrom<ISerializable>(obj);
            Assert.IsAssignableFrom<WebRequest>(obj);
            HttpWebRequest af = Assert.IsAssignableFrom<HttpWebRequest>(obj);
            Assert.Equal("http://www.contoso.com/", af.RequestUri.ToString());

            // 実行時の型を判定
            // (インターフェイスや抽象クラスは実行時の型になりえないので常に失敗)
            Assert.IsNotType<ISerializable>(obj); // インターフェイス
            Assert.IsNotType<WebRequest>(obj);    // 抽象クラス
            Assert.IsNotType<FtpWebRequest>(obj); // 継承していないクラス
            HttpWebRequest hr = Assert.IsType<HttpWebRequest>(obj);
            Assert.Equal("http://www.contoso.com/", hr.RequestUri.ToString());
        }

        [Fact(DisplayName = "例外の検証")]
        public async Task AssertException()
        {
            // 例外の検証
            // "Buffer cannot be null. (Parameter 'buffer')"
            var ex1 = Assert.Throws<ArgumentNullException>(
                () => new MemoryStream(null));
            Assert.StartsWith("Buffer cannot be null.", ex1.Message);

            // 非同期メソッドでの例外の検証
            // "Value cannot be null. (Parameter 'destination')"
            var ex2 = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await new MemoryStream().CopyToAsync(null));
            Assert.StartsWith("Value cannot be null.", ex2.Message);
        }

        [Fact(DisplayName = "コレクションの基本的な検証")]
        public void AssertCollectionBasic()
        {
            // 検証メソッドの引数はIEnumerable型なので配列やリスト等を指定可

            // リスト系

            Assert.Empty(new int[0]);
            var ints = new int[] { 1, 2 };
            Assert.Equal(new int[] { 1, 2 }, ints);
            Assert.Contains(2, ints);
            Assert.DoesNotContain(3, ints);

            Assert.Empty(new List<string>());
            var strList = new List<string>() { "abc", "123" };
            Assert.Equal(new List<string>() { "abc", "123" }, strList);
            Assert.Contains("123", strList);
            Assert.DoesNotContain("xyz", strList);

            var firstElement = Assert.Single(new int[] { 100 });
            Assert.Equal(100, firstElement);

            // ディクショナリ系

            Assert.Empty(new Dictionary<string, string>());
            var dic1 = new Dictionary<string, string>() { ["k1"] = "v1", ["k2"] = "v2" };
            var dic2 = new Dictionary<string, string>() { ["k1"] = "v1", ["k2"] = "v2" };
            Assert.Equal(dic1, dic2);
        }

        [Fact(DisplayName = "コレクションの内容を検証")]
        public void AssertCollectionContent()
        {
            var list = new List<TestData>() { new TestData("a", 1, true), new TestData("b", 2, true) };

            // 要素毎に異なる検証（全ての要素に対する検証ラムダ式の指定が必須）
            Assert.Collection(list,
                e => { Assert.Equal("a", e.Name); Assert.Equal(1, e.Count); Assert.True(e.Succeeded); },
                e => { Assert.Equal("b", e.Name); Assert.Equal(2, e.Count); Assert.True(e.Succeeded); }
            );

            // 全ての要素に対する検証（単一の検証ラムダ式のみ指定可能）
            Assert.All(list, e => Assert.True(e.Succeeded));
        }


        [Fact(DisplayName = "独自クラスを格納するリストの検証")]
        public void AssertObjectCollection()
        {
            // 適切なEquals/GetHashCodeの実装がないリストは検証不可
            var data1List1 = new List<LData1>() { new LData1(1, "a"), new LData1(2, "b") };
            var data1List2 = new List<LData1>() { new LData1(1, "a"), new LData1(2, "b") };
            Assert.NotEqual(data1List1, data1List2);
            // IEqualityComparerを実装した等価比較クラスを指定することで検証可
            // (ただし、等価比較クラスから判定に必要なフィールドにアクセスできる必要あり。
            // 判定にprivateフィールドが必要な場合は判定不可となる。)
            Assert.Equal(data1List1, data1List2, new LData1Comparer());

            // 適切なEquals/GetHashCodeを実装したリストは検証可
            var data2List1 = new List<LData2>() { new LData2(1, "a"), new LData2(2, "b") };
            var data2List2 = new List<LData2>() { new LData2(1, "a"), new LData2(2, "b") };
            Assert.Equal(data2List1, data2List2);
        }

        [Fact(DisplayName = "独自クラスを格納するディクショナリの検証")]
        public void AssertObjectDictionary()
        {
            // Dictionary<K, V>におけるVに対してオブジェクトを指定する想定。
            // 適切なEquals/GetHashCodeの実装がないオブジェクトの場合は検証不可。
            var data1Dic1 = new Dictionary<string, DData1>()
            {
                ["key1"] = new DData1(1),
                ["key2"] = new DData1(2)
            };
            var data1Dic2 = new Dictionary<string, DData1>()
            {
                ["key1"] = new DData1(1),
                ["key2"] = new DData1(2)
            };
            Assert.NotEqual(data1Dic1, data1Dic2);
            // IEqualityComparerを実装した等価比較クラスを指定することで検証可
            Assert.Equal(data1Dic1, data1Dic2, new DicComparer());

            // Dictionary<K, V>におけるVに対してオブジェクトを指定する想定。
            // 適切なEquals/GetHashCodeを実装したオブジェクトの場合は検証可。
            var data2Dic1 = new Dictionary<string, DData2>()
            {
                ["key1"] = new DData2(1),
                ["key2"] = new DData2(2)
            };
            var data2Dic2 = new Dictionary<string, DData2>()
            {
                ["key1"] = new DData2(1),
                ["key2"] = new DData2(2)
            };
            Assert.Equal(data2Dic1, data2Dic2);

        }

    }

    // オブジェクトリストの検証で使用するサンプルクラス

    public class TestData
    {
        public string Name { get; }
        public int Count { get; }
        public bool Succeeded { get; }
        public TestData(string name, int count, bool succeeded)
        {
            Name = name;
            Count = count;
            Succeeded = succeeded;
        }
    }

    public class LData1
    {
        public int Val1 { get; }
        public string Val2 { get; }
        public LData1(int val1, string val2)
        {
            Val1 = val1;
            Val2 = val2;
        }
    }
    public class LData2 : LData1
    {
        public LData2(int val1, string val2) : base(val1, val2) { }

        // VisualStudioのリファクタリング機能で自動追加可能
        // https://docs.microsoft.com/ja-jp/visualstudio/ide/reference/generate-equals-gethashcode-methods?view=vs-2019
        public override bool Equals(object obj)
            => obj is LData2 data &&
                Val1 == data.Val1 &&
                Val2 == data.Val2;

        public override int GetHashCode()
            => HashCode.Combine(Val1, Val2);
    }

    public class LData1Comparer : IEqualityComparer<LData1>
    {
        public bool Equals([AllowNull] LData1 x, [AllowNull] LData1 y)
            => (x == null && y == null)
                || (x != null && y != null
                    && x.Val1 == y.Val1
                    && x.Val2 == y.Val2);

        public int GetHashCode([DisallowNull] LData1 obj)
            => throw new NotImplementedException();
    }

    // オブジェクトを格納するディクショナリの検証で使用するサンプルクラス

    public class DData1
    {
        public int Val { get; }
        public DData1(int val) => Val = val;
    }
    public class DData2 : DData1
    {
        public DData2(int val) : base(val) { }
        public override bool Equals(object obj)
            => obj is DData2 data && Val == data.Val;
        public override int GetHashCode()
            => HashCode.Combine(Val);
    }

    // Dictionary<string, DData1>に特化した等価比較クラス
    // (より汎用的なDictionary<K, V>の定義も可能だが、Vに対応するDData1クラスで
    // Equals()の実装が必要となってしまうので、ここでは特化している。)
    public class DicComparer : IEqualityComparer<Dictionary<string, DData1>>
    {
        public bool Equals(
            [AllowNull] Dictionary<string, DData1> x, [AllowNull] Dictionary<string, DData1> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;

            // 格納数の検証
            if (x.Count != y.Count) return false;

            // 含まれるキーの検証
            foreach (var xkey in x.Keys)
                if (!y.ContainsKey(xkey)) return false;

            // 含まれる値の検証
            foreach (var xpair in x)
                if (xpair.Value.Val != y[xpair.Key].Val) return false;

            return true;
        }

        public int GetHashCode([DisallowNull] Dictionary<string, DData1> obj)
            => throw new NotImplementedException();
    }

}
