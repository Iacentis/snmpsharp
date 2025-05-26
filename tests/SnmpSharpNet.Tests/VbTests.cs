namespace SnmpSharpNet.Tests;

public class VbTests
{
    private static Vb GetInitial()
    {
        var pdu = new Vb("1.2.3.4");
        return pdu;
    }

    [Test]
    public async Task EncodedToDecode()
    {
        var initial = GetInitial();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.Encode(buffer);
        var @new = new Vb();
        @new.Decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}