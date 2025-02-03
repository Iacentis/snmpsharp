namespace SnmpSharpNet.Tests;

public class UserSecurityModelTests
{
    [Test]
    public async Task EncodedToDecodeMutable()
    {
        var buffer = new MutableByte();
        var initial = new UserSecurityModel
        {
            EngineTime = 2,
            EngineBoots = 3
        };
        initial.encode(buffer);
        var @new = new UserSecurityModel();
        @new.decode(buffer, 0);
        await Assert.That(@new.EngineTime).IsEqualTo(initial.EngineTime);
        await Assert.That(@new.EngineBoots).IsEqualTo(initial.EngineBoots);
    }

    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new UserSecurityModel
        {
            EngineTime = 2,
            EngineBoots = 3,
        };
        Span<byte> buffer = stackalloc byte[initial.ByteLength];
        initial.encode(buffer);
        var @new = new UserSecurityModel();
        @new.decode(buffer, 0);
        await Assert.That(@new.EngineTime).IsEqualTo(initial.EngineTime);
        await Assert.That(@new.EngineBoots).IsEqualTo(initial.EngineBoots);
    }
}