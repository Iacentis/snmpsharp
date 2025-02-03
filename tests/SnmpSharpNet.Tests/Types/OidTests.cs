namespace SnmpSharpNet.Tests;

public class OidTests
{
    [Test]
    [Arguments("1.3.2.4.1.5.6.8")]
    public async Task EncodedToDecode(string a)
    {
        var initial = new Oid(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Oid();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}