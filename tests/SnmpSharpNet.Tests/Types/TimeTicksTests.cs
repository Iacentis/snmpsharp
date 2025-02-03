namespace SnmpSharpNet.Tests;

public class TimeTicksTests
{
    [Test]
    [Arguments(1u)]
    [Arguments(2u)]
    [Arguments(3u)]
    public async Task EncodedToDecode(uint a)
    {
        var initial = new TimeTicks(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new TimeTicks();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}