namespace SnmpSharpNet.Tests;

public class ScopedPduTests
{
    private static ScopedPdu GetInitial()
    {
        var pdu = new ScopedPdu(PduType.Get)
        {
            RequestId = Random.Shared.Next(),
        };
        pdu.ContextEngineId.Set("Something");
        pdu.ContextName.Set("SomethingElse");
        pdu.VbList.Add(new Oid("1.3.1.4.2.5"));
        pdu.VbList.Add(new Oid("1.3.1.42.2.1.412.12.4.1"));
        return pdu;
    }

    [Test]
    public async Task EncodedToDecode()
    {
        var initial = GetInitial();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.Encode(buffer);
        var @new = new ScopedPdu();
        @new.Decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}