namespace SnmpSharpNet.Tests;

public class NoSuchInstanceTests
{
    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new NoSuchInstance();
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new NoSuchInstance();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
    [Test]
    public async Task EncodedToDecodeMutable()
    {
        var initial = new NoSuchInstance();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new NoSuchInstance();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}