using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.UI.ViewModels;

public partial class LatencyViewModel : ObservableObject
{
    private readonly ILogger<LatencyViewModel> _logger;

    [ObservableProperty]
    private string _header = "Latency";

    [ObservableProperty]
    private string _description = "Core-to-Core and Memory Subsystem Latency - Measures inter-core communication and memory access times.";

    [ObservableProperty]
    private bool _isRunning;

    public LatencyViewModel(ILogger<LatencyViewModel> logger)
    {
        _logger = logger;
        _logger.LogDebug("LatencyViewModel initialized");
    }

    [RelayCommand]
    private void StartBenchmark()
    {
        _logger.LogInformation("Latency benchmark started");
        IsRunning = true;
        // TODO: Implement benchmark logic using CoherencyLatency
    }

    [RelayCommand]
    private void StopBenchmark()
    {
        _logger.LogInformation("Latency benchmark stopped");
        IsRunning = false;
        // TODO: Implement stop logic
    }
}
