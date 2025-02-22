using System.Net;

namespace SnmpSharpNet.Tests;

public class TrapPduTests
{
    [Test]
    public async Task EncodedToDecodeMutable()
    {
        var initial = GetInitial();
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new TrapPdu();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }

    private static TrapPdu GetInitial()
    {
        var pdu = new TrapPdu();
        pdu.Enterprise.Set(new Oid("1.3.1.4.2.5"));
        pdu.AgentAddress.Set(IPAddress.Parse("1.2.3.4"));
        pdu.Generic = 2;
        pdu.Specific = 3;
        pdu.TimeStamp = 4;
        pdu.VbList.Add(new Oid("1.3.1.4.2.5"));
        pdu.VbList.Add(new Oid("1.3.1.42.2.1.412.12.4.1"));
        return pdu;
    }

    [Test]
    public async Task EncodedToDecode()
    {
        var initial = GetInitial();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new TrapPdu();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }

    [Test]
    public async Task BothMethodsShouldProduceEqualBuffers()
    {
        var initial = GetInitial();
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var arr = buffer.ToArray();
        var secondBuffer = new MutableByte();
        initial.encode(secondBuffer);
        for (int i = 0; i < arr.Length; i++)
        {
            await Assert.That(arr[i]).IsEqualTo(secondBuffer[i]);
        }
    }
}