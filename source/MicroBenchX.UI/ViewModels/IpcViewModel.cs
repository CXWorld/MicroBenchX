using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.UI.ViewModels;

public partial class IpcViewModel : ObservableObject
{
    private readonly ILogger<IpcViewModel> _logger;

    [ObservableProperty]
    private string _header = "IPC";

    [ObservableProperty]
    private string _description = "Instructions Per Cycle - Measures CPU instruction throughput using assembly-level benchmarks.";

    [ObservableProperty]
    private bool _isRunning;

    public IpcViewModel(ILogger<IpcViewModel> logger)
    {
        _logger = logger;
        _logger.LogDebug("IpcViewModel initialized");
    }

    [RelayCommand]
    private void StartBenchmark()
    {
        _logger.LogInformation("IPC benchmark started");
        IsRunning = true;
        // TODO: Implement benchmark logic using MicroBenchX.Ipc
    }

    [RelayCommand]
    private void StopBenchmark()
    {
        _logger.LogInformation("IPC benchmark stopped");
        IsRunning = false;
        // TODO: Implement stop logic
    }
}
