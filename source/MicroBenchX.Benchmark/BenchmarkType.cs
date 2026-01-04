namespace MicroBenchX.Benchmark;

/// <summary>
/// Defines the category of a benchmark.
/// </summary>
public enum BenchmarkType
{
    /// <summary>
    /// Performance Per Clock benchmarks.
    /// </summary>
    Ppc,

    /// <summary>
    /// Instructions Per Cycle benchmarks.
    /// </summary>
    Ipc,

    /// <summary>
    /// Core-to-core latency benchmarks.
    /// </summary>
    CoreToCore,

    /// <summary>
    /// Memory subsystem latency benchmarks.
    /// </summary>
    MemoryLatency
}
