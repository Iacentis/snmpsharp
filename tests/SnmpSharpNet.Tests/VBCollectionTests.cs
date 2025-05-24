namespace SnmpSharpNet.Tests;

public class VbCollectionTests
{

    private static VbCollection GetInitial()
    {
        var pdu = new VbCollection
        {
            new Oid("1.3.1"),
            new Oid("1.3.1.42.2.1.412.12.4.1")
        };
        return pdu;
    }

    [Test]
    public async Task EncodedToDecode()
    {
        var initial = GetInitial();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.Encode(buffer);
        var @new = new VbCollection();
        @new.Decode(buffer, 0);
        await Assert.That(@new.Count).IsEqualTo(@new.Count);
        for (int i = 0; i < initial.Count; i++)
        {
            var a = initial[i];
            var b = @new[i];
            await Assert.That(a.ToString()).IsEqualTo(b.ToString());
        }
    }
}