using ExampleUtils.Utils;
using System;
using Xunit;

namespace ExampleUtils.Test.Utils
{
    public class NumberUtilsTests
    {
        // CreateZonedDecimalBytes

        [Theory(DisplayName = "整数からゾーン10進数を生成: 正常系")]
        [InlineData(new byte[] { 0x30 }, 0)]
        [InlineData(new byte[] { 0x71 }, -1)]
        [InlineData(new byte[] { 0x31, 0x32, 0x33 }, 123)]
        [InlineData(new byte[] { 0x31, 0x32, 0x73 }, -123)]
        public void Test_CreateZonedDecimalBytes_Ok(byte[] expected, long test)
            => Assert.Equal(expected, NumberUtils.CreateZonedDecimalBytes(test));

        // ParseZonedDecimal

        [Theory(DisplayName = "ゾーン10進数から整数を生成: 正常系")]
        [InlineData(0, new byte[] { 0x30 })]
        [InlineData(-1, new byte[] { 0x71 })]
        [InlineData(123, new byte[] { 0x31, 0x32, 0x33 })]
        [InlineData(-123, new byte[] { 0x31, 0x32, 0x73 })]
        public void Test_ParseZonedDecimal_Ok(long expected, byte[] test)
            => Assert.Equal(expected, NumberUtils.ParseZonedDecimal(test));

        [Theory(DisplayName = "ゾーン10進数から整数を生成: 異常系(ゾーン部不正)")]
        [InlineData(new byte[] { 0x31, 0x42, 0x33 })]
        public void Test_ParseZonedDecimal_Ng_InvalidZonePart(byte[] test)
        {
            var ex = Assert.Throws<ArgumentException>( ()=> NumberUtils.ParseZonedDecimal(test));
            Assert.StartsWith("不正なゾーン部: ", ex.Message);
        }

        [Theory(DisplayName = "ゾーン10進数から整数を生成: 異常系(符号部不正)")]
        [InlineData(new byte[] { 0x31, 0x32, 0x43 })]
        public void Test_ParseZonedDecimal_Ng_InvalidSignPart(byte[] test)
        {
            var ex = Assert.Throws<ArgumentException>(() => NumberUtils.ParseZonedDecimal(test));
            Assert.StartsWith("不正な符号部: ", ex.Message);
        }

        [Theory(DisplayName = "ゾーン10進数から整数を生成: 異常系(数値部不正)")]
        [InlineData(new byte[] { 0x31, 0x3a, 0x33 })]
        [InlineData(new byte[] { 0x31, 0x3a, 0x3b })]
        public void Test_ParseZonedDecimal_Ng_InvalidNumPart(byte[] test)
        {
            var ex = Assert.Throws<ArgumentException>(() => NumberUtils.ParseZonedDecimal(test));
            Assert.StartsWith("不正な数値部: ", ex.Message);
        }

        [Fact(DisplayName = "ゾーン10進数から整数を生成: 異常系(桁あふれ)")]
        public void Test_ParseZonedDecimal_Ng_Overflow()
        {
            var test = new byte[] {
                0x39, 0x39, 0x39, 0x39, 0x39, /* */ 0x39, 0x39, 0x39, 0x39, 0x39,
                0x39, 0x39, 0x39, 0x39, 0x39, /* */ 0x39, 0x39, 0x39, 0x39, 0x39
            };
            var ex = Assert.Throws<OverflowException>(() => 
                NumberUtils.ParseZonedDecimal(test));
            Assert.Equal("Value was either too large or too small for an Int64.", ex.Message);
        }

        // CreatePackedDecimalBytes

        [Theory(DisplayName = "整数からパック10進数を生成: 正常系")]
        [InlineData(new byte[] { 0x0c }, 0)]
        [InlineData(new byte[] { 0x1d }, -1)]
        [InlineData(new byte[] { 0x12, 0x3c }, 123)]
        [InlineData(new byte[] { 0x12, 0x3d }, -123)]
        public void Test_CreatePackedDecimalBytes_Ok(byte[] expected, long test)
            => Assert.Equal(expected, NumberUtils.CreatePackedDecimalBytes(test));

        // ParsePackedDecimal

        [Theory(DisplayName = "パック10進数から整数を生成: 正常系")]
        [InlineData(0, new byte[] { 0x0c })]
        [InlineData(-1, new byte[] { 0x1d })]
        [InlineData(123, new byte[] { 0x12, 0x3c })]
        [InlineData(-123, new byte[] { 0x12, 0x3d })]
        public void Test_ParsePackedDecimal_Ok(long expected, byte[] test)
            => Assert.Equal(expected, NumberUtils.ParsePackedDecimal(test));

        [Theory(DisplayName = "パック10進数から整数を生成: 異常系(符号部不正)")]
        [InlineData(new byte[] { 0x01, 0x2f })]
        public void Test_ParsePackedDecimal_Ng_InvalidSignPart(byte[] test)
        {
            var ex = Assert.Throws<ArgumentException>(() => NumberUtils.ParsePackedDecimal(test));
            Assert.StartsWith("不正な符号部: ", ex.Message);
        }

        [Theory(DisplayName = "パック10進数から整数を生成: 異常系(数値部不正)")]
        [InlineData(new byte[] { 0xa2, 0x3c })]
        [InlineData(new byte[] { 0x1b, 0x3c })]
        [InlineData(new byte[] { 0x12, 0xcc })]
        public void Test_ParsePackedDecimal_Ng_InvalidNumPart(byte[] test)
        {
            var ex = Assert.Throws<ArgumentException>(() => NumberUtils.ParsePackedDecimal(test));
            Assert.StartsWith("不正な数値部: ", ex.Message);
        }

        [Fact(DisplayName = "パック10進数から整数を生成: 異常系(桁あふれ)")]
        public void Test_ParsePackedDecimal_Ng_Overflow()
        {
            var test = new byte[] {
                0x99, 0x99, 0x99, 0x99, 0x99, /* */ 0x99, 0x99, 0x99, 0x99, 0x9d
            };
            var ex = Assert.Throws<OverflowException>(() =>
                NumberUtils.ParsePackedDecimal(test));
            Assert.Equal("Value was either too large or too small for an Int64.", ex.Message);
        }

    }

}
