namespace SnmpSharpNet.Tests;

public class NoSuchObjectTests
{
    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new NoSuchObject();
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new NoSuchObject();
        @new.decode(buffer, 0);
        await Assert.That(@new.ToString()).IsEqualTo(initial.ToString());
    }
}