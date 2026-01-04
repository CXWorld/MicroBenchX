using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark;

/// <summary>
/// Base class for compute benchmarks that run on specific cores.
/// </summary>
public abstract class ComputeBenchmarkBase : BenchmarkBase
{
    private readonly CoreAffinityConfig _affinityConfig;

    /// <summary>
    /// Gets the core affinity configuration.
    /// </summary>
    public CoreAffinityConfig AffinityConfig => _affinityConfig;

    protected ComputeBenchmarkBase(ILogger logger, CoreAffinityConfig affinityConfig)
        : base(logger)
    {
        _affinityConfig = affinityConfig;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var threadCount = _affinityConfig.ThreadCount;
        var results = new ConcurrentBag<double>();
        var threads = new Thread[threadCount];
        var startBarrier = new Barrier(threadCount + 1); // +1 for main thread
        var exceptions = new ConcurrentBag<Exception>();

        for (int i = 0; i < threadCount; i++)
        {
            int threadIndex = i;
            int coreIndex = _affinityConfig.Mode == CoreAffinityMode.Auto
                ? -1
                : _affinityConfig.CoreIndices[threadIndex];

            threads[i] = new Thread(() =>
            {
                try
                {
                    // Set affinity if needed
                    if (coreIndex >= 0 && OperatingSystem.IsWindows())
                    {
                        ThreadAffinityHelper.SetCurrentThreadAffinity(coreIndex);
                    }

                    // Wait for all threads to be ready
                    startBarrier.SignalAndWait();

                    // Run the benchmark workload
                    var score = RunWorkload(threadIndex, coreIndex, cancellationToken);
                    results.Add(score);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancelled
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            })
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };

            threads[i].Start();
        }

        // Signal all threads to start simultaneously
        startBarrier.SignalAndWait();

        // Wait for all threads to complete
        await Task.Run(() =>
        {
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }, cancellationToken);

        if (exceptions.Count > 0)
        {
            throw new AggregateException("One or more benchmark threads failed.", exceptions);
        }

        // Aggregate results
        CurrentScore = AggregateResults(results.ToArray());
    }

    /// <summary>
    /// Runs the benchmark workload on a single thread.
    /// </summary>
    /// <param name="threadIndex">The index of this thread (0 to ThreadCount-1).</param>
    /// <param name="coreIndex">The core this thread is pinned to, or -1 for auto.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The score for this thread.</returns>
    protected abstract double RunWorkload(int threadIndex, int coreIndex, CancellationToken cancellationToken);

    /// <summary>
    /// Aggregates the results from all threads into a single score.
    /// Default implementation sums all scores.
    /// </summary>
    /// <param name="threadScores">Scores from each thread.</param>
    /// <returns>The aggregated score.</returns>
    protected virtual double AggregateResults(double[] threadScores)
    {
        return threadScores.Sum();
    }
}
