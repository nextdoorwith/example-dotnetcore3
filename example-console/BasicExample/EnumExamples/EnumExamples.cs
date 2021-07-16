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

    public class EnumExamples: BaseTests
    {
        public EnumExamples(ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName = "基本")]
        public void Example1()
        {
            // 列挙体の基底クラスはEnum、基になる値型はint
            var e = EnumTest1.Val1;
            var et = e.GetType();
            Console.WriteLine("Type                : {0}", et.FullName);
            Console.WriteLine("BaseType            : {0}", et.BaseType.FullName);
            Console.WriteLine("UnderlyingSystemType: {0}", et.UnderlyingSystemType.FullName);
            Console.WriteLine("EnumUnderlyingType  : {0}", et.GetEnumUnderlyingType().FullName);

            // 列挙体の名称と整数値を取得
            var val1str = EnumTest1.Val1.ToString();
            var val1int = (int)EnumTest1.Val1;
            Console.WriteLine("{0}: {1}", val1str, val1int);

            // 列挙体の各値名の取得
            var names = Enum.GetNames(typeof(EnumTest1));
            Console.WriteLine(string.Join(',', names)); // "Val1,Val2,Val3"

            // 列挙体の各値の一覧の取得
            var array = Enum.GetValues(typeof(EnumTest2)); // System.Array型
            var values = array.OfType<EnumTest2>().ToArray(); // Arrayを配列に変換(LINQ)
            Console.WriteLine(values[0]); // "ValA"
            Console.WriteLine(values[1]); // "ValB" 
            Console.WriteLine(values[2]); // "ValC"
        }

        [Fact(DisplayName = "列挙体変換(静的)")]
        public void Example2()
        {

            // 整数から列挙体に変換
            EnumTest1 val2 = (EnumTest1)100;
            Console.WriteLine("{0}({1})", val2, val2.GetType().FullName);

            // Enum型から特定の列挙体に変換
            Enum en = EnumTest1.Val3;
            EnumTest1 en2test1 = (EnumTest1)en;
            Console.WriteLine(en2test1);
            EnumTest2 en2test2 = (EnumTest2)en;
            Console.WriteLine(en2test2);

        }

        [Fact(DisplayName = "列挙体変換(動的)")]
        public void Example3()
        {
            // 変換先列挙体を実行時に決定
            var type = Type.GetType("BasicExample.EnumExamples.EnumTest2");

            // 列挙体に含まれる値か否かを判定
            Console.WriteLine(Enum.IsDefined(type, 400)); // False
            Console.WriteLine(Enum.IsDefined(type, 300)); // True

            // 整数を列挙体に変換
            var valc = Enum.ToObject(type, 300);
            Console.WriteLine("{0}({1})", valc, valc.GetType().FullName);
            // -> "ValC(BasicExample.EnumExamples.EnumTest2)"

            // 文字列を列挙体に変換
            var valb = Enum.Parse(type, "ValB", true);
            Console.WriteLine("{0}({1})", valb, valb.GetType().FullName);
            // -> "ValB(BasicExample.EnumExamples.EnumTest2)"

        }

    }

}
