namespace SnmpSharpNet.Tests;

public class V2ErrorTests
{
    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new V2Error();
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new V2Error();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}