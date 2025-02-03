using System.Text;
using BenchmarkDotNet.Attributes;

namespace SnmpSharpNet.Tests;

[MemoryDiagnoser]
public class OctetStringBenchmarks
{
    [Params(4, 64, 1024)] public int Value { get; set; }

    private OctetString initial = new();
    private OctetString @new = new();

    [GlobalSetup]
    public void Setup()
    {
        var buffer = new byte[Value];
        Random.Shared.NextBytes(buffer);
        initial = new OctetString(buffer);
        @new = new OctetString();
    }

    [Benchmark(Baseline = true)]
    public OctetString EncodedToDecodeMutable()
    {
        var buffer = new MutableByte();
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }

    [Benchmark]
    public OctetString EncodedToDecode()
    {
        Span<byte> buffer = stackalloc byte[Value + 10];
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }
}