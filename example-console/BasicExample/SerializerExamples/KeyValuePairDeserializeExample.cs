using BasicExample.Misc;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace BasicExample.SerializerExamples
{
    public class KeyValuePairDeserializeExample : BaseTests
    {
        private const string TestJson =
            "{" +
            "  \"key1\": {" +
            "    \"value1\": 111," +
            "    \"value2\": \"test1\"," +
            "    \"value3\": [1, 2, 3]" +
            "  }," +
            "  \"key2\": {" +
            "    \"value1\": 222," +
            "    \"value2\": \"test2\"," +
            "    \"value3\": [2, 3, 4]" +
            "  }" +
            "}";

        public KeyValuePairDeserializeExample(ITestOutputHelper output) : base(output) { }

        // 【基本】
        // How to serialize and deserialize (marshal and unmarshal) JSON in .NET
        // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-core-3-1

        // 【手動で解析する場合】
        // How to use a DOM, Utf8JsonReader, and Utf8JsonWriter in System.Text.Json
        // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-use-dom-utf8jsonreader-utf8jsonwriter?pivots=dotnet-core-3-1


        [Fact(DisplayName = "OrderedDictionaryの値を手動で解析する方法")]
        public void Test_OrderedDeserialize1()
        {
            var result = JsonSerializer.Deserialize<OrderedDictionary>(TestJson);
            foreach (DictionaryEntry e in result)
            {
                var key = e.Key;
                var valElement = (JsonElement)e.Value;
                var entity = new MyEntity()
                {
                    Value1 = valElement.GetProperty("value1").GetInt32(),
                    Value2 = valElement.GetProperty("value2").GetString(),
                    Value3 = valElement.GetProperty("value3").EnumerateArray()
                        .Select(je => je.GetInt32()).ToArray()
                };
                Console.WriteLine("{0}={1}", key, entity);
            }
        }

        [Fact(DisplayName = "OrderedDictionaryの値をデシリアライズする方法")]
        public void Test_OrderedDeserialize2()
        {
            var result = JsonSerializer.Deserialize<OrderedDictionary>(TestJson);
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            foreach (DictionaryEntry e in result)
            {
                var key = e.Key;
                var valElement = (JsonElement)e.Value;
                var valJson = valElement.GetRawText();
                var entity = JsonSerializer.Deserialize<MyEntity>(valJson, options);
                Console.WriteLine("{0}={1}", key, entity);
            }
        }

        [Fact(DisplayName = "JsonDocumentを使って手動で解析する方法")]
        public void Test_OrderedDeserialize3()
        {
            using var document = JsonDocument.Parse(TestJson);
            JsonElement root = document.RootElement;

            foreach (var jsonProperty in root.EnumerateObject())
            {
                var key = jsonProperty.Name;
                var valElement = jsonProperty.Value;
                var entity = new MyEntity()
                {
                    Value1 = valElement.GetProperty("value1").GetInt32(),
                    Value2 = valElement.GetProperty("value2").GetString(),
                    Value3 = valElement.GetProperty("value3").EnumerateArray()
                        .Select(ve => ve.GetInt32()).ToArray()
                };
                Console.WriteLine("{0}={1}", key, entity);
            }
        }

        class MyEntity
        {
            public int Value1 { get; set; }
            public string Value2 { get; set; }
            public int[] Value3 { get; set; }
            public override string ToString()
                => $"{{Value1={Value1}, Value2={Value2}, Value3=[{string.Join(", ", Value3)}]}}";
        }

    }

}
