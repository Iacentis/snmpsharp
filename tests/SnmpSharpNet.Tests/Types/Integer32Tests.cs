using System.Text;

namespace SnmpSharpNet.Tests;

public class Integer32Tests
{
    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(30000000)]
    [Arguments(int.MaxValue)]
    [Arguments(-2)]
    [Arguments(-1)]
    [Arguments(-22)]
    [Arguments(int.MinValue)]
    public async Task EncodedToDecodeMutable(int a)
    {
        var buffer = new MutableByte();
        var initial = new Integer32(a);
        initial.encode(buffer);
        var @new = new Integer32();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(30000000)]
    [Arguments(int.MaxValue)]
    [Arguments(-2)]
    [Arguments(-1)]
    [Arguments(-22)]
    [Arguments(int.MinValue)]
    public async Task EncodedToDecode(int a)
    {
        var initial = new Integer32(a);
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new Integer32();
        @new.decode(buffer, 0);
        await Assert.That(@new).IsEqualTo(initial);
    }
}