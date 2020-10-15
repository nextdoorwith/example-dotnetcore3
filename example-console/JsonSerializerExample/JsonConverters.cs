using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonSerializerExample
{
    public class BoolSampleConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetString().Equals("yes"); // "yes"ならtrue、それ以外はfalse
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            => writer.WriteStringValue(value ? "yes" : "no");
    }

    public class BoolStringConverterBase : JsonConverter<bool>
    {
        public string TrueValue { get; }
        public string FalseValue { get; }
        public bool IgnoreCase { get; }
        public BoolStringConverterBase(string trueValue, string falseValue, bool ignoreCase = true)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
            IgnoreCase = ignoreCase;
        }

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetString().Equals(TrueValue, IgnoreCase ?
                StringComparison.CurrentCultureIgnoreCase :
                StringComparison.CurrentCulture);

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            => writer.WriteStringValue(value ? TrueValue : FalseValue);
    }

    public class BoolYesNoConverter : BoolStringConverterBase
    {
        public BoolYesNoConverter() : base("yes", "no") { }
    }

    public class BoolOnOffConverter : BoolStringConverterBase
    {
        public BoolOnOffConverter() : base("on", "off") { }
    }

    public class BoolIntConverterBase : JsonConverter<bool>
    {
        public int TrueValue { get; }
        public int FalseValue { get; }
        public BoolIntConverterBase(int trueValue, int falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetInt32().Equals(TrueValue);
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value ? TrueValue : FalseValue);
    }

    public class BoolOneZeroConverter : BoolIntConverterBase
    {
        public BoolOneZeroConverter() : base(1, 0) { }
    }


    public class DateTimeConverterBase : JsonConverter<DateTime>
    {
        public string DateTimeFormat { get; }
        public DateTimeConverterBase(string dateTimeFormat)
        {
            DateTimeFormat = dateTimeFormat;
        }
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.ParseExact(reader.GetString(), DateTimeFormat, CultureInfo.CurrentCulture);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(DateTimeFormat));
    }

    public class DateTimeLocalConverter : DateTimeConverterBase
    {
        public DateTimeLocalConverter() : base("yyyy/MM/dd HH:mm:ss") { }
    }

    public class DateTimeYmdConverter : DateTimeConverterBase
    {
        public DateTimeYmdConverter() : base("yyyyMMdd") { }
    }

    public class DateTimeYmHyphenConverter : DateTimeConverterBase
    {
        public DateTimeYmHyphenConverter() : base("yyyy-MM") { }
    }


    public class StatusEnumStringConverter : JsonConverter<Status>
    {
        public override Status Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.GetString())
            {
                case "begin": return Status.Start;
                case "finish": return Status.End;
                default: return default;
            }
        }
        public override void Write(Utf8JsonWriter writer, Status value, JsonSerializerOptions options)
        {
            string result;
            switch (value)
            {
                case Status.Start: result = "begin"; break;
                case Status.End: result = "finish"; break;
                default: result = string.Empty; break;
            }
            writer.WriteStringValue(result);
        }
    }
}
