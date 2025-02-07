using System;
using FluentAssertions;
using PKHeX.Core;
using Xunit;
using static System.Buffers.Binary.BinaryPrimitives;

namespace PKHeX.Tests.Util;

public class ConvertUtilTests
{
    [Theory]
    [InlineData("-050 ", -50)]
    [InlineData("123 45678", 12345678)]
    [InlineData("--8765-43 21", -87654321)]
    public void CheckConvertValidI32(string v, int result)
    {
        var convert = Core.Util.ToInt32(v);
        convert.Should().Be(result);
    }

    [Theory]
    [InlineData("50", 50)]
    [InlineData("12 345678", 12345678)]
    [InlineData("87654 321", 87654321)]
    public void CheckConvertValidU32(string v, uint result)
    {
        var convert = Core.Util.ToUInt32(v);
        convert.Should().Be(result);
    }

    [Theory]
    [InlineData("0x50", 0x50)]
    [InlineData("0x12 345678", 0x12345678)]
    [InlineData("8aF5z4 32-1", 0x8aF54321)]
    public void CheckConvertValidHexU32(string v, uint result)
    {
        var convert = Core.Util.GetHexValue(v);
        convert.Should().Be(result);
    }

    [Theory]
    [InlineData("01020304", 0x1020304)]
    public void CheckConvertHexString(string v, uint result)
    {
        var convert = Core.Util.GetBytesFromHexString(v);
        var u32 = ReadUInt32LittleEndian(convert);
        u32.Should().Be(result);

        var remake = Core.Util.GetHexStringFromBytes(convert);
        remake.Should().Be(v);
    }

    [Theory]
    [InlineData(0x12345678, 12345678)]
    public void CheckConvertBCD_Little(uint raw, int expect)
    {
        Span<byte> data = stackalloc byte[4];
        WriteUInt32LittleEndian(data, raw);

        var result = BinaryCodedDecimal.ToInt32LE(data);
        result.Should().Be(expect);

        Span<byte> newData = stackalloc byte[4];
        BinaryCodedDecimal.WriteBytesLE(newData, result);
        data.SequenceEqual(newData).Should().BeTrue();
    }

    [Theory]
    [InlineData(0x78563412, 12345678)]
    public void CheckConvertBCD_Big(uint raw, int expect)
    {
        Span<byte> data = stackalloc byte[4];
        WriteUInt32LittleEndian(data, raw);

        var result = BinaryCodedDecimal.ToInt32BE(data);
        result.Should().Be(expect);

        Span<byte> newData = stackalloc byte[4];
        BinaryCodedDecimal.WriteBytesBE(newData, result);
        data.SequenceEqual(newData).Should().BeTrue();
    }
}
