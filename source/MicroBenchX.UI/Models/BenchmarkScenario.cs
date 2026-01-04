namespace MicroBenchX.UI.Models;

/// <summary>
/// Represents a benchmark scenario that can be selected in the UI.
/// </summary>
public class BenchmarkScenario
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Category { get; init; }
    public required string ScoreUnit { get; init; }

    /// <summary>
    /// Gets the predefined benchmark scenarios.
    /// </summary>
    public static IReadOnlyList<BenchmarkScenario> All { get; } =
    [
        // Compute benchmarks
        new BenchmarkScenario
        {
            Id = "avx2-compute",
            Name = "AVX2 Compute",
            Description = "Instruction-heavy benchmark using AVX2 FMA operations",
            Category = "Compute",
            ScoreUnit = "GFLOPS"
        },

        // Natural Cache benchmarks (Image Processing)
        new BenchmarkScenario
        {
            Id = "image-gaussianblur3x3",
            Name = "Image Blur (3x3)",
            Description = "Gaussian blur filter on 2048x2048 image - L2 cache friendly",
            Category = "Cache (Image)",
            ScoreUnit = "MP/s"
        },
        new BenchmarkScenario
        {
            Id = "image-gaussianblur5x5",
            Name = "Image Blur (5x5)",
            Description = "Gaussian blur filter with larger kernel - more cache pressure",
            Category = "Cache (Image)",
            ScoreUnit = "MP/s"
        },
        new BenchmarkScenario
        {
            Id = "image-sobeledgedetection",
            Name = "Edge Detection",
            Description = "Sobel edge detection algorithm on 2048x2048 image",
            Category = "Cache (Image)",
            ScoreUnit = "MP/s"
        },
        new BenchmarkScenario
        {
            Id = "image-boxblur",
            Name = "Box Blur (7x7)",
            Description = "Box blur with 7-pixel radius - heavy cache usage",
            Category = "Cache (Image)",
            ScoreUnit = "MP/s"
        },
        new BenchmarkScenario
        {
            Id = "image-sharpen",
            Name = "Image Sharpen",
            Description = "Sharpening filter on 2048x2048 image",
            Category = "Cache (Image)",
            ScoreUnit = "MP/s"
        },

        // Natural Memory benchmarks (Compression)
        new BenchmarkScenario
        {
            Id = "compress-lz77",
            Name = "LZ77 Compression",
            Description = "LZ77-style sliding window compression on 64MB data",
            Category = "Memory (Compression)",
            ScoreUnit = "MB/s"
        },
        new BenchmarkScenario
        {
            Id = "compress-rle",
            Name = "RLE Compression",
            Description = "Run-length encoding compression on 64MB data",
            Category = "Memory (Compression)",
            ScoreUnit = "MB/s"
        },
        new BenchmarkScenario
        {
            Id = "compress-dictionary",
            Name = "Dictionary Compression",
            Description = "Dictionary-based (LZW-style) compression on 64MB data",
            Category = "Memory (Compression)",
            ScoreUnit = "MB/s"
        },

        // Synthetic Cache benchmarks (kept for technical analysis)
        new BenchmarkScenario
        {
            Id = "cache-l2",
            Name = "L2 Cache Stress (Synthetic)",
            Description = "Pointer chasing in L2-sized buffer",
            Category = "Cache (Synthetic)",
            ScoreUnit = "GB/s"
        },
        new BenchmarkScenario
        {
            Id = "cache-l3",
            Name = "L3 Cache Stress (Synthetic)",
            Description = "Pointer chasing in L3-sized buffer",
            Category = "Cache (Synthetic)",
            ScoreUnit = "GB/s"
        },

        // Synthetic Memory benchmarks (kept for technical analysis)
        new BenchmarkScenario
        {
            Id = "memory-sequentialread",
            Name = "Memory Read (Synthetic)",
            Description = "Sequential read with 512MB buffer",
            Category = "Memory (Synthetic)",
            ScoreUnit = "GB/s"
        },
        new BenchmarkScenario
        {
            Id = "memory-sequentialwrite",
            Name = "Memory Write (Synthetic)",
            Description = "Sequential write with 512MB buffer",
            Category = "Memory (Synthetic)",
            ScoreUnit = "GB/s"
        },
        new BenchmarkScenario
        {
            Id = "memory-sequentialreadwrite",
            Name = "Memory Read-Write (Synthetic)",
            Description = "Read-modify-write with 512MB buffer",
            Category = "Memory (Synthetic)",
            ScoreUnit = "GB/s"
        }
    ];
}
