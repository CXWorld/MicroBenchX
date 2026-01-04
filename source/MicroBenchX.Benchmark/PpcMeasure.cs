using System.Collections.Concurrent;
using LibreHardwareMonitor.Hardware;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark;

/// <summary>
/// Records for storing frequency samples.
/// </summary>
public readonly record struct FrequencySample(int CoreIndex, double FrequencyMHz, DateTime Timestamp);

/// <summary>
/// Result of PPC measurement containing frequency statistics and PPC value.
/// </summary>
public class PpcMeasureResult
{
    public double BenchmarkScore { get; init; }
    public TimeSpan Duration { get; init; }
    public double AverageFrequencyMHz { get; init; }
    public double MinFrequencyMHz { get; init; }
    public double MaxFrequencyMHz { get; init; }
    public double PerformancePerClock { get; init; }
    public IReadOnlyDictionary<int, double> PerCoreAverageFrequency { get; init; } = new Dictionary<int, double>();
    public int SampleCount { get; init; }
}

/// <summary>
/// Measures Performance Per Clock (PPC) by tracking CPU frequency during benchmark execution.
/// Uses LibreHardwareMonitor for frequency monitoring.
/// </summary>
public sealed class PpcMeasure : IDisposable
{
    private readonly Computer _computer;
    private readonly ILogger<PpcMeasure>? _logger;
    private readonly TimeSpan _pollingInterval;
    private readonly int[] _coreIndices;
    private readonly ConcurrentBag<FrequencySample> _samples = new();
    private readonly CancellationTokenSource _cts = new();

    private Task? _pollingTask;
    private DateTime _startTime;
    private bool _isRunning;
    private bool _disposed;
    private IHardware? _cpu;
    private readonly Dictionary<int, ISensor> _coreClockSensors = new();

    /// <summary>
    /// Creates a new PpcMeasure instance.
    /// </summary>
    /// <param name="coreIndices">Indices of cores to monitor. Empty array means all cores.</param>
    /// <param name="pollingInterval">Interval between frequency samples.</param>
    /// <param name="logger">Optional logger.</param>
    public PpcMeasure(int[] coreIndices, TimeSpan pollingInterval, ILogger<PpcMeasure>? logger = null)
    {
        _coreIndices = coreIndices ?? [];
        _pollingInterval = pollingInterval;
        _logger = logger;

        _computer = new Computer
        {
            IsCpuEnabled = true
        };
    }

    /// <summary>
    /// Creates a PpcMeasure from a CoreAffinityConfig.
    /// </summary>
    public static PpcMeasure FromAffinityConfig(
        CoreAffinityConfig config,
        TimeSpan pollingInterval,
        ILogger<PpcMeasure>? logger = null)
    {
        int[] coreIndices = config.Mode switch
        {
            CoreAffinityMode.SingleCore => config.CoreIndices,
            CoreAffinityMode.SpecificCores => config.CoreIndices,
            CoreAffinityMode.AllCores => Enumerable.Range(0, Environment.ProcessorCount).ToArray(),
            CoreAffinityMode.Auto => [], // Empty means all cores
            _ => []
        };

        return new PpcMeasure(coreIndices, pollingInterval, logger);
    }

    /// <summary>
    /// Gets whether monitoring is currently active.
    /// </summary>
    public bool IsRunning => _isRunning;

    /// <summary>
    /// Gets the current sample count.
    /// </summary>
    public int SampleCount => _samples.Count;

    /// <summary>
    /// Gets the collected frequency samples.
    /// </summary>
    public IReadOnlyCollection<FrequencySample> Samples => _samples.ToArray();

    /// <summary>
    /// Initializes the hardware monitoring. Must be called before Start().
    /// </summary>
    public void Initialize()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _computer.Open();

        // Find the CPU hardware
        _cpu = _computer.Hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);

        if (_cpu == null)
        {
            _logger?.LogWarning("No CPU hardware found");
            return;
        }

        _cpu.Update();

        // Find clock sensors for the specified cores
        var clockSensors = _cpu.Sensors
            .Where(s => s.SensorType == SensorType.Clock)
            .ToList();

        _logger?.LogDebug("Found {Count} clock sensors", clockSensors.Count);

        // Map core indices to sensors
        // Sensor names are typically "Core #0", "Core #1", etc. for regular clock
        // or "Core #0 (Effective)" for effective clock
        foreach (var sensor in clockSensors)
        {
            // Try to extract core index from sensor name
            // Match patterns like "Core #0", "Core #1", etc. (not effective)
            if (sensor.Name.StartsWith("Core #") && !sensor.Name.Contains("Effective"))
            {
                var indexStr = sensor.Name.Replace("Core #", "").Trim();
                if (int.TryParse(indexStr, out int coreIndex))
                {
                    // If specific cores requested, only add those
                    if (_coreIndices.Length == 0 || _coreIndices.Contains(coreIndex))
                    {
                        _coreClockSensors[coreIndex] = sensor;
                        _logger?.LogDebug("Tracking sensor '{Name}' for core {Index}", sensor.Name, coreIndex);
                    }
                }
            }
        }

        // If no per-core sensors found, try to use average/max clock sensors
        if (_coreClockSensors.Count == 0)
        {
            _logger?.LogWarning("No per-core clock sensors found. Trying fallback sensors.");

            // Try "CPU Clock" or similar aggregate sensors
            var fallbackSensor = clockSensors
                .FirstOrDefault(s => s.Name.Contains("CPU Clock") ||
                                    s.Name.Contains("CPU Max") ||
                                    s.Name.Contains("Bus Speed"));

            if (fallbackSensor != null)
            {
                _coreClockSensors[-1] = fallbackSensor; // Use -1 as aggregate indicator
                _logger?.LogDebug("Using fallback sensor '{Name}'", fallbackSensor.Name);
            }
        }

        _logger?.LogInformation("PpcMeasure initialized with {Count} clock sensors", _coreClockSensors.Count);
    }

    /// <summary>
    /// Starts frequency monitoring.
    /// </summary>
    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_isRunning)
        {
            _logger?.LogWarning("PpcMeasure already running");
            return;
        }

        if (_cpu == null)
        {
            throw new InvalidOperationException("Initialize() must be called before Start()");
        }

        _samples.Clear();
        _startTime = DateTime.UtcNow;
        _isRunning = true;

        _pollingTask = Task.Run(PollingLoopAsync);

        _logger?.LogDebug("PpcMeasure started with polling interval {Interval}ms", _pollingInterval.TotalMilliseconds);
    }

    /// <summary>
    /// Stops frequency monitoring and calculates the PPC result.
    /// </summary>
    /// <param name="benchmarkScore">The benchmark score to calculate PPC against.</param>
    /// <returns>The PPC measurement result.</returns>
    public async Task<PpcMeasureResult> StopAsync(double benchmarkScore)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (!_isRunning)
        {
            throw new InvalidOperationException("PpcMeasure is not running");
        }

        _isRunning = false;
        await _cts.CancelAsync();

        if (_pollingTask != null)
        {
            try
            {
                await _pollingTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        var duration = DateTime.UtcNow - _startTime;

        return CalculateResult(benchmarkScore, duration);
    }

    /// <summary>
    /// Stops frequency monitoring synchronously.
    /// </summary>
    public PpcMeasureResult Stop(double benchmarkScore)
    {
        return StopAsync(benchmarkScore).GetAwaiter().GetResult();
    }

    private async Task PollingLoopAsync()
    {
        while (_isRunning && !_cts.Token.IsCancellationRequested)
        {
            try
            {
                SampleFrequencies();
                await Task.Delay(_pollingInterval, _cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during frequency sampling");
            }
        }
    }

    private void SampleFrequencies()
    {
        if (_cpu == null) return;

        _cpu.Update();
        var timestamp = DateTime.UtcNow;

        foreach (var (coreIndex, sensor) in _coreClockSensors)
        {
            if (sensor.Value.HasValue)
            {
                _samples.Add(new FrequencySample(coreIndex, sensor.Value.Value, timestamp));
            }
        }
    }

    private PpcMeasureResult CalculateResult(double benchmarkScore, TimeSpan duration)
    {
        var samplesArray = _samples.ToArray();

        if (samplesArray.Length == 0)
        {
            _logger?.LogWarning("No frequency samples collected");
            return new PpcMeasureResult
            {
                BenchmarkScore = benchmarkScore,
                Duration = duration,
                AverageFrequencyMHz = 0,
                MinFrequencyMHz = 0,
                MaxFrequencyMHz = 0,
                PerformancePerClock = 0,
                SampleCount = 0
            };
        }

        // Calculate per-core averages
        var perCoreGroups = samplesArray.GroupBy(s => s.CoreIndex);
        var perCoreAverages = perCoreGroups.ToDictionary(
            g => g.Key,
            g => g.Average(s => s.FrequencyMHz)
        );

        // Calculate overall statistics
        var allFrequencies = samplesArray.Select(s => s.FrequencyMHz).ToArray();
        double avgFreq = allFrequencies.Average();
        double minFreq = allFrequencies.Min();
        double maxFreq = allFrequencies.Max();

        // Calculate PPC: Performance Per Clock
        // PPC = Score / (Average Frequency in GHz * Duration in seconds)
        // This gives a normalized score independent of clock speed
        double avgFreqGHz = avgFreq / 1000.0;
        double durationSeconds = duration.TotalSeconds;
        double ppc = durationSeconds > 0 && avgFreqGHz > 0
            ? benchmarkScore / (avgFreqGHz * durationSeconds)
            : 0;

        _logger?.LogInformation(
            "PPC Result: Score={Score:F2}, AvgFreq={Freq:F0}MHz, Duration={Duration:F2}s, PPC={Ppc:F4}",
            benchmarkScore, avgFreq, durationSeconds, ppc);

        return new PpcMeasureResult
        {
            BenchmarkScore = benchmarkScore,
            Duration = duration,
            AverageFrequencyMHz = avgFreq,
            MinFrequencyMHz = minFreq,
            MaxFrequencyMHz = maxFreq,
            PerformancePerClock = ppc,
            PerCoreAverageFrequency = perCoreAverages,
            SampleCount = samplesArray.Length
        };
    }

    public void Dispose()
    {
        if (_disposed) return;

        _isRunning = false;
        _cts.Cancel();
        _cts.Dispose();
        _pollingTask?.Wait(1000);
        _computer.Close();

        _disposed = true;
    }
}
