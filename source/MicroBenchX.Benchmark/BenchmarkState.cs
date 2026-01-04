namespace MicroBenchX.Benchmark;

/// <summary>
/// Represents the current state of a benchmark.
/// </summary>
public enum BenchmarkState
{
    /// <summary>
    /// Benchmark is ready to start.
    /// </summary>
    Idle,

    /// <summary>
    /// Benchmark is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// Benchmark completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Benchmark was aborted by user.
    /// </summary>
    Aborted,

    /// <summary>
    /// Benchmark failed with an error.
    /// </summary>
    Failed
}
