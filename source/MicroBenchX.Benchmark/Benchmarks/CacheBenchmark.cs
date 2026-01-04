using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark.Benchmarks;

/// <summary>
/// Specifies which cache level to stress.
/// </summary>
public enum CacheLevel
{
    /// <summary>
    /// Target L2 cache (256KB - 1MB typical).
    /// </summary>
    L2,

    /// <summary>
    /// Target L3 cache (8MB - 64MB typical).
    /// </summary>
    L3
}

/// <summary>
/// Cache-heavy benchmark that stresses L2 or L3 cache with random access patterns.
/// Uses pointer chasing to measure cache latency and bandwidth.
/// </summary>
public class CacheBenchmark : ComputeBenchmarkBase
{
    // Default sizes - can be adjusted based on actual cache sizes
    private const int DefaultL2SizeBytes = 512 * 1024;     // 512 KB (fits in most L2 caches)
    private const int DefaultL3SizeBytes = 16 * 1024 * 1024; // 16 MB (fits in most L3 caches)
    private const int DefaultIterations = 500_000_000; // ~5-10 seconds per thread

    private readonly CacheLevel _cacheLevel;
    private readonly int _bufferSizeBytes;
    private readonly int _iterations;

    public override string Id => $"cache-{_cacheLevel.ToString().ToLowerInvariant()}";
    public override string Name => $"{_cacheLevel} Cache Stress";
    public override string Description => $"Cache-heavy benchmark stressing {_cacheLevel} cache with random access patterns.";
    public override BenchmarkType Type => BenchmarkType.MemoryLatency;

    public CacheBenchmark(
        ILogger<CacheBenchmark> logger,
        CoreAffinityConfig affinityConfig,
        CacheLevel cacheLevel,
        int? bufferSizeBytes = null,
        int iterations = DefaultIterations)
        : base(logger, affinityConfig)
    {
        _cacheLevel = cacheLevel;
        _bufferSizeBytes = bufferSizeBytes ?? (cacheLevel == CacheLevel.L2 ? DefaultL2SizeBytes : DefaultL3SizeBytes);
        _iterations = iterations;
    }

    protected override double RunWorkload(int threadIndex, int coreIndex, CancellationToken cancellationToken)
    {
        // Each thread gets its own buffer to avoid false sharing
        var elementCount = _bufferSizeBytes / sizeof(long);
        var buffer = new long[elementCount];

        // Create a random permutation for pointer chasing
        // This ensures we access all elements in a pseudo-random order
        CreatePointerChasePattern(buffer);

        // Warm up the cache
        WarmUpCache(buffer);

        var sw = Stopwatch.StartNew();

        // Run the pointer chase benchmark
        var checksum = RunPointerChase(buffer, _iterations, cancellationToken);

        sw.Stop();

        // Prevent optimization
        if (checksum == long.MaxValue) throw new InvalidOperationException("Unexpected checksum");

        // Calculate bandwidth: iterations * 8 bytes per access
        var totalBytes = (long)_iterations * sizeof(long);
        var seconds = sw.Elapsed.TotalSeconds;
        var bandwidthGBps = totalBytes / seconds / 1e9;

        // Also calculate latency per access
        var latencyNs = seconds * 1e9 / _iterations;

        // Store latency in additional data (we'll return bandwidth as main score)
        // Note: In real impl, we'd use BenchmarkResult.AdditionalData

        return bandwidthGBps;
    }

    private static void CreatePointerChasePattern(long[] buffer)
    {
        var rng = new Random(42); // Fixed seed for reproducibility
        var indices = Enumerable.Range(0, buffer.Length).ToArray();

        // Fisher-Yates shuffle
        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        // Create pointer chain: each element points to the next in the shuffled sequence
        for (int i = 0; i < indices.Length - 1; i++)
        {
            buffer[indices[i]] = indices[i + 1];
        }
        // Last element points back to first
        buffer[indices[^1]] = indices[0];
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WarmUpCache(long[] buffer)
    {
        // Touch all elements to bring them into cache
        long sum = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            sum += buffer[i];
        }
        // Prevent optimization
        if (sum == long.MinValue) throw new InvalidOperationException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long RunPointerChase(long[] buffer, int iterations, CancellationToken cancellationToken)
    {
        long index = 0;
        const int checkInterval = 1_000_000;

        for (int i = 0; i < iterations; i++)
        {
            // Chase the pointer
            index = buffer[index];

            if (i % checkInterval == 0 && cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        return index;
    }

    protected override string GetScoreUnit() => "GB/s";

    protected override double AggregateResults(double[] threadScores)
    {
        // Sum bandwidth across all threads
        return threadScores.Sum();
    }
}
