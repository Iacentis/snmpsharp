namespace SnmpSharpNet.Tests;

public class NullTests
{
    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new Null();
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Null();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}