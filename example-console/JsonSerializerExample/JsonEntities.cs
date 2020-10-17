using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace JsonSerializerExample
{
    public class BasicExampleElement
    {
        // 基本型
        public int IntValue { get; set; }
        public int? IntNullableValue { get; set; }
        public float FloatValue { get; set; }
        public string StringValue { get; set; }
        public byte[] ByteArrayValue { get; set; }
        public IEnumerable<NestedDummyElement> Children { get; set; }

        // bool型
        public BoolExampleElement BoolExample { get; set; }

        // DateTime型
        public DateTimeExampleElement DateTimeExample { get; set; }

        // 列挙型
        public EnumExampleElement EnumExample { get; set; }

        // 任意クラス
        public ClassExampleElement ClassExample { get; set; }
    }

    public class NestedDummyElement
    {
        public int IntValue { get; set; }
        public string StringValue { get; set; }
    }

    public class BoolExampleElement
    {
        public bool BoolValue { get; set; }

        [JsonConverter(typeof(BoolSampleConverter))]
        public bool BoolSampleValue { get; set; }

        [JsonConverter(typeof(BoolOneZeroConverter))]
        public bool BoolOneZeroValue { get; set; }

        [JsonConverter(typeof(BoolYesNoConverter))]
        public bool BoolYesNoValue { get; set; }
    }

    public class DateTimeExampleElement
    {
        public DateTime DateTimeValue { get; set; }

        [JsonConverter(typeof(DateTimeYmdConverter))]
        public DateTime DateTimeYmdValue { get; set; }

        [JsonConverter(typeof(DateTimeYmHyphenConverter))]
        public DateTime DateTimeYmHyphenValue { get; set; }
    }

    public class EnumExampleElement
    {
        public Status StatusValue { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status StatusStringValue { get; set; }

        [JsonConverter(typeof(StatusEnumStringConverter))]
        public Status StatusCustomStringValue { get; set; }
    }

    public class ClassExampleElement
    {
        public string SiteName { get; set; }
        public UriElement Uri { get; set; }
    }

    [JsonConverter(typeof(UrlElementConverter))]
    public class UriElement
    {
        public string Scheme { get; set; }
        public string Hostname { get; set; }
        public int PortNumber { get; set; }
    }

    public enum Status
    {
        Start = 1, End = 2
    }

}
