using BasicExample.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace BasicExample.EnumExamples
{
    // 列挙体に表示名と文字列値を持たせるサンプル

    // 列挙体の例１
    public enum EnumExTest1{
        [EnumInfo("値1", "01")]
        Val1,
        [EnumInfo("値2", "02")]
        Val2,
        [EnumInfo("値3", "03")]
        Val3
    }

    // 列挙体の例２
    public enum EnumExTest2
    {
        [EnumInfo("いいえ", "0")]
        No,
        [EnumInfo("はい", "1")]
        Yes
    }

    /// <summary>
    /// 列挙体付加情報属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumInfoAttribute : Attribute
    {
        public string DisplayName { get; }
        public string Value { get; }
        public EnumInfoAttribute(string displayName, string value)
        {
            DisplayName = displayName;
            Value = value;
        }
    };

    /// <summary>
    /// Enum拡張
    /// </summary>
    public static class EnumExtensions
    {
        // 書込みがない場合はスレッドセーフ
        // https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.generic.dictionary-2?view=netcore-3.1#thread-safety
        private static Dictionary<Enum, string> _displayNameDic = new Dictionary<Enum, string>();
        private static Dictionary<Enum, string> _valueDic = new Dictionary<Enum, string>();

        // 使用時の性能向上のため、クラスロード時に纏めて初期化する。
        static EnumExtensions()
        {
            // アセンブリに含まれる列挙体の全てを取得
            Assembly assembly = typeof(EnumExtensions).Assembly;
            var enumTypes = assembly.GetTypes().Where(e => e.IsEnum).ToArray();

            // 列挙体で特定属性が指定されている値がある場合、当該属性の表示名をキャッシュ
            foreach (var type in enumTypes)
            {
                // 列挙体に含まれる全ての値を取得(列挙体の値自身もEnum型)
                var vals = Enum.GetValues(type).OfType<Enum>().ToArray(); // System.Arrayから配列に変換

                // 属性が指定された値がある場合、表示名を追加
                foreach (var v in vals)
                {
                    var mi = type.GetMember(v.ToString()).FirstOrDefault();
                    var attr = mi?.GetCustomAttribute<EnumInfoAttribute>();
                    if (attr != null)
                    {
                        _displayNameDic[v] = attr.DisplayName;
                        _valueDic[v] = attr.Value;
                    }
                }
            }
        }

        public static string GetDisplayName(this Enum e)
            => _displayNameDic.ContainsKey(e) ? _displayNameDic[e]: null;

        public static string GetValue(this Enum e)
            => _valueDic.ContainsKey(e) ? _valueDic[e] : null;
    }

    /// <summary>
    /// 動作確認用コード
    /// </summary>
    public class EnumExtensionExample : BaseTests
    {
        public EnumExtensionExample(ITestOutputHelper output) : base(output) { }

        [Fact(DisplayName = "基本")]
        public void Example1()
        {
            Console.WriteLine(EnumExTest1.Val1.GetDisplayName());
            Console.WriteLine(EnumExTest1.Val1.GetValue());

            Console.WriteLine(EnumExTest2.Yes.GetDisplayName());
            Console.WriteLine(EnumExTest2.Yes.GetValue());

            var e1 = (EnumExTest2)1;
            Console.WriteLine(e1.GetDisplayName());
        }

    }

}
