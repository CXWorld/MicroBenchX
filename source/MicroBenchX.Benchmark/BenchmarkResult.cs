namespace MicroBenchX.Benchmark;

/// <summary>
/// Represents the result of a benchmark execution.
/// </summary>
public class BenchmarkResult
{
    /// <summary>
    /// Gets or sets the benchmark score (higher is better).
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Gets or sets the unit of the score (e.g., "IPC", "ns", "GB/s").
    /// </summary>
    public string ScoreUnit { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total runtime of the benchmark.
    /// </summary>
    public TimeSpan Runtime { get; set; }

    /// <summary>
    /// Gets or sets when the benchmark started.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets when the benchmark ended.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets or sets the final state of the benchmark.
    /// </summary>
    public BenchmarkState State { get; set; }

    /// <summary>
    /// Gets or sets an error message if the benchmark failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets additional data specific to the benchmark type.
    /// </summary>
    public Dictionary<string, object> AdditionalData { get; set; } = [];
}
