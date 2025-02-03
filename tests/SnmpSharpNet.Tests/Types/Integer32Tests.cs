namespace SnmpSharpNet.Tests;

public class Integer32Tests
{
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    public async Task EncodedToDecode(int a)
    {
        var initial = new Integer32(a);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Integer32();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}