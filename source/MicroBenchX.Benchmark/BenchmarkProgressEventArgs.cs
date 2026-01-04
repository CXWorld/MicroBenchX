namespace MicroBenchX.Benchmark;

/// <summary>
/// Event arguments for benchmark progress updates.
/// </summary>
public class BenchmarkProgressEventArgs : EventArgs
{
    /// <summary>
    /// Gets the progress percentage (0-100).
    /// </summary>
    public double ProgressPercent { get; }

    /// <summary>
    /// Gets the current score at this point in execution.
    /// </summary>
    public double CurrentScore { get; }

    /// <summary>
    /// Gets an optional status message.
    /// </summary>
    public string? Message { get; }

    public BenchmarkProgressEventArgs(double progressPercent, double currentScore, string? message = null)
    {
        ProgressPercent = progressPercent;
        CurrentScore = currentScore;
        Message = message;
    }
}
