using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark.Benchmarks;

/// <summary>
/// Specifies the memory access pattern.
/// </summary>
public enum MemoryAccessPattern
{
    /// <summary>
    /// Sequential read (streaming).
    /// </summary>
    SequentialRead,

    /// <summary>
    /// Sequential write (streaming).
    /// </summary>
    SequentialWrite,

    /// <summary>
    /// Sequential read-modify-write.
    /// </summary>
    SequentialReadWrite,

    /// <summary>
    /// Random read access.
    /// </summary>
    RandomRead
}

/// <summary>
/// Memory bandwidth benchmark that stresses main memory with large data sets.
/// Uses buffers larger than L3 cache to measure actual RAM bandwidth.
/// </summary>
public class MemoryBandwidthBenchmark : ComputeBenchmarkBase
{
    // Default: 512 MB per thread (should exceed any L3 cache)
    private const int DefaultBufferSizeMB = 512;
    private const int DefaultIterations = 20; // ~5-10 seconds per thread

    private readonly int _bufferSizeMB;
    private readonly int _iterations;
    private readonly MemoryAccessPattern _accessPattern;

    public override string Id => $"memory-{_accessPattern.ToString().ToLowerInvariant()}";
    public override string Name => $"Memory Bandwidth ({_accessPattern})";
    public override string Description => $"Memory bandwidth benchmark using {_accessPattern} access pattern on large buffers.";
    public override BenchmarkType Type => BenchmarkType.MemoryLatency;

    public MemoryBandwidthBenchmark(
        ILogger<MemoryBandwidthBenchmark> logger,
        CoreAffinityConfig affinityConfig,
        MemoryAccessPattern accessPattern = MemoryAccessPattern.SequentialRead,
        int bufferSizeMB = DefaultBufferSizeMB,
        int iterations = DefaultIterations)
        : base(logger, affinityConfig)
    {
        _bufferSizeMB = bufferSizeMB;
        _iterations = iterations;
        _accessPattern = accessPattern;
    }

    protected override double RunWorkload(int threadIndex, int coreIndex, CancellationToken cancellationToken)
    {
        var bufferSizeBytes = _bufferSizeMB * 1024 * 1024;
        var buffer = GC.AllocateArray<byte>(bufferSizeBytes, pinned: true);

        // Initialize buffer with pattern
        InitializeBuffer(buffer);

        // Flush caches by reading a second large buffer
        FlushCaches(bufferSizeBytes);

        var sw = Stopwatch.StartNew();
        long totalBytesProcessed = 0;

        for (int iter = 0; iter < _iterations; iter++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            long bytesThisIteration = _accessPattern switch
            {
                MemoryAccessPattern.SequentialRead => RunSequentialRead(buffer),
                MemoryAccessPattern.SequentialWrite => RunSequentialWrite(buffer),
                MemoryAccessPattern.SequentialReadWrite => RunSequentialReadWrite(buffer),
                MemoryAccessPattern.RandomRead => RunRandomRead(buffer),
                _ => throw new NotSupportedException()
            };

            totalBytesProcessed += bytesThisIteration;
        }

        sw.Stop();

        var seconds = sw.Elapsed.TotalSeconds;
        var bandwidthGBps = totalBytesProcessed / seconds / 1e9;

        return bandwidthGBps;
    }

    private static void InitializeBuffer(byte[] buffer)
    {
        var rng = new Random(42);
        // Initialize with random data to prevent any compression optimizations
        for (int i = 0; i < Math.Min(buffer.Length, 1024 * 1024); i += 64)
        {
            buffer[i] = (byte)rng.Next(256);
        }
    }

    private static void FlushCaches(int size)
    {
        // Allocate and touch a second buffer to flush caches
        var flushBuffer = new byte[size];
        long sum = 0;
        for (int i = 0; i < flushBuffer.Length; i += 64)
        {
            sum += flushBuffer[i];
        }
        // Prevent optimization
        if (sum == long.MinValue) throw new InvalidOperationException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long RunSequentialRead(byte[] buffer)
    {
        long sum = 0;

        if (Avx2.IsSupported)
        {
            // Use AVX2 for maximum bandwidth
            var span = MemoryMarshal.Cast<byte, Vector256<long>>(buffer.AsSpan());
            var accumulator = Vector256<long>.Zero;

            for (int i = 0; i < span.Length; i++)
            {
                accumulator = Avx2.Add(accumulator, span[i]);
            }

            sum = accumulator.GetElement(0) + accumulator.GetElement(1) +
                  accumulator.GetElement(2) + accumulator.GetElement(3);
        }
        else
        {
            // Fallback to scalar
            var longSpan = MemoryMarshal.Cast<byte, long>(buffer.AsSpan());
            for (int i = 0; i < longSpan.Length; i++)
            {
                sum += longSpan[i];
            }
        }

        // Prevent optimization
        if (sum == long.MaxValue) throw new InvalidOperationException();

        return buffer.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long RunSequentialWrite(byte[] buffer)
    {
        if (Avx2.IsSupported)
        {
            var span = MemoryMarshal.Cast<byte, Vector256<long>>(buffer.AsSpan());
            var value = Vector256.Create(0x0102030405060708L);

            for (int i = 0; i < span.Length; i++)
            {
                span[i] = value;
            }
        }
        else
        {
            var longSpan = MemoryMarshal.Cast<byte, long>(buffer.AsSpan());
            for (int i = 0; i < longSpan.Length; i++)
            {
                longSpan[i] = 0x0102030405060708L;
            }
        }

        return buffer.Length;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long RunSequentialReadWrite(byte[] buffer)
    {
        if (Avx2.IsSupported)
        {
            var span = MemoryMarshal.Cast<byte, Vector256<long>>(buffer.AsSpan());
            var addend = Vector256.Create(1L);

            for (int i = 0; i < span.Length; i++)
            {
                span[i] = Avx2.Add(span[i], addend);
            }
        }
        else
        {
            var longSpan = MemoryMarshal.Cast<byte, long>(buffer.AsSpan());
            for (int i = 0; i < longSpan.Length; i++)
            {
                longSpan[i]++;
            }
        }

        // Read + Write = 2x the buffer size
        return buffer.Length * 2L;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long RunRandomRead(byte[] buffer)
    {
        long sum = 0;
        var rng = new Random(42);
        var longSpan = MemoryMarshal.Cast<byte, long>(buffer.AsSpan());
        var accessCount = longSpan.Length;

        // Pre-generate random indices for consistent behavior
        var indices = new int[accessCount];
        for (int i = 0; i < accessCount; i++)
        {
            indices[i] = rng.Next(longSpan.Length);
        }

        for (int i = 0; i < accessCount; i++)
        {
            sum += longSpan[indices[i]];
        }

        // Prevent optimization
        if (sum == long.MaxValue) throw new InvalidOperationException();

        // Random read touches accessCount * 8 bytes
        return (long)accessCount * sizeof(long);
    }

    protected override string GetScoreUnit() => "GB/s";

    protected override double AggregateResults(double[] threadScores)
    {
        // Sum bandwidth across all threads
        return threadScores.Sum();
    }
}
