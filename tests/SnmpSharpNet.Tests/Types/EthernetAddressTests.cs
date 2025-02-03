namespace SnmpSharpNet.Tests;

public class EthernetAddressTests
{
    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new EthernetAddress([1, 2, 3, 4, 5, 6]);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new EthernetAddress();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}