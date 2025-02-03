namespace SnmpSharpNet.Tests;

public class OctetStringTests
{
    [Test]
    [Arguments("yooooo")]
    public async Task EncodedToDecode(string a)
    {
        var initial = new OctetString(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new OctetString();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}