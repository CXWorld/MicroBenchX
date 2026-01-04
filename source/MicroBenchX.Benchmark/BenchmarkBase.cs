using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark;

/// <summary>
/// Base class for all benchmarks providing common functionality.
/// </summary>
public abstract class BenchmarkBase : IBenchmark
{
    private readonly ILogger _logger;
    private readonly Stopwatch _stopwatch = new();
    private CancellationTokenSource? _abortTokenSource;
    private BenchmarkState _state = BenchmarkState.Idle;
    private double _currentScore;

    protected BenchmarkBase(ILogger logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public abstract string Id { get; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public abstract BenchmarkType Type { get; }

    /// <inheritdoc />
    public BenchmarkState State
    {
        get => _state;
        protected set
        {
            if (_state != value)
            {
                _state = value;
                StateChanged?.Invoke(this, value);
            }
        }
    }

    /// <inheritdoc />
    public double CurrentScore
    {
        get => _currentScore;
        protected set => _currentScore = value;
    }

    /// <inheritdoc />
    public TimeSpan ElapsedTime => _stopwatch.Elapsed;

    /// <inheritdoc />
    public BenchmarkResult? LastResult { get; protected set; }

    /// <inheritdoc />
    public event EventHandler<BenchmarkState>? StateChanged;

    /// <inheritdoc />
    public event EventHandler<BenchmarkProgressEventArgs>? ProgressChanged;

    /// <inheritdoc />
    public async Task<BenchmarkResult> StartAsync(CancellationToken cancellationToken = default)
    {
        if (State == BenchmarkState.Running)
        {
            throw new InvalidOperationException("Benchmark is already running.");
        }

        _abortTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _abortTokenSource.Token;

        var result = new BenchmarkResult
        {
            StartTime = DateTime.UtcNow
        };

        try
        {
            State = BenchmarkState.Running;
            _currentScore = 0;
            _stopwatch.Restart();

            _logger.LogInformation("Starting benchmark: {Name} ({Id})", Name, Id);

            await ExecuteAsync(token);

            _stopwatch.Stop();
            result.EndTime = DateTime.UtcNow;
            result.Runtime = _stopwatch.Elapsed;
            result.Score = _currentScore;
            result.ScoreUnit = GetScoreUnit();
            result.State = BenchmarkState.Completed;

            State = BenchmarkState.Completed;
            _logger.LogInformation("Benchmark completed: {Name}, Score: {Score} {Unit}, Runtime: {Runtime}",
                Name, result.Score, result.ScoreUnit, result.Runtime);
        }
        catch (OperationCanceledException)
        {
            _stopwatch.Stop();
            result.EndTime = DateTime.UtcNow;
            result.Runtime = _stopwatch.Elapsed;
            result.Score = _currentScore;
            result.ScoreUnit = GetScoreUnit();
            result.State = BenchmarkState.Aborted;

            State = BenchmarkState.Aborted;
            _logger.LogWarning("Benchmark aborted: {Name}", Name);
        }
        catch (Exception ex)
        {
            _stopwatch.Stop();
            result.EndTime = DateTime.UtcNow;
            result.Runtime = _stopwatch.Elapsed;
            result.Score = 0;
            result.ScoreUnit = GetScoreUnit();
            result.State = BenchmarkState.Failed;
            result.ErrorMessage = ex.Message;

            State = BenchmarkState.Failed;
            _logger.LogError(ex, "Benchmark failed: {Name}", Name);
        }
        finally
        {
            _abortTokenSource?.Dispose();
            _abortTokenSource = null;
        }

        LastResult = result;
        return result;
    }

    /// <inheritdoc />
    public void Abort()
    {
        if (State == BenchmarkState.Running)
        {
            _logger.LogInformation("Aborting benchmark: {Name}", Name);
            _abortTokenSource?.Cancel();
        }
    }

    /// <summary>
    /// Executes the benchmark logic. Override this in derived classes.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Gets the unit for the score (e.g., "IPC", "ns", "GB/s").
    /// </summary>
    protected abstract string GetScoreUnit();

    /// <summary>
    /// Reports progress during benchmark execution.
    /// </summary>
    protected void ReportProgress(double progressPercent, double currentScore, string? message = null)
    {
        _currentScore = currentScore;
        ProgressChanged?.Invoke(this, new BenchmarkProgressEventArgs(progressPercent, currentScore, message));
    }
}
