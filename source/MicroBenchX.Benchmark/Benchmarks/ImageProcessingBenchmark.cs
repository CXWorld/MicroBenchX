using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark.Benchmarks;

/// <summary>
/// Image processing filter types.
/// </summary>
public enum ImageFilter
{
    /// <summary>
    /// 3x3 Gaussian blur - L2 cache friendly.
    /// </summary>
    GaussianBlur3x3,

    /// <summary>
    /// 5x5 Gaussian blur - more cache pressure.
    /// </summary>
    GaussianBlur5x5,

    /// <summary>
    /// Sobel edge detection.
    /// </summary>
    SobelEdgeDetection,

    /// <summary>
    /// Box blur with variable radius.
    /// </summary>
    BoxBlur,

    /// <summary>
    /// Sharpen filter.
    /// </summary>
    Sharpen
}

/// <summary>
/// Cache-heavy benchmark using real image processing algorithms.
/// Applies convolution filters to synthetic image data, which naturally
/// stresses the cache due to neighborhood pixel access patterns.
/// </summary>
public class ImageProcessingBenchmark : ComputeBenchmarkBase
{
    private const int DefaultImageWidth = 2048;
    private const int DefaultImageHeight = 2048;
    private const int DefaultIterations = 50; // 50 passes over the image

    private readonly int _imageWidth;
    private readonly int _imageHeight;
    private readonly int _iterations;
    private readonly ImageFilter _filter;

    public override string Id => $"image-{_filter.ToString().ToLowerInvariant()}";
    public override string Name => $"Image Processing ({_filter})";
    public override string Description => $"Cache-intensive image processing using {_filter} filter on {_imageWidth}x{_imageHeight} image.";
    public override BenchmarkType Type => BenchmarkType.MemoryLatency;

    public ImageProcessingBenchmark(
        ILogger<ImageProcessingBenchmark> logger,
        CoreAffinityConfig affinityConfig,
        ImageFilter filter = ImageFilter.GaussianBlur3x3,
        int imageWidth = DefaultImageWidth,
        int imageHeight = DefaultImageHeight,
        int iterations = DefaultIterations)
        : base(logger, affinityConfig)
    {
        _imageWidth = imageWidth;
        _imageHeight = imageHeight;
        _iterations = iterations;
        _filter = filter;
    }

    protected override double RunWorkload(int threadIndex, int coreIndex, CancellationToken cancellationToken)
    {
        // Each thread works on its own image buffer
        var source = new float[_imageWidth * _imageHeight];
        var destination = new float[_imageWidth * _imageHeight];

        // Initialize with pseudo-random gradient data
        InitializeImage(source, threadIndex);

        var sw = Stopwatch.StartNew();
        long totalPixelsProcessed = 0;

        for (int iter = 0; iter < _iterations; iter++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            // Apply the selected filter
            int pixelsProcessed = _filter switch
            {
                ImageFilter.GaussianBlur3x3 => ApplyGaussianBlur3x3(source, destination, _imageWidth, _imageHeight),
                ImageFilter.GaussianBlur5x5 => ApplyGaussianBlur5x5(source, destination, _imageWidth, _imageHeight),
                ImageFilter.SobelEdgeDetection => ApplySobelEdgeDetection(source, destination, _imageWidth, _imageHeight),
                ImageFilter.BoxBlur => ApplyBoxBlur(source, destination, _imageWidth, _imageHeight, 7),
                ImageFilter.Sharpen => ApplySharpen(source, destination, _imageWidth, _imageHeight),
                _ => ApplyGaussianBlur3x3(source, destination, _imageWidth, _imageHeight)
            };

            totalPixelsProcessed += pixelsProcessed;

            // Swap buffers for next iteration
            (source, destination) = (destination, source);
        }

        sw.Stop();

        // Prevent optimization
        if (source[0] == float.NaN) throw new InvalidOperationException();

        // Calculate megapixels per second
        var seconds = sw.Elapsed.TotalSeconds;
        var megapixelsPerSecond = totalPixelsProcessed / seconds / 1_000_000.0;

        return megapixelsPerSecond;
    }

    private static void InitializeImage(float[] image, int seed)
    {
        var rng = new Random(42 + seed);
        int width = (int)Math.Sqrt(image.Length);

        for (int y = 0; y < width; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Create gradient with noise for realistic image data
                float gradient = (float)(x + y) / (width * 2);
                float noise = (float)rng.NextDouble() * 0.1f;
                image[y * width + x] = gradient + noise;
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ApplyGaussianBlur3x3(float[] source, float[] dest, int width, int height)
    {
        // 3x3 Gaussian kernel (approximation)
        // 1 2 1
        // 2 4 2
        // 1 2 1  / 16
        const float k00 = 1f / 16f, k01 = 2f / 16f, k02 = 1f / 16f;
        const float k10 = 2f / 16f, k11 = 4f / 16f, k12 = 2f / 16f;
        const float k20 = 1f / 16f, k21 = 2f / 16f, k22 = 1f / 16f;

        int pixelsProcessed = 0;

        for (int y = 1; y < height - 1; y++)
        {
            int rowOffset = y * width;
            int prevRowOffset = (y - 1) * width;
            int nextRowOffset = (y + 1) * width;

            for (int x = 1; x < width - 1; x++)
            {
                float sum =
                    source[prevRowOffset + x - 1] * k00 +
                    source[prevRowOffset + x] * k01 +
                    source[prevRowOffset + x + 1] * k02 +
                    source[rowOffset + x - 1] * k10 +
                    source[rowOffset + x] * k11 +
                    source[rowOffset + x + 1] * k12 +
                    source[nextRowOffset + x - 1] * k20 +
                    source[nextRowOffset + x] * k21 +
                    source[nextRowOffset + x + 1] * k22;

                dest[rowOffset + x] = sum;
                pixelsProcessed++;
            }
        }

        return pixelsProcessed;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ApplyGaussianBlur5x5(float[] source, float[] dest, int width, int height)
    {
        // 5x5 Gaussian kernel (approximation)
        float[] kernel = {
            1f/256f,  4f/256f,  6f/256f,  4f/256f, 1f/256f,
            4f/256f, 16f/256f, 24f/256f, 16f/256f, 4f/256f,
            6f/256f, 24f/256f, 36f/256f, 24f/256f, 6f/256f,
            4f/256f, 16f/256f, 24f/256f, 16f/256f, 4f/256f,
            1f/256f,  4f/256f,  6f/256f,  4f/256f, 1f/256f
        };

        int pixelsProcessed = 0;

        for (int y = 2; y < height - 2; y++)
        {
            for (int x = 2; x < width - 2; x++)
            {
                float sum = 0;
                int ki = 0;

                for (int ky = -2; ky <= 2; ky++)
                {
                    int rowOffset = (y + ky) * width;
                    for (int kx = -2; kx <= 2; kx++)
                    {
                        sum += source[rowOffset + x + kx] * kernel[ki++];
                    }
                }

                dest[y * width + x] = sum;
                pixelsProcessed++;
            }
        }

        return pixelsProcessed;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ApplySobelEdgeDetection(float[] source, float[] dest, int width, int height)
    {
        // Sobel kernels for edge detection
        // Gx:          Gy:
        // -1  0  1     -1 -2 -1
        // -2  0  2      0  0  0
        // -1  0  1      1  2  1

        int pixelsProcessed = 0;

        for (int y = 1; y < height - 1; y++)
        {
            int rowOffset = y * width;
            int prevRowOffset = (y - 1) * width;
            int nextRowOffset = (y + 1) * width;

            for (int x = 1; x < width - 1; x++)
            {
                // Compute Gx
                float gx =
                    -source[prevRowOffset + x - 1] +
                    source[prevRowOffset + x + 1] +
                    -2f * source[rowOffset + x - 1] +
                    2f * source[rowOffset + x + 1] +
                    -source[nextRowOffset + x - 1] +
                    source[nextRowOffset + x + 1];

                // Compute Gy
                float gy =
                    -source[prevRowOffset + x - 1] +
                    -2f * source[prevRowOffset + x] +
                    -source[prevRowOffset + x + 1] +
                    source[nextRowOffset + x - 1] +
                    2f * source[nextRowOffset + x] +
                    source[nextRowOffset + x + 1];

                // Magnitude (approximation using |Gx| + |Gy| instead of sqrt)
                dest[rowOffset + x] = MathF.Abs(gx) + MathF.Abs(gy);
                pixelsProcessed++;
            }
        }

        return pixelsProcessed;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ApplyBoxBlur(float[] source, float[] dest, int width, int height, int radius)
    {
        int kernelSize = radius * 2 + 1;
        float divisor = kernelSize * kernelSize;
        int pixelsProcessed = 0;

        for (int y = radius; y < height - radius; y++)
        {
            for (int x = radius; x < width - radius; x++)
            {
                float sum = 0;

                for (int ky = -radius; ky <= radius; ky++)
                {
                    int rowOffset = (y + ky) * width;
                    for (int kx = -radius; kx <= radius; kx++)
                    {
                        sum += source[rowOffset + x + kx];
                    }
                }

                dest[y * width + x] = sum / divisor;
                pixelsProcessed++;
            }
        }

        return pixelsProcessed;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ApplySharpen(float[] source, float[] dest, int width, int height)
    {
        // Sharpen kernel:
        //  0 -1  0
        // -1  5 -1
        //  0 -1  0

        int pixelsProcessed = 0;

        for (int y = 1; y < height - 1; y++)
        {
            int rowOffset = y * width;
            int prevRowOffset = (y - 1) * width;
            int nextRowOffset = (y + 1) * width;

            for (int x = 1; x < width - 1; x++)
            {
                float sum =
                    -source[prevRowOffset + x] +
                    -source[rowOffset + x - 1] +
                    5f * source[rowOffset + x] +
                    -source[rowOffset + x + 1] +
                    -source[nextRowOffset + x];

                // Clamp to valid range
                dest[rowOffset + x] = Math.Clamp(sum, 0f, 1f);
                pixelsProcessed++;
            }
        }

        return pixelsProcessed;
    }

    protected override string GetScoreUnit() => "MP/s";

    protected override double AggregateResults(double[] threadScores)
    {
        // Sum megapixels/second across all threads
        return threadScores.Sum();
    }
}
