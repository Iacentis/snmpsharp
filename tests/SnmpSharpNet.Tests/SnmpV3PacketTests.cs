namespace SnmpSharpNet.Tests;

public class SnmpV3PacketTests
{
    [Test]
    [Arguments(1, 2, 3)]
    public async Task EncodedBytesDecodeToSamePacket(int a, int b, int c)
    {
        var pdu = new ScopedPdu(PduType.GetNext, a * b * c);
        var parameters = new SecureAgentParameters();
        parameters.EngineId.Set($"{b}");
        parameters.EngineTime.Set($"{a}");
        parameters.EngineBoots.Set($"{c}");
        var packet = new SnmpV3Packet(parameters, pdu);
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
        var bytes = packet.encode();
        var newPacket = new SnmpV3Packet();
        newPacket.decode(bytes, bytes.Length);
        await Assert.That(newPacket.ToString()).IsEqualTo(packet.ToString());
    }
}