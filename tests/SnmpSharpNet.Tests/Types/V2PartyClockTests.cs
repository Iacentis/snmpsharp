namespace SnmpSharpNet.Tests;

public class V2PartyClockTests
{
    [Test]
    [Arguments(1u)]
    [Arguments(2u)]
    [Arguments(3u)]
    public async Task EncodedToDecodeMutable(uint a)
    {
        var initial = new V2PartyClock(new UInteger32(a));
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new V2PartyClock();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }

    [Test]
    [Arguments(1u)]
    [Arguments(2u)]
    [Arguments(3u)]
    public async Task EncodedToDecode(uint a)
    {
        Span<byte> buffer = stackalloc byte[UInteger32.MaxEncodedSize];
        var initial = new V2PartyClock(new UInteger32(a));
        initial.encode(buffer);
        var @new = new V2PartyClock();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}