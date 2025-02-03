namespace SnmpSharpNet.Tests;

public class MsgFlagsTests
{
    [Test]
    public async Task EncodedToDecodeMutable()
    {
        var initial = new MsgFlags(false, true, false);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new MsgFlags();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }

    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new MsgFlags(false, true, false);
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new MsgFlags();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}