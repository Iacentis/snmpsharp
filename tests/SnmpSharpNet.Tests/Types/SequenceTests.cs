namespace SnmpSharpNet.Tests;

public class SequenceTests
{
    [Test]
    public async Task EncodedToDecode()
    {
        var initial = new Sequence([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
        var buffer = new MutableByte();
        initial.encode(buffer);
        var @new = new Sequence();
        @new.decode(buffer, 0);
        var seqTree = string.Join(", ", @new.Value);
        var inTree = string.Join(", ", initial.Value);
        await Assert.That(seqTree).IsEqualTo(inTree);
    }
}