using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark.Benchmarks;

/// <summary>
/// AVX2 instruction-heavy benchmark that maximizes instruction throughput.
/// Uses fused multiply-add (FMA) operations to stress the CPU's execution units.
/// </summary>
public class Avx2ComputeBenchmark : ComputeBenchmarkBase
{
    private const int DefaultIterations = 500_000_000; // ~5-10 seconds per thread
    private const int OperationsPerIteration = 16; // 16 FMA operations per iteration

    private readonly int _iterations;

    public override string Id => "avx2-compute";
    public override string Name => "AVX2 Compute";
    public override string Description => "Instruction-heavy benchmark using AVX2 FMA operations to stress CPU execution units.";
    public override BenchmarkType Type => BenchmarkType.Ppc;

    public Avx2ComputeBenchmark(ILogger<Avx2ComputeBenchmark> logger, CoreAffinityConfig affinityConfig, int iterations = DefaultIterations)
        : base(logger, affinityConfig)
    {
        if (!Avx2.IsSupported)
        {
            throw new PlatformNotSupportedException("AVX2 is not supported on this CPU.");
        }

        if (!Fma.IsSupported)
        {
            throw new PlatformNotSupportedException("FMA is not supported on this CPU.");
        }

        _iterations = iterations;
    }

    protected override double RunWorkload(int threadIndex, int coreIndex, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();

        // Run the AVX2 FMA workload
        var result = RunAvx2FmaWorkload(_iterations, cancellationToken);

        sw.Stop();

        // Prevent optimization from removing the computation
        if (result == 0) throw new InvalidOperationException("Unexpected zero result");

        // Calculate GFLOPS: iterations * operations per iteration * 8 floats per vector * 2 (mul+add in FMA)
        var totalOperations = (long)_iterations * OperationsPerIteration * 8 * 2;
        var seconds = sw.Elapsed.TotalSeconds;
        var gflops = totalOperations / seconds / 1e9;

        return gflops;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static float RunAvx2FmaWorkload(int iterations, CancellationToken cancellationToken)
    {
        // Initialize vectors with different values to prevent optimization
        var v0 = Vector256.Create(1.0f);
        var v1 = Vector256.Create(1.0001f);
        var v2 = Vector256.Create(1.0002f);
        var v3 = Vector256.Create(1.0003f);
        var v4 = Vector256.Create(1.0004f);
        var v5 = Vector256.Create(1.0005f);
        var v6 = Vector256.Create(1.0006f);
        var v7 = Vector256.Create(1.0007f);
        var v8 = Vector256.Create(1.0008f);
        var v9 = Vector256.Create(1.0009f);
        var v10 = Vector256.Create(1.0010f);
        var v11 = Vector256.Create(1.0011f);
        var v12 = Vector256.Create(1.0012f);
        var v13 = Vector256.Create(1.0013f);
        var v14 = Vector256.Create(1.0014f);
        var v15 = Vector256.Create(1.0015f);

        var multiplier = Vector256.Create(0.9999f);
        var addend = Vector256.Create(0.0001f);

        // Check cancellation periodically
        const int checkInterval = 1_000_000;

        for (int i = 0; i < iterations; i++)
        {
            // 16 independent FMA operations per iteration
            // Using independent chains to maximize instruction-level parallelism
            v0 = Fma.MultiplyAdd(v0, multiplier, addend);
            v1 = Fma.MultiplyAdd(v1, multiplier, addend);
            v2 = Fma.MultiplyAdd(v2, multiplier, addend);
            v3 = Fma.MultiplyAdd(v3, multiplier, addend);
            v4 = Fma.MultiplyAdd(v4, multiplier, addend);
            v5 = Fma.MultiplyAdd(v5, multiplier, addend);
            v6 = Fma.MultiplyAdd(v6, multiplier, addend);
            v7 = Fma.MultiplyAdd(v7, multiplier, addend);
            v8 = Fma.MultiplyAdd(v8, multiplier, addend);
            v9 = Fma.MultiplyAdd(v9, multiplier, addend);
            v10 = Fma.MultiplyAdd(v10, multiplier, addend);
            v11 = Fma.MultiplyAdd(v11, multiplier, addend);
            v12 = Fma.MultiplyAdd(v12, multiplier, addend);
            v13 = Fma.MultiplyAdd(v13, multiplier, addend);
            v14 = Fma.MultiplyAdd(v14, multiplier, addend);
            v15 = Fma.MultiplyAdd(v15, multiplier, addend);

            if (i % checkInterval == 0 && cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }

        // Combine all results to prevent dead code elimination
        var sum = Avx.Add(v0, v1);
        sum = Avx.Add(sum, v2);
        sum = Avx.Add(sum, v3);
        sum = Avx.Add(sum, v4);
        sum = Avx.Add(sum, v5);
        sum = Avx.Add(sum, v6);
        sum = Avx.Add(sum, v7);
        sum = Avx.Add(sum, v8);
        sum = Avx.Add(sum, v9);
        sum = Avx.Add(sum, v10);
        sum = Avx.Add(sum, v11);
        sum = Avx.Add(sum, v12);
        sum = Avx.Add(sum, v13);
        sum = Avx.Add(sum, v14);
        sum = Avx.Add(sum, v15);

        // Extract a scalar to return
        return sum.GetElement(0);
    }

    protected override string GetScoreUnit() => "GFLOPS";

    protected override double AggregateResults(double[] threadScores)
    {
        // Sum GFLOPS across all threads
        return threadScores.Sum();
    }
}
