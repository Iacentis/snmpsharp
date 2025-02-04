namespace SnmpSharpNet.Tests;

public class OpaqueTests
{
    [Test]
    [Arguments("1.3.2.4.1.5.6.8")]
    public async Task EncodedToDecodeMutable(string a)
    {
        var initial = new Opaque(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Opaque();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }

    [Test]
    [Arguments("1.3.2.4.1.5.6.8")]
    public async Task EncodedToDecode(string a)
    {
        var initial = new Opaque(a);
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new Opaque();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}