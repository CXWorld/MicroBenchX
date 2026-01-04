namespace MicroBenchX.Benchmark;

/// <summary>
/// Defines the contract for all benchmarks.
/// </summary>
public interface IBenchmark
{
    /// <summary>
    /// Gets the unique identifier of the benchmark.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the display name of the benchmark.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets a description of what the benchmark measures.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Gets the type/category of the benchmark.
    /// </summary>
    BenchmarkType Type { get; }

    /// <summary>
    /// Gets the current state of the benchmark.
    /// </summary>
    BenchmarkState State { get; }

    /// <summary>
    /// Gets the current score (available during and after execution).
    /// </summary>
    double CurrentScore { get; }

    /// <summary>
    /// Gets the elapsed runtime since the benchmark started.
    /// </summary>
    TimeSpan ElapsedTime { get; }

    /// <summary>
    /// Gets the result of the last benchmark execution.
    /// </summary>
    BenchmarkResult? LastResult { get; }

    /// <summary>
    /// Starts the benchmark execution asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The benchmark result.</returns>
    Task<BenchmarkResult> StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Aborts the currently running benchmark.
    /// </summary>
    void Abort();

    /// <summary>
    /// Event raised when the benchmark state changes.
    /// </summary>
    event EventHandler<BenchmarkState>? StateChanged;

    /// <summary>
    /// Event raised when progress is updated during execution.
    /// </summary>
    event EventHandler<BenchmarkProgressEventArgs>? ProgressChanged;
}
