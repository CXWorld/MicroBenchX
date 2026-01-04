using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor.Hardware.Cpu;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.UI.Services;

/// <summary>
/// Service for monitoring CPU hardware using LibreHardwareMonitor.
/// </summary>
public sealed class HardwareMonitorService : IDisposable
{
    private readonly ILogger<HardwareMonitorService> _logger;
    private readonly Computer _computer;
    private GenericCpu? _cpu;
    private bool _isInitialized;
    private bool _disposed;

    public HardwareMonitorService(ILogger<HardwareMonitorService> logger)
    {
        _logger = logger;
        _computer = new Computer
        {
            IsCpuEnabled = true
        };
    }

    /// <summary>
    /// Gets whether the service is initialized.
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Gets the CPU hardware instance.
    /// </summary>
    public GenericCpu? Cpu => _cpu;

    /// <summary>
    /// Gets the CPU name.
    /// </summary>
    public string CpuName => _cpu?.Name ?? "Unknown";

    /// <summary>
    /// Gets the number of physical cores.
    /// </summary>
    public int CoreCount => _cpu?.CoreCount ?? Environment.ProcessorCount;

    /// <summary>
    /// Gets the number of threads.
    /// </summary>
    public int ThreadCount => Environment.ProcessorCount;

    /// <summary>
    /// Gets whether the CPU has a hybrid architecture (P-cores and E-cores).
    /// </summary>
    public bool IsHybrid => _cpu?.IsHybrid ?? false;

    /// <summary>
    /// Initializes the hardware monitor.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized || _disposed)
            return;

        try
        {
            _computer.Open();

            // Find the CPU hardware
            foreach (var hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu && hardware is GenericCpu cpu)
                {
                    _cpu = cpu;
                    _logger.LogInformation("Found CPU: {Name}, Cores: {Cores}, Hybrid: {Hybrid}",
                        cpu.Name, cpu.CoreCount, cpu.IsHybrid);
                    break;
                }
            }

            if (_cpu == null)
            {
                _logger.LogWarning("No CPU hardware found");
            }

            _isInitialized = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize hardware monitor");
        }
    }

    /// <summary>
    /// Updates all sensor values.
    /// </summary>
    public void Update()
    {
        if (!_isInitialized || _cpu == null)
            return;

        try
        {
            _cpu.Update();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update CPU sensors");
        }
    }

    /// <summary>
    /// Gets the core type for the specified thread index.
    /// </summary>
    /// <param name="threadIndex">The thread index.</param>
    /// <returns>The core type.</returns>
    public CpuCoreType GetCoreType(int threadIndex)
    {
        if (_cpu == null || !_isInitialized)
            return CpuCoreType.Standard;

        // Map thread index to core index
        int coreIndex = GetPhysicalCoreIndex(threadIndex);
        return _cpu.GetCoreType(coreIndex);
    }

    /// <summary>
    /// Gets the physical core index for a thread.
    /// </summary>
    /// <param name="threadIndex">The thread index.</param>
    /// <returns>The physical core index.</returns>
    public int GetPhysicalCoreIndex(int threadIndex)
    {
        if (_cpu == null)
            return threadIndex;

        // Walk through CpuId to find which core this thread belongs to
        var cpuId = _cpu.CpuId;
        for (int coreIdx = 0; coreIdx < cpuId.Length; coreIdx++)
        {
            for (int threadIdx = 0; threadIdx < cpuId[coreIdx].Length; threadIdx++)
            {
                if (cpuId[coreIdx][threadIdx].Thread == threadIndex)
                {
                    return coreIdx;
                }
            }
        }

        return threadIndex; // Fallback
    }

    /// <summary>
    /// Gets the thread index within a physical core (0 or 1 for SMT).
    /// </summary>
    /// <param name="threadIndex">The thread index.</param>
    /// <returns>The thread index within the core.</returns>
    public int GetThreadIndexInCore(int threadIndex)
    {
        if (_cpu == null)
            return 0;

        var cpuId = _cpu.CpuId;
        for (int coreIdx = 0; coreIdx < cpuId.Length; coreIdx++)
        {
            for (int threadIdx = 0; threadIdx < cpuId[coreIdx].Length; threadIdx++)
            {
                if (cpuId[coreIdx][threadIdx].Thread == threadIndex)
                {
                    return threadIdx;
                }
            }
        }

        return 0;
    }

    /// <summary>
    /// Gets the current load for the specified thread.
    /// </summary>
    /// <param name="threadIndex">The thread index.</param>
    /// <returns>The load percentage (0-100), or null if not available.</returns>
    public float? GetThreadLoad(int threadIndex)
    {
        if (_cpu == null || !_isInitialized)
            return null;

        // Find the load sensor for this thread
        // Sensor names are like "Core #1" or "Core #1 Thread #1"
        foreach (var sensor in _cpu.Sensors)
        {
            if (sensor.SensorType == SensorType.Load && sensor.Index == threadIndex + 2)
            {
                return sensor.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all thread loads.
    /// </summary>
    /// <returns>Dictionary mapping thread index to load percentage.</returns>
    public Dictionary<int, float> GetAllThreadLoads()
    {
        var loads = new Dictionary<int, float>();

        if (_cpu == null || !_isInitialized)
            return loads;

        foreach (var sensor in _cpu.Sensors)
        {
            if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
            {
                // Thread load sensors have index starting at 2 (0=Total, 1=Max)
                if (sensor.Index >= 2)
                {
                    int threadIndex = sensor.Index - 2;
                    loads[threadIndex] = sensor.Value.Value;
                }
            }
        }

        return loads;
    }

    /// <summary>
    /// Gets the total CPU load.
    /// </summary>
    /// <returns>Total load percentage (0-100), or null if not available.</returns>
    public float? GetTotalLoad()
    {
        if (_cpu == null || !_isInitialized)
            return null;

        foreach (var sensor in _cpu.Sensors)
        {
            if (sensor.SensorType == SensorType.Load && sensor.Name == "CPU Total")
            {
                return sensor.Value;
            }
        }

        return null;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _computer.Close();
        _disposed = true;
    }
}
