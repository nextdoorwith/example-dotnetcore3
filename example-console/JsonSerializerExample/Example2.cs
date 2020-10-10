using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace JsonSerializerExample
{
    public class Example2
    {
        public static void Run()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };
            var result = JsonSerializer.Serialize(new BasicTypeElement(), options);
            Console.WriteLine(result);
        }

    }

    public class BasicTypeElement
    {
        public byte ByteValue { get; set; } = 0x20;
        public byte[] ByteArrayValue { get; set; } = { 0x00, 0x01, 0x02, 0x03 };

        public char CharValue { get; set; } = (char)0x21;
        public char[] CharArrayValue { get; set; } = { 'a', 'b', 'c' };

        public int IntValue { get; set; } = int.MinValue;
        public int[] IntArrayValue { get; set; } = { 1, 2, 3, 4};

        public long LongValue { get; set; } = long.MaxValue;
        
        public float FloatZeroValue { get; set; } = 0;
        public float FloatAnyValue { get; set; } = 3.14F;
        public float FloatMaxValue { get; set; } = float.MaxValue;
        //public float FloatInfinityValue { get; set; } = float.NegativeInfinity;

        public double DoubleZeroValue { get; set; } = 0;
        public double DoubleAnyValue { get; set; } = 1000000.000D;
        public double DoubleMaxValue { get; set; } = double.MinValue;
        //public double DoubleInfinityValue { get; set; } = double.PositiveInfinity;

        public string StringValue { get; set; } = "123\tabc\r\nABC";
        public string StringEscValue { get; set; } = @"!""#$%&'()*+,-./:;<=>?@[\]^_`{|}~";

        public bool BoolValue { get; set; } = true;

        public Status EnumValue { get; set; } = Status.End;

        public IEnumerable<string> EnumerableValue { get; set; } = new List<string>() { "123", "abc" };

        //public IDictionary<int, float> DicValue1 { get; set; } = 
        //    new Dictionary<int, float>(){
        //        [100] = 1.0F
        //    };
        //public IDictionary<object, float> DicValue2 { get; set; } =
        //    new Dictionary<object, float>()
        //    {
        //        ["100"] = 1.0F
        //    };
        public IDictionary<string, int> DicValue3 { get; set; } =
            new Dictionary<string, int>()
            {
                ["aaa"] = 1111,
                ["bbb"] = 2222
            };

    }

}
