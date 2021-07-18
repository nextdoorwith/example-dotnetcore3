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
        // 参考）
        // 読み取りのみの場合、Dictionary<,>はスレッドセーフ
        // https://docs.microsoft.com/ja-jp/dotnet/api/system.collections.generic.dictionary-2?view=netcore-3.1#thread-safety

        /// <summary>
        /// 表示名ディクショナリ
        /// </summary>
        private static Dictionary<Enum, string> _displayNameDic = new Dictionary<Enum, string>();

        /// <summary>
        /// 文字列値ディクショナリ
        /// </summary>
        private static Dictionary<Enum, string> _strValDic = new Dictionary<Enum, string>();

        // 使用時の性能向上のため、クラスロード時に纏めて初期化する。
        static EnumExtensions()
        {
            // アセンブリに含まれる列挙体の全てを取得
            Assembly assembly = typeof(EnumExtensions).Assembly;
            var enumTypes = assembly.GetTypes().Where(e => e.IsEnum).ToArray();

            // 列挙体で属性指定がある場合、表示名と文字列値をキャッシュ
            foreach (var type in enumTypes)
            {
                // 列挙体に含まれる全ての値を取得(列挙体の値自身もEnum型)
                // (System.Arrayから配列に変換)
                var vals = Enum.GetValues(type).OfType<Enum>().ToArray();

                // 属性が指定された値がある場合、表示名と文字列値を追加
                foreach (var v in vals)
                {
                    var mi = type.GetMember(v.ToString()).FirstOrDefault();
                    var attr = mi?.GetCustomAttribute<EnumInfoAttribute>();
                    if (attr != null)
                    {
                        _displayNameDic[v] = attr.DisplayName;
                        _strValDic[v] = attr.Value;
                    }
                }
            }
        }

        /// <summary>
        /// 表示名を取得する。
        /// </summary>
        /// <param name="e">列挙体値</param>
        /// <returns>表示名</returns>
        public static string GetDisplayName(this Enum e)
            => _displayNameDic.ContainsKey(e) ? _displayNameDic[e]: null;

        /// <summary>
        /// 文字列値を取得する。
        /// </summary>
        /// <param name="e">列挙体値</param>
        /// <returns>文字列値</returns>
        public static string GetStrValue(this Enum e)
            => _strValDic.ContainsKey(e) ? _strValDic[e] : null;
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
            Console.WriteLine(EnumExTest1.Val1.ToString());       // "Val1"
            Console.WriteLine((int)EnumExTest1.Val1);             // "0"
            Console.WriteLine(EnumExTest1.Val1.GetDisplayName()); // "値1"
            Console.WriteLine(EnumExTest1.Val1.GetStrValue());    // "01"

            Console.WriteLine(EnumExTest2.Yes.ToString());       // "Yes"
            Console.WriteLine((int)EnumExTest2.Yes);             // "1"
            Console.WriteLine(EnumExTest2.Yes.GetDisplayName()); // "はい"
            Console.WriteLine(EnumExTest2.Yes.GetStrValue());    // "1"
        }

    }

}
