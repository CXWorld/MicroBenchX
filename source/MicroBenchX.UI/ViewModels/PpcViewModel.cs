using System.Collections.ObjectModel;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibreHardwareMonitor.Hardware.Cpu;
using Microsoft.Extensions.Logging;
using MicroBenchX.Benchmark;
using MicroBenchX.Benchmark.Benchmarks;
using MicroBenchX.UI.Models;
using MicroBenchX.UI.Services;

namespace MicroBenchX.UI.ViewModels;

public partial class PpcViewModel : ObservableObject, IDisposable
{
    private readonly ILogger<PpcViewModel> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly HardwareMonitorService _hardwareMonitor;
    private readonly DispatcherTimer _loadUpdateTimer;
    private CancellationTokenSource? _benchmarkCts;
    private PpcMeasure? _ppcMeasure;
    private bool _disposed;

    [ObservableProperty]
    private string _header = "PPC";

    [ObservableProperty]
    private string _description = "Performance Per Clock - Measures how much work the CPU accomplishes per clock cycle.";

    // Hardware Info
    [ObservableProperty]
    private string _cpuName = "Detecting...";

    [ObservableProperty]
    private int _threadCount;

    [ObservableProperty]
    private int _coreCount;

    [ObservableProperty]
    private string _cpuArchitecture = "";

    [ObservableProperty]
    private bool _isHybridCpu;

    // Core Grid
    public ObservableCollection<CoreInfo> Cores { get; } = [];

    [ObservableProperty]
    private int _selectedCoreCount;

    // Benchmark Scenarios
    public IReadOnlyList<BenchmarkScenario> Scenarios { get; } = BenchmarkScenario.All;

    [ObservableProperty]
    private BenchmarkScenario? _selectedScenario;

    // Benchmark State
    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    [ObservableProperty]
    private double _progress;

    // Results
    [ObservableProperty]
    private bool _hasResults;

    [ObservableProperty]
    private double _benchmarkScore;

    [ObservableProperty]
    private string _scoreUnit = "";

    [ObservableProperty]
    private double _averageFrequencyMHz;

    [ObservableProperty]
    private double _minFrequencyMHz;

    [ObservableProperty]
    private double _maxFrequencyMHz;

    [ObservableProperty]
    private double _performancePerClock;

    [ObservableProperty]
    private TimeSpan _benchmarkDuration;

    [ObservableProperty]
    private int _sampleCount;

    public PpcViewModel(
        ILogger<PpcViewModel> logger,
        ILoggerFactory loggerFactory,
        HardwareMonitorService hardwareMonitor)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _hardwareMonitor = hardwareMonitor;

        // Initialize hardware monitor
        _hardwareMonitor.Initialize();

        InitializeHardwareInfo();
        InitializeCoreGrid();

        SelectedScenario = Scenarios.FirstOrDefault();

        // Setup load update timer (500ms interval)
        _loadUpdateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };
        _loadUpdateTimer.Tick += LoadUpdateTimer_Tick;
        _loadUpdateTimer.Start();

        _logger.LogDebug("PpcViewModel initialized");
    }

    private void LoadUpdateTimer_Tick(object? sender, EventArgs e)
    {
        UpdateCoreLoads();
    }

    private void UpdateCoreLoads()
    {
        if (_disposed)
            return;

        try
        {
            _hardwareMonitor.Update();
            var loads = _hardwareMonitor.GetAllThreadLoads();

            foreach (var core in Cores)
            {
                if (loads.TryGetValue(core.Index, out var load))
                {
                    core.Load = load;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update core loads");
        }
    }

    private void InitializeHardwareInfo()
    {
        ThreadCount = _hardwareMonitor.ThreadCount;
        CoreCount = _hardwareMonitor.CoreCount;
        CpuName = _hardwareMonitor.CpuName;
        IsHybridCpu = _hardwareMonitor.IsHybrid;
        CpuArchitecture = Environment.Is64BitProcess ? "x64" : "x86";

        _logger.LogInformation("CPU: {Name}, Cores: {Cores}, Threads: {Threads}, Hybrid: {Hybrid}",
            CpuName, CoreCount, ThreadCount, IsHybridCpu);
    }

    private void InitializeCoreGrid()
    {
        Cores.Clear();
        for (int i = 0; i < ThreadCount; i++)
        {
            var core = new CoreInfo(i)
            {
                CoreType = _hardwareMonitor.GetCoreType(i),
                PhysicalCoreIndex = _hardwareMonitor.GetPhysicalCoreIndex(i),
                ThreadIndexInCore = _hardwareMonitor.GetThreadIndexInCore(i)
            };

            core.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CoreInfo.IsSelected))
                {
                    UpdateSelectedCoreCount();
                }
            };

            Cores.Add(core);
        }

        UpdateSelectedCoreCount();

        _logger.LogDebug("Initialized {Count} cores in grid", Cores.Count);
    }

    private void UpdateSelectedCoreCount()
    {
        SelectedCoreCount = Cores.Count(c => c.IsSelected);
        OnPropertyChanged(nameof(CanStart));
    }

    public bool CanStart => !IsRunning && SelectedCoreCount > 0 && SelectedScenario != null;

    partial void OnIsRunningChanged(bool value)
    {
        OnPropertyChanged(nameof(CanStart));
    }

    partial void OnSelectedScenarioChanged(BenchmarkScenario? value)
    {
        OnPropertyChanged(nameof(CanStart));
        if (value != null)
        {
            ScoreUnit = value.ScoreUnit;
        }
    }

    [RelayCommand]
    private void SelectAllCores()
    {
        foreach (var core in Cores)
        {
            core.IsSelected = true;
        }
    }

    [RelayCommand]
    private void SelectNoCores()
    {
        foreach (var core in Cores)
        {
            core.IsSelected = false;
        }
    }

    [RelayCommand]
    private void SelectPCores()
    {
        foreach (var core in Cores)
        {
            // Select P-cores or Standard cores (non-hybrid)
            core.IsSelected = core.CoreType == CpuCoreType.PerformanceCore ||
                              core.CoreType == CpuCoreType.Standard;
        }
    }

    [RelayCommand]
    private void SelectECores()
    {
        foreach (var core in Cores)
        {
            // Select E-cores
            core.IsSelected = core.CoreType == CpuCoreType.EfficiencyCore ||
                              core.CoreType == CpuCoreType.LowPowerEfficiencyCore;
        }
    }

    [RelayCommand]
    private void SelectPhysicalCores()
    {
        // Select first thread of each physical core
        var selectedPhysicalCores = new HashSet<int>();
        foreach (var core in Cores)
        {
            if (!selectedPhysicalCores.Contains(core.PhysicalCoreIndex))
            {
                core.IsSelected = true;
                selectedPhysicalCores.Add(core.PhysicalCoreIndex);
            }
            else
            {
                core.IsSelected = false;
            }
        }
    }

    [RelayCommand]
    private void ToggleCore(CoreInfo? core)
    {
        if (core != null && !IsRunning)
        {
            core.IsSelected = !core.IsSelected;
        }
    }

    [RelayCommand]
    private async Task StartBenchmarkAsync()
    {
        if (!CanStart || SelectedScenario == null)
            return;

        _logger.LogInformation("Starting PPC benchmark with scenario: {Scenario}", SelectedScenario.Name);

        IsRunning = true;
        HasResults = false;
        StatusMessage = "Initializing...";
        Progress = 0;

        _benchmarkCts = new CancellationTokenSource();

        try
        {
            var selectedCoreIndices = Cores
                .Where(c => c.IsSelected)
                .Select(c => c.Index)
                .ToArray();

            // Mark selected cores as active
            foreach (var core in Cores.Where(c => c.IsSelected))
            {
                core.IsActive = true;
            }

            var affinityConfig = CoreAffinityConfig.SpecificCores(selectedCoreIndices);

            // Create PPC measure with 100ms polling
            _ppcMeasure = PpcMeasure.FromAffinityConfig(
                affinityConfig,
                TimeSpan.FromMilliseconds(100),
                _loggerFactory.CreateLogger<PpcMeasure>());

            StatusMessage = "Initializing hardware monitor...";
            _ppcMeasure.Initialize();

            StatusMessage = "Running benchmark...";
            Progress = 10;

            // Create the appropriate benchmark
            IBenchmark benchmark = CreateBenchmark(SelectedScenario, affinityConfig);

            // Start frequency monitoring
            _ppcMeasure.Start();

            // Run benchmark
            var result = await benchmark.StartAsync(_benchmarkCts.Token);

            Progress = 80;
            StatusMessage = "Calculating PPC...";

            // Stop monitoring and get results
            var ppcResult = await _ppcMeasure.StopAsync(result.Score);

            // Update UI with results
            BenchmarkScore = result.Score;
            ScoreUnit = SelectedScenario.ScoreUnit;
            AverageFrequencyMHz = ppcResult.AverageFrequencyMHz;
            MinFrequencyMHz = ppcResult.MinFrequencyMHz;
            MaxFrequencyMHz = ppcResult.MaxFrequencyMHz;
            PerformancePerClock = ppcResult.PerformancePerClock;
            BenchmarkDuration = ppcResult.Duration;
            SampleCount = ppcResult.SampleCount;
            HasResults = true;

            Progress = 100;
            StatusMessage = $"Completed - Score: {result.Score:F2} {ScoreUnit}";

            _logger.LogInformation(
                "Benchmark completed. Score: {Score} {Unit}, PPC: {PPC:F4}",
                result.Score, ScoreUnit, ppcResult.PerformancePerClock);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Benchmark cancelled";
            _logger.LogInformation("Benchmark cancelled by user");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            _logger.LogError(ex, "Benchmark failed");
        }
        finally
        {
            // Clear active state
            foreach (var core in Cores)
            {
                core.IsActive = false;
                core.HasFrequencyData = false;
            }

            _ppcMeasure?.Dispose();
            _ppcMeasure = null;
            _benchmarkCts?.Dispose();
            _benchmarkCts = null;
            IsRunning = false;
        }
    }

    private IBenchmark CreateBenchmark(BenchmarkScenario scenario, CoreAffinityConfig affinityConfig)
    {
        return scenario.Id switch
        {
            // Compute
            "avx2-compute" => new Avx2ComputeBenchmark(
                _loggerFactory.CreateLogger<Avx2ComputeBenchmark>(),
                affinityConfig),

            // Image Processing (Cache)
            "image-gaussianblur3x3" => new ImageProcessingBenchmark(
                _loggerFactory.CreateLogger<ImageProcessingBenchmark>(),
                affinityConfig,
                ImageFilter.GaussianBlur3x3),

            "image-gaussianblur5x5" => new ImageProcessingBenchmark(
                _loggerFactory.CreateLogger<ImageProcessingBenchmark>(),
                affinityConfig,
                ImageFilter.GaussianBlur5x5),

            "image-sobeledgedetection" => new ImageProcessingBenchmark(
                _loggerFactory.CreateLogger<ImageProcessingBenchmark>(),
                affinityConfig,
                ImageFilter.SobelEdgeDetection),

            "image-boxblur" => new ImageProcessingBenchmark(
                _loggerFactory.CreateLogger<ImageProcessingBenchmark>(),
                affinityConfig,
                ImageFilter.BoxBlur),

            "image-sharpen" => new ImageProcessingBenchmark(
                _loggerFactory.CreateLogger<ImageProcessingBenchmark>(),
                affinityConfig,
                ImageFilter.Sharpen),

            // Compression (Memory)
            "compress-lz77" => new CompressionBenchmark(
                _loggerFactory.CreateLogger<CompressionBenchmark>(),
                affinityConfig,
                CompressionAlgorithm.LZ77),

            "compress-rle" => new CompressionBenchmark(
                _loggerFactory.CreateLogger<CompressionBenchmark>(),
                affinityConfig,
                CompressionAlgorithm.RLE),

            "compress-dictionary" => new CompressionBenchmark(
                _loggerFactory.CreateLogger<CompressionBenchmark>(),
                affinityConfig,
                CompressionAlgorithm.Dictionary),

            // Synthetic Cache
            "cache-l2" => new CacheBenchmark(
                _loggerFactory.CreateLogger<CacheBenchmark>(),
                affinityConfig,
                CacheLevel.L2),

            "cache-l3" => new CacheBenchmark(
                _loggerFactory.CreateLogger<CacheBenchmark>(),
                affinityConfig,
                CacheLevel.L3),

            // Synthetic Memory
            "memory-sequentialread" => new MemoryBandwidthBenchmark(
                _loggerFactory.CreateLogger<MemoryBandwidthBenchmark>(),
                affinityConfig,
                MemoryAccessPattern.SequentialRead),

            "memory-sequentialwrite" => new MemoryBandwidthBenchmark(
                _loggerFactory.CreateLogger<MemoryBandwidthBenchmark>(),
                affinityConfig,
                MemoryAccessPattern.SequentialWrite),

            "memory-sequentialreadwrite" => new MemoryBandwidthBenchmark(
                _loggerFactory.CreateLogger<MemoryBandwidthBenchmark>(),
                affinityConfig,
                MemoryAccessPattern.SequentialReadWrite),

            _ => throw new NotSupportedException($"Unknown scenario: {scenario.Id}")
        };
    }

    [RelayCommand]
    private void StopBenchmark()
    {
        _logger.LogInformation("Stopping PPC benchmark");
        _benchmarkCts?.Cancel();
        StatusMessage = "Stopping...";
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _loadUpdateTimer.Stop();
        _disposed = true;
    }
}
