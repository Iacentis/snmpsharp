namespace SnmpSharpNet.Tests;

public class UInteger32Tests
{
    [Test]
    [Arguments(uint.MaxValue)]
    [Arguments(2u)]
    [Arguments(uint.MinValue)]
    public async Task EncodedToDecodeMutable(uint a)
    {
        var initial = new UInteger32(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new UInteger32();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }

    [Test]
    [Arguments(uint.MaxValue)]
    [Arguments(2u)]
    [Arguments(uint.MinValue)]
    public async Task EncodedToDecode(uint a)
    {
        Span<byte> buffer = stackalloc byte[UInteger32.MaxEncodedSize];
        var initial = new UInteger32(a);
        initial.encode(buffer);
        var @new = new UInteger32();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}