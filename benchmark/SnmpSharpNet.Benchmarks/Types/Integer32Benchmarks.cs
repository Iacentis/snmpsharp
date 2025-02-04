using System.Text;
using BenchmarkDotNet.Attributes;

namespace SnmpSharpNet.Tests;

[MemoryDiagnoser]
public class Integer32Benchmarks
{
    [Params(0, 1, 30000000, int.MaxValue, -2, -1, -22, int.MinValue)]
    public int Value { get; set; }

    private Integer32 initial = new();
    private Integer32 @new = new();

    [GlobalSetup]
    public void Setup()
    {
        initial = new Integer32(Value);
        @new = new Integer32();
    }

    [Benchmark(Baseline = true)]
    public Integer32 EncodedToDecodeMutable()
    {
        var buffer = new MutableByte();
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }

    [Benchmark]
    public Integer32 EncodedToDecode()
    {
        Span<byte> buffer = stackalloc byte[Integer32.MaxEncodedSize];
        initial.encode(buffer);
        @new.decode(buffer, 0);
        return @new;
    }
}