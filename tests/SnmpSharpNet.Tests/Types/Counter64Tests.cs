namespace SnmpSharpNet.Tests;

public class Counter64Tests
{
    [Test]
    [Arguments(ulong.MinValue)]
    [Arguments(2u)]
    [Arguments(ulong.MaxValue)]
    public async Task EncodedToDecodeMutable(ulong a)
    {
        var initial = new Counter64(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Counter64();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
    [Test]
    [Arguments(ulong.MinValue)]
    [Arguments(2u)]
    [Arguments(ulong.MaxValue)]
    public async Task EncodedToDecode(ulong a)
    {
        var initial = new Counter64(a);
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new Counter64();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}