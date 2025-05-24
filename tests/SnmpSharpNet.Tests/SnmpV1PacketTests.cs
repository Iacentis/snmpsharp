
namespace SnmpSharpNet.Tests;

public class SnmpV1PacketTests
{
    [Test]
    public async Task CommunityIsRetrievable()
    {
        var packet = new SnmpV1Packet("public");

        await Assert.That(packet.Community.ToString()).IsEqualTo("public");
    }

    [Test]
    [Arguments(1, 2, 3)]
    public async Task EncodedBytesDecodeToSamePacket(int a, int b, int c)
    {
        var packet = new SnmpV1Packet("public");
        VbCollection vbs =
        [
            new Vb(new Oid("1.3.2"), new Integer32(a)),
            new Vb(new Oid("1.3.3"), new Integer32(b)),
            new Vb(new Oid("1.3.4"), new Integer32(c))
        ];
        packet.Pdu.SetVbList(vbs);
        packet.Pdu.RequestId = a * b * c;
        packet.Pdu.ErrorIndex = a + b + c;
        packet.Pdu.ErrorStatus = a - b + c;
        var bytes = packet.Encode();
        var newPacket = new SnmpV1Packet();
        newPacket.Decode(bytes);
        await Assert.That(newPacket.ToString()).IsEqualTo(packet.ToString());
    }
}