using System.Text;
using BenchmarkDotNet.Attributes;

namespace SnmpSharpNet.Tests;

[MemoryDiagnoser]
public class UInteger32Benchmarks
{
    [Params(uint.MinValue, uint.MaxValue / 2, uint.MaxValue)]
    public uint Value { get; set; }

    private UInteger32 initial = new();
    private UInteger32 @new = new();

    [GlobalSetup]
    public void Setup()
    {
        initial = new UInteger32(Value);
        @new = new UInteger32();
    }

    [Benchmark(Baseline = true)]
    public UInteger32 EncodedToDecodeMutable()
    {
        var buffer = new MutableByte();
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }

    [Benchmark]
    public UInteger32 EncodedToDecode()
    {
        Span<byte> buffer = stackalloc byte[Integer32.MaxEncodedSize];
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }
}