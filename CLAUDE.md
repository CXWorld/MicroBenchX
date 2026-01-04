# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MicroBenchX is a CPU/GPU micro-benchmarking suite for Windows. It measures IPC (Instructions Per Cycle), PPC (Performance Per Clock), and inter-core latencies.

## Build Commands

```bash
# Build entire solution (requires Visual Studio 2022 with v143 toolset)
msbuild MicroBenchX.sln /p:Configuration=Release /p:Platform=x64

# Build UI project only (.NET 9)
dotnet build source/MicroBenchX.UI/MicroBenchX.UI.csproj

# Build C++ projects
msbuild source\MicroBenchX.Ipc\MicroBenchX.Ipc.vcxproj /p:Configuration=Release /p:Platform=x64
msbuild source\MicroBenchX.Icl\CoherencyLatency.vcxproj /p:Configuration=Release /p:Platform=x64
```

## Prerequisites

NASM (Netwide Assembler) is required for building MicroBenchX.Ipc:
1. Install NASM from https://www.nasm.us/
2. Install VSNASM from https://github.com/ShiftMediaProject/VSNASM
3. Set environment variable: `NASMPATH=C:\Program Files\NASM`

## Architecture

### Projects

| Project | Type | Purpose |
|---------|------|---------|
| `MicroBenchX.UI` | .NET 9 WPF | Main UI application with MVVM |
| `MicroBenchX.Benchmark` | .NET 9 Class Library | Benchmark abstraction and base classes |
| `MicroBenchX.Ipc` | C++ Console + NASM | IPC benchmark tests with hand-written assembly |
| `CoherencyLatency` | C++ Console | Core-to-core latency measurement |

### MicroBenchX.Benchmark Architecture

Benchmark abstraction layer:

```
source/MicroBenchX.Benchmark/
├── IBenchmark.cs              # Benchmark interface (Start, Abort, Score, Runtime, etc.)
├── BenchmarkBase.cs           # Abstract base class with common implementation
├── ComputeBenchmarkBase.cs    # Base for multi-threaded compute benchmarks
├── CoreAffinity.cs            # Thread-to-core pinning configuration
├── BenchmarkType.cs           # Enum: Ppc, Ipc, CoreToCore, MemoryLatency
├── BenchmarkState.cs          # Enum: Idle, Running, Completed, Aborted, Failed
├── BenchmarkResult.cs         # Result container with score, runtime, additional data
├── BenchmarkProgressEventArgs.cs
└── Benchmarks/
    ├── Avx2ComputeBenchmark.cs      # AVX2 FMA instruction-heavy (GFLOPS)
    ├── CacheBenchmark.cs            # L2/L3 cache stress with pointer chasing
    └── MemoryBandwidthBenchmark.cs  # Main memory bandwidth (GB/s)
```

**Core Affinity Modes** (`CoreAffinityConfig`):
- `Auto()` - 1 thread, OS schedules
- `SingleCore(index)` - 1 thread pinned to specific core
- `AllCores()` - 1 thread per logical processor
- `SpecificCores(indices)` - threads pinned to given cores

### MicroBenchX.UI Architecture

The UI uses MVVM pattern with dependency injection:

```
source/MicroBenchX.UI/
├── App.xaml.cs              # DI container setup (Microsoft.Extensions.Hosting)
├── ViewModels/
│   ├── MainWindowViewModel  # Tab navigation, child ViewModels
│   ├── PpcViewModel         # Performance Per Clock tab
│   ├── IpcViewModel         # Instructions Per Cycle tab
│   ├── LatencyViewModel     # Core-to-core & memory latency tab
│   └── InfoViewModel        # Hardware info tab
├── Views/
│   ├── MainWindow.xaml      # TabControl container
│   └── *View.xaml           # Tab content views
├── Converters/              # Value converters
└── Resources/Styles.xaml    # Modern flat design styles
```

**Key packages:**
- `CommunityToolkit.Mvvm` - MVVM with source generators
- `Microsoft.Extensions.Hosting` - Generic host and DI
- `Microsoft.Extensions.Logging` - Logging infrastructure
- `LiveChartsCore.SkiaSharpView.WPF` - Charts and visualizations

### IPC Tests (`source/MicroBenchX.Ipc/IpcTests/`)

Assembly files for specific CPU instruction benchmarks:
- **Scalar**: `add_int64`, `mul_int64`, `div_int64`, `mov_int64`, etc.
- **SSE**: `add_sse128_float`, `mul_sse128_int`, etc.
- **AVX256**: `add_avx256_float`, `mul_avx256_int`, etc.
- **AVX512**: `add_avx512_float`, `mul_avx512_int`, etc.
- **AMD Zen-specific**: `zen_fpu_mix_21`, `zen_fpu_mix_22`

Key files:
- `source/MicroBenchX.Ipc/MicroBenchX.cpp` - Main entry point and test runner
- `source/MicroBenchX.Ipc/parameters.inc` - Shared assembly parameters
- `source/MicroBenchX.Ipc/InstructionSet.cpp` - CPU feature detection

### Core-to-Core Latency (`source/MicroBenchX.Icl/`)

Single file `CoherencyLatency.cpp` measures inter-core communication latency via cache coherency.

## Testing Notes

- IPC tests require fixed CPU clock speed (disable turbo, sleep states, AVX offsets)
- Set Windows power plan to "High Performance"
- For latency tests, set process priority to High and close other applications
- Latency heatmaps can be visualized at: https://www.capframex.com/assets/static/latency-heatmap.html
