namespace SnmpSharpNet.Tests;

public class NoSuchObjectTests
{
    [Test]
    public async Task EncodedToDecodeMutable()
    {
        var initial = new NoSuchObject();
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new NoSuchObject();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new NoSuchObject();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new NoSuchObject();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}