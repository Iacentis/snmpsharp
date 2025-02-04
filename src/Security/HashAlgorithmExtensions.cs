using System;
using System.Buffers;
using System.Security.Cryptography;

namespace SnmpSharpNet;

public static class HashAlgorithmExtensions
{
    public static T? WithHashed<T>(this HashAlgorithm hashAlgorithm, Span<byte> toCompute,
        Func<Span<byte>, int, T> postHashFunction)
    {
        var array = ArrayPool<byte>.Shared.Rent(hashAlgorithm.HashSize);
        var span = array.AsSpan(0, hashAlgorithm.HashSize);
        T? result = default;
        if (hashAlgorithm.TryComputeHash(toCompute, span, out var written))
        {
            result = postHashFunction(span, written);
        }

        ArrayPool<byte>.Shared.Return(array);
        return result;
    }

    public static void WithHashed(this HashAlgorithm hashAlgorithm, Span<byte> toCompute,
        Action<Span<byte>, int> postHashFunction)
    {
        var array = ArrayPool<byte>.Shared.Rent(hashAlgorithm.HashSize);
        var span = array.AsSpan(0, hashAlgorithm.HashSize);
        if (hashAlgorithm.TryComputeHash(toCompute, span, out var written))
        {
            postHashFunction(span, written);
        }

        ArrayPool<byte>.Shared.Return(array);
    }
}