using LibreHardwareMonitor.Hardware;

namespace MicroBenchX.Benchmark;

/// <summary>
/// Defines how threads should be assigned to CPU cores.
/// </summary>
public enum CoreAffinityMode
{
    /// <summary>
    /// Let the OS scheduler decide (no affinity set).
    /// </summary>
    Auto,

    /// <summary>
    /// Run on a single specific core.
    /// </summary>
    SingleCore,

    /// <summary>
    /// Run on all available cores.
    /// </summary>
    AllCores,

    /// <summary>
    /// Run on a specific set of cores.
    /// </summary>
    SpecificCores
}

/// <summary>
/// Configuration for core affinity and thread assignment.
/// </summary>
public class CoreAffinityConfig
{
    /// <summary>
    /// Gets the affinity mode.
    /// </summary>
    public CoreAffinityMode Mode { get; }

    /// <summary>
    /// Gets the core indices to use (for SingleCore and SpecificCores modes).
    /// </summary>
    public int[] CoreIndices { get; }

    /// <summary>
    /// Gets the number of threads that will be used.
    /// </summary>
    public int ThreadCount => Mode switch
    {
        CoreAffinityMode.Auto => 1,
        CoreAffinityMode.SingleCore => 1,
        CoreAffinityMode.AllCores => Environment.ProcessorCount,
        CoreAffinityMode.SpecificCores => CoreIndices.Length,
        _ => 1
    };

    private CoreAffinityConfig(CoreAffinityMode mode, int[] coreIndices)
    {
        Mode = mode;
        CoreIndices = coreIndices;
    }

    /// <summary>
    /// Creates a configuration that lets the OS schedule the single thread.
    /// </summary>
    public static CoreAffinityConfig Auto() => new(CoreAffinityMode.Auto, []);

    /// <summary>
    /// Creates a configuration that runs on a single specific core.
    /// </summary>
    /// <param name="coreIndex">The zero-based core index.</param>
    public static CoreAffinityConfig SingleCore(int coreIndex)
    {
        ValidateCoreIndex(coreIndex);
        return new(CoreAffinityMode.SingleCore, [coreIndex]);
    }

    /// <summary>
    /// Creates a configuration that runs on all available cores.
    /// </summary>
    public static CoreAffinityConfig AllCores()
    {
        var indices = Enumerable.Range(0, Environment.ProcessorCount).ToArray();
        return new(CoreAffinityMode.AllCores, indices);
    }

    /// <summary>
    /// Creates a configuration that runs on specific cores.
    /// </summary>
    /// <param name="coreIndices">The zero-based core indices.</param>
    public static CoreAffinityConfig SpecificCores(params int[] coreIndices)
    {
        if (coreIndices.Length == 0)
            throw new ArgumentException("At least one core index must be specified.", nameof(coreIndices));

        foreach (var index in coreIndices)
            ValidateCoreIndex(index);

        return new(CoreAffinityMode.SpecificCores, coreIndices.Distinct().ToArray());
    }

    private static void ValidateCoreIndex(int coreIndex)
    {
        if (coreIndex < 0 || coreIndex >= Environment.ProcessorCount)
        {
            throw new ArgumentOutOfRangeException(nameof(coreIndex),
                $"Core index must be between 0 and {Environment.ProcessorCount - 1}.");
        }
    }

    public override string ToString() => Mode switch
    {
        CoreAffinityMode.Auto => "Auto (1 thread)",
        CoreAffinityMode.SingleCore => $"Core {CoreIndices[0]}",
        CoreAffinityMode.AllCores => $"All cores ({Environment.ProcessorCount} threads)",
        CoreAffinityMode.SpecificCores => $"Cores [{string.Join(", ", CoreIndices)}]",
        _ => "Unknown"
    };
}

/// <summary>
/// Provides methods for setting thread affinity using LibreHardwareMonitor's ThreadAffinity.
/// </summary>
public static class ThreadAffinityHelper
{
    /// <summary>
    /// Sets the current thread's affinity to the specified core.
    /// </summary>
    /// <param name="coreIndex">The zero-based core index.</param>
    /// <returns>The previous affinity that can be used to restore it later.</returns>
    public static GroupAffinity SetCurrentThreadAffinity(int coreIndex)
    {
        // Use group 0 for cores 0-63, group 1 for 64-127, etc.
        ushort group = (ushort)(coreIndex / 64);
        int indexInGroup = coreIndex % 64;

        var affinity = GroupAffinity.Single(group, indexInGroup);
        return ThreadAffinity.Set(affinity);
    }

    /// <summary>
    /// Sets the current thread's affinity to the specified GroupAffinity.
    /// </summary>
    /// <param name="affinity">The affinity to set.</param>
    /// <returns>The previous affinity.</returns>
    public static GroupAffinity SetCurrentThreadAffinity(GroupAffinity affinity)
    {
        return ThreadAffinity.Set(affinity);
    }

    /// <summary>
    /// Restores the thread affinity to a previously saved value.
    /// </summary>
    /// <param name="previousAffinity">The previous affinity to restore.</param>
    public static void RestoreThreadAffinity(GroupAffinity previousAffinity)
    {
        if (previousAffinity != GroupAffinity.Undefined)
        {
            ThreadAffinity.Set(previousAffinity);
        }
    }

    /// <summary>
    /// Clears the current thread's affinity (allows all cores in group 0).
    /// </summary>
    public static void ClearCurrentThreadAffinity()
    {
        // Set affinity to all cores in group 0
        int coresInGroup0 = Math.Min(Environment.ProcessorCount, 64);
        ulong allCoresMask = (1UL << coresInGroup0) - 1;
        var affinity = new GroupAffinity(0, allCoresMask);
        ThreadAffinity.Set(affinity);
    }
}
