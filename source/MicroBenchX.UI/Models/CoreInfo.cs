using CommunityToolkit.Mvvm.ComponentModel;
using LibreHardwareMonitor.Hardware.Cpu;

namespace MicroBenchX.UI.Models;

/// <summary>
/// Represents a CPU core for visualization in the core grid.
/// </summary>
public partial class CoreInfo : ObservableObject
{
    public CoreInfo(int index)
    {
        Index = index;
        DisplayName = $"{index}";
    }

    /// <summary>
    /// Zero-based core/thread index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Display name for the core.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Whether this core is selected for benchmarking.
    /// </summary>
    [ObservableProperty]
    private bool _isSelected;

    /// <summary>
    /// Whether this core is currently active (running benchmark).
    /// </summary>
    [ObservableProperty]
    private bool _isActive;

    /// <summary>
    /// Current CPU load percentage (0-100).
    /// </summary>
    [ObservableProperty]
    private double _load;

    /// <summary>
    /// Core type (P-core, E-core, Standard, etc.).
    /// </summary>
    [ObservableProperty]
    private CpuCoreType _coreType = CpuCoreType.Standard;

    /// <summary>
    /// Gets a short label for the core type.
    /// </summary>
    public string CoreTypeLabel => CoreType switch
    {
        CpuCoreType.PerformanceCore => "P",
        CpuCoreType.EfficiencyCore => "E",
        CpuCoreType.LowPowerEfficiencyCore => "LPE",
        CpuCoreType.DenseCore => "D",
        _ => ""
    };

    /// <summary>
    /// Current frequency in MHz (if available during benchmark).
    /// </summary>
    [ObservableProperty]
    private double _currentFrequencyMHz;

    /// <summary>
    /// Whether frequency data is available.
    /// </summary>
    [ObservableProperty]
    private bool _hasFrequencyData;

    /// <summary>
    /// Physical core index (for SMT systems, multiple threads map to same core).
    /// </summary>
    [ObservableProperty]
    private int _physicalCoreIndex;

    /// <summary>
    /// Thread index within the physical core (0 or 1 for SMT).
    /// </summary>
    [ObservableProperty]
    private int _threadIndexInCore;
}
