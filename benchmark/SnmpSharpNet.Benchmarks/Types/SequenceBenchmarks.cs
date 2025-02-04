using System.Text;
using BenchmarkDotNet.Attributes;

namespace SnmpSharpNet.Tests;

[MemoryDiagnoser]
public class SequenceBenchmarks
{
    [Params(4, 64, 1024)] public int Value { get; set; }

    private Sequence initial = new();
    private Sequence @new = new();

    [GlobalSetup]
    public void Setup()
    {
        var buffer = new byte[Value];
        Random.Shared.NextBytes(buffer);
        initial = new Sequence(buffer);
        @new = new Sequence();
    }

    [Benchmark(Baseline = true)]
    public Sequence EncodedToDecodeMutable()
    {
        var buffer = new MutableByte();
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }

    [Benchmark]
    public Sequence EncodedToDecode()
    {
        Span<byte> buffer = stackalloc byte[Value + 10];
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }
}