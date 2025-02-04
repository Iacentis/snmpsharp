namespace SnmpSharpNet.Tests;

public class EndOfMibViewTests
{
    [Test]
    public async Task EncodedToDecodeMutable()
    {
        var initial = new EndOfMibView();
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new EndOfMibView();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }

    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new EndOfMibView();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new EndOfMibView();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}