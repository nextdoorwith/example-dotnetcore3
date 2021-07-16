using BasicExample.Misc;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace BasicExample.EnumExamples
{
    public enum EnumTest1
    {
        Val1,       // 0
        Val2 = 100, // 100
        Val3        // 101
    }
    public enum EnumTest2
    {
        ValA = 100,
        ValB = 200,
        ValC = 300
    }
    public enum EnumTest3
    {
        No, Yes
    }

    public class EnumExamples: BaseTests
    {
        public EnumExamples(ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName = "基本")]
        public void Example1()
        {
            // 列挙体の基底クラスはEnum、基になる値型はint
            var e = EnumTest1.Val1;
            var et = e.GetType();
            Console.WriteLine(et.FullName);
            // -> "BasicExample.EnumExamples.EnumTest1"
            Console.WriteLine(et.BaseType.FullName);
            // -> "System.Enum"
            Console.WriteLine(et.UnderlyingSystemType.FullName);
            // ->"BasicExample.EnumExamples.EnumTest1"
            Console.WriteLine(et.GetEnumUnderlyingType().FullName);
            // -> "System.Int32"

            // 列挙体の名称と整数値を取得
            var val1str = EnumTest1.Val1.ToString();
            var val1int = (int)EnumTest1.Val1;
            Console.WriteLine("{0}: {1}", val1str, val1int); // "Val1: 0"

            // 列挙体の各値名の取得
            var names = Enum.GetNames(typeof(EnumTest1));
            Console.WriteLine(string.Join(',', names)); // "Val1,Val2,Val3"

            // 列挙体の各値の一覧の取得
            var array = Enum.GetValues(typeof(EnumTest2)); // System.Array型
            foreach(var v in array)
                Console.WriteLine(v); // "ValA", "ValB", "ValC"

            // 列挙体の各値の一覧の取得(Arrayから配列変換)
            var values = array.OfType<EnumTest2>().ToArray();
            Console.WriteLine(values[0]); // "ValA"
        }

        [Fact(DisplayName = "列挙体変換(静的)")]
        public void Example2()
        {
            // 整数から列挙体に変換
            var int100 = 100;
            Console.WriteLine(Enum.IsDefined(typeof(EnumTest2), int100));  // "True"
            EnumTest1 val2 = (EnumTest1)int100;
            Enum.IsDefined(typeof(EnumTest1), val2);
            Console.WriteLine("{0}({1})", val2, val2.GetType().FullName);
            // -> "Val2(BasicExample.EnumExamples.EnumTest1)"

            // 整数から列挙体に変換(該当する列挙体値なし)
            var int999 = 999;
            Console.WriteLine(Enum.IsDefined(typeof(EnumTest1), int999)); // "False"
            EnumTest1 valx = (EnumTest1)int999;
            Console.WriteLine("{0}({1})", valx, valx.GetType().FullName);
            // -> "999(BasicExample.EnumExamples.EnumTest1)"

            // Enum型から特定の列挙体に変換
            Enum e = EnumTest1.Val2; // Val2は100
            EnumTest1 e1 = (EnumTest1)e;
            Console.WriteLine(e1); // "Val2"
            EnumTest2 e2 = (EnumTest2)e;
            Console.WriteLine(e2); // "ValA"(EnumTest2で100はValA)

            // Enum型から特定の列挙体に変換(該当する列挙体値なし)
            EnumTest3 e3 = (EnumTest3)e;
            Console.WriteLine(e3); // "100"
        }

        [Fact(DisplayName = "列挙体変換(動的)")]
        public void Example3()
        {
            // 変換先列挙体を実行時に決定する想定の例
            var enumTest2 = Type.GetType("BasicExample.EnumExamples.EnumTest2");

            // 列挙体に含まれる値か否かを判定
            Console.WriteLine(Enum.IsDefined(enumTest2, 400)); // "False"
            Console.WriteLine(Enum.IsDefined(enumTest2, 300)); // "True"

            // 整数を列挙体に変換
            var valc = Enum.ToObject(enumTest2, 300);
            Console.WriteLine("{0}({1})", valc, valc.GetType().FullName);
            // -> "ValC(BasicExample.EnumExamples.EnumTest2)"

            // 整数を列挙体に変換(該当する列挙体値なし)
            var val999 = Enum.ToObject(enumTest2, 999);
            Console.WriteLine("{0}({1})", val999, val999.GetType().FullName);
            // -> "999(BasicExample.EnumExamples.EnumTest2)"

            // 文字列を列挙体に変換
            var valb = Enum.Parse(enumTest2, "ValB", true);
            Console.WriteLine("{0}({1})", valb, valb.GetType().FullName);
            // -> "ValB(BasicExample.EnumExamples.EnumTest2)"

            // 文字列を列挙体に変換(該当する列挙体値なし)
            //var valx = Enum.Parse(enumTest2, "ValX", true);
            // -> System.ArgumentException : Requested value 'ValX' was not found.
        }

    }

}
