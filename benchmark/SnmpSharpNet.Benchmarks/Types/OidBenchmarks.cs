using System.Text;
using BenchmarkDotNet.Attributes;

namespace SnmpSharpNet.Tests;

[MemoryDiagnoser]
public class OidBenchmarks
{
    private Oid initial = new();
    private Oid @new = new();
    private const string Value = "1.22.333.4444.55555.666666.7777777.88888888.999999999";

    [GlobalSetup]
    public void Setup()
    {
        initial = new Oid();
        @new = new Oid();
    }

    [Benchmark(Baseline = true)]
    public Oid EncodedToDecodeMutable()
    {
        var buffer = new MutableByte();
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }

    [Benchmark]
    public Oid EncodedToDecode()
    {
        Span<byte> buffer = stackalloc byte[Value.Length * 2];
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }
}