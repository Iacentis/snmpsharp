namespace SnmpSharpNet.Tests;

public class Counter64Tests
{
    [Test]
    [Arguments(1u)]
    [Arguments(2u)]
    [Arguments(3u)]
    public async Task EncodedToDecode(uint a)
    {
        var initial = new Counter64(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Counter64();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}