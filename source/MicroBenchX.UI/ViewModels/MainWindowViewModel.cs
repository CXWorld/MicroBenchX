using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private string _title = "MicroBenchX";

    [ObservableProperty]
    private int _selectedTabIndex;

    public PpcViewModel PpcViewModel { get; }
    public IpcViewModel IpcViewModel { get; }
    public LatencyViewModel LatencyViewModel { get; }
    public InfoViewModel InfoViewModel { get; }

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        PpcViewModel ppcViewModel,
        IpcViewModel ipcViewModel,
        LatencyViewModel latencyViewModel,
        InfoViewModel infoViewModel)
    {
        _logger = logger;
        PpcViewModel = ppcViewModel;
        IpcViewModel = ipcViewModel;
        LatencyViewModel = latencyViewModel;
        InfoViewModel = infoViewModel;

        _logger.LogInformation("MainWindowViewModel initialized");
    }
}
