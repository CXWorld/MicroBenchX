using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.UI.ViewModels;

public partial class InfoViewModel : ObservableObject
{
    private readonly ILogger<InfoViewModel> _logger;

    [ObservableProperty]
    private string _header = "Info";

    [ObservableProperty]
    private string _description = "Hardware Information - Displays CPU, memory, and system specifications.";

    [ObservableProperty]
    private string _cpuName = string.Empty;

    [ObservableProperty]
    private int _coreCount;

    [ObservableProperty]
    private int _threadCount;

    public InfoViewModel(ILogger<InfoViewModel> logger)
    {
        _logger = logger;
        _logger.LogDebug("InfoViewModel initialized");
    }

    [RelayCommand]
    private void RefreshInfo()
    {
        _logger.LogInformation("Refreshing hardware info");
        // TODO: Implement hardware info retrieval
    }
}
