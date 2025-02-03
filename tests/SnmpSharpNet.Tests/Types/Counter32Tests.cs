namespace SnmpSharpNet.Tests;

public class Counter32Tests
{
    [Test]
    [Arguments(1u)]
    [Arguments(2u)]
    [Arguments(3u)]
    public async Task EncodedToDecode(uint a)
    {
        var initial = new Counter32(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Counter32();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}