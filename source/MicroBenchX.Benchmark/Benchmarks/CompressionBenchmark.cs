using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace MicroBenchX.Benchmark.Benchmarks;

/// <summary>
/// Compression algorithm types.
/// </summary>
public enum CompressionAlgorithm
{
    /// <summary>
    /// LZ77-style sliding window compression.
    /// </summary>
    LZ77,

    /// <summary>
    /// Run-length encoding.
    /// </summary>
    RLE,

    /// <summary>
    /// Dictionary-based compression (simplified LZW-style).
    /// </summary>
    Dictionary
}

/// <summary>
/// Memory-heavy benchmark using compression algorithms.
/// Compression naturally stresses memory bandwidth due to:
/// - Large sliding window searches (LZ77)
/// - Sequential reading of input data
/// - Writing compressed output
/// </summary>
public class CompressionBenchmark : ComputeBenchmarkBase
{
    private const int DefaultBufferSizeMB = 64;
    private const int DefaultIterations = 5;
    private const int DefaultWindowSize = 32768; // 32KB sliding window

    private readonly int _bufferSizeMB;
    private readonly int _iterations;
    private readonly int _windowSize;
    private readonly CompressionAlgorithm _algorithm;

    public override string Id => $"compress-{_algorithm.ToString().ToLowerInvariant()}";
    public override string Name => $"Compression ({_algorithm})";
    public override string Description => $"Memory-intensive {_algorithm} compression on {_bufferSizeMB}MB data.";
    public override BenchmarkType Type => BenchmarkType.MemoryLatency;

    public CompressionBenchmark(
        ILogger<CompressionBenchmark> logger,
        CoreAffinityConfig affinityConfig,
        CompressionAlgorithm algorithm = CompressionAlgorithm.LZ77,
        int bufferSizeMB = DefaultBufferSizeMB,
        int iterations = DefaultIterations,
        int windowSize = DefaultWindowSize)
        : base(logger, affinityConfig)
    {
        _bufferSizeMB = bufferSizeMB;
        _iterations = iterations;
        _windowSize = windowSize;
        _algorithm = algorithm;
    }

    protected override double RunWorkload(int threadIndex, int coreIndex, CancellationToken cancellationToken)
    {
        int bufferSize = _bufferSizeMB * 1024 * 1024;
        var inputData = new byte[bufferSize];
        var outputBuffer = new byte[bufferSize + bufferSize / 8]; // Allow for expansion

        // Generate compressible test data (realistic file-like content)
        GenerateTestData(inputData, threadIndex);

        var sw = Stopwatch.StartNew();
        long totalBytesProcessed = 0;

        for (int iter = 0; iter < _iterations; iter++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            int compressedSize = _algorithm switch
            {
                CompressionAlgorithm.LZ77 => CompressLZ77(inputData, outputBuffer, _windowSize),
                CompressionAlgorithm.RLE => CompressRLE(inputData, outputBuffer),
                CompressionAlgorithm.Dictionary => CompressDictionary(inputData, outputBuffer),
                _ => CompressLZ77(inputData, outputBuffer, _windowSize)
            };

            totalBytesProcessed += inputData.Length;

            // Prevent optimization
            if (compressedSize == 0) throw new InvalidOperationException();
        }

        sw.Stop();

        // Calculate throughput in MB/s
        var seconds = sw.Elapsed.TotalSeconds;
        var mbPerSecond = totalBytesProcessed / seconds / (1024.0 * 1024.0);

        return mbPerSecond;
    }

    private static void GenerateTestData(byte[] data, int seed)
    {
        var rng = new Random(42 + seed);

        // Generate compressible data with patterns (like real files)
        int pos = 0;

        while (pos < data.Length)
        {
            int blockType = rng.Next(100);

            if (blockType < 30)
            {
                // Repeated pattern (like text with common words)
                int patternLength = rng.Next(4, 32);
                int repeatCount = rng.Next(3, 50);
                var pattern = new byte[patternLength];
                rng.NextBytes(pattern);

                for (int r = 0; r < repeatCount && pos < data.Length; r++)
                {
                    for (int p = 0; p < patternLength && pos < data.Length; p++)
                    {
                        data[pos++] = pattern[p];
                    }
                }
            }
            else if (blockType < 60)
            {
                // Run of similar bytes (like image regions)
                byte baseValue = (byte)rng.Next(256);
                int runLength = rng.Next(10, 200);

                for (int i = 0; i < runLength && pos < data.Length; i++)
                {
                    data[pos++] = (byte)(baseValue + rng.Next(-2, 3));
                }
            }
            else if (blockType < 80)
            {
                // Sequential/incremental data (like indices)
                byte start = (byte)rng.Next(256);
                int length = rng.Next(20, 100);

                for (int i = 0; i < length && pos < data.Length; i++)
                {
                    data[pos++] = (byte)(start + i);
                }
            }
            else
            {
                // Random data (incompressible)
                int length = rng.Next(10, 50);
                for (int i = 0; i < length && pos < data.Length; i++)
                {
                    data[pos++] = (byte)rng.Next(256);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CompressLZ77(byte[] input, byte[] output, int windowSize)
    {
        int inputPos = 0;
        int outputPos = 0;
        int inputLength = input.Length;

        while (inputPos < inputLength && outputPos < output.Length - 3)
        {
            int bestMatchLength = 0;
            int bestMatchOffset = 0;

            // Search for matches in the sliding window
            int windowStart = Math.Max(0, inputPos - windowSize);
            int maxMatchLength = Math.Min(258, inputLength - inputPos);

            for (int searchPos = windowStart; searchPos < inputPos; searchPos++)
            {
                int matchLength = 0;

                while (matchLength < maxMatchLength &&
                       inputPos + matchLength < inputLength &&
                       input[searchPos + matchLength] == input[inputPos + matchLength])
                {
                    matchLength++;

                    // Prevent searching past current position
                    if (searchPos + matchLength >= inputPos)
                        break;
                }

                if (matchLength > bestMatchLength)
                {
                    bestMatchLength = matchLength;
                    bestMatchOffset = inputPos - searchPos;
                }
            }

            if (bestMatchLength >= 3)
            {
                // Output match: flag byte + offset (2 bytes) + length
                output[outputPos++] = 0xFF; // Match marker
                output[outputPos++] = (byte)(bestMatchOffset & 0xFF);
                output[outputPos++] = (byte)((bestMatchOffset >> 8) | ((bestMatchLength - 3) << 4));
                inputPos += bestMatchLength;
            }
            else
            {
                // Output literal byte
                byte literal = input[inputPos++];
                if (literal == 0xFF)
                {
                    output[outputPos++] = 0xFF;
                    output[outputPos++] = 0x00; // Escaped literal 0xFF
                }
                else
                {
                    output[outputPos++] = literal;
                }
            }
        }

        return outputPos;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CompressRLE(byte[] input, byte[] output)
    {
        int inputPos = 0;
        int outputPos = 0;
        int inputLength = input.Length;

        while (inputPos < inputLength && outputPos < output.Length - 3)
        {
            byte currentByte = input[inputPos];
            int runLength = 1;

            // Count consecutive identical bytes
            while (inputPos + runLength < inputLength &&
                   runLength < 255 &&
                   input[inputPos + runLength] == currentByte)
            {
                runLength++;
            }

            if (runLength >= 4)
            {
                // Output run: marker + count + byte
                output[outputPos++] = 0x00; // RLE marker
                output[outputPos++] = (byte)runLength;
                output[outputPos++] = currentByte;
                inputPos += runLength;
            }
            else
            {
                // Output literal(s)
                int literalCount = 0;
                int literalStart = inputPos;

                while (inputPos < inputLength &&
                       literalCount < 127 &&
                       outputPos + literalCount + 2 < output.Length)
                {
                    // Check if next sequence would be better as RLE
                    if (inputPos + 4 <= inputLength)
                    {
                        byte next = input[inputPos];
                        if (input[inputPos + 1] == next &&
                            input[inputPos + 2] == next &&
                            input[inputPos + 3] == next)
                        {
                            break;
                        }
                    }

                    literalCount++;
                    inputPos++;
                }

                if (literalCount > 0)
                {
                    output[outputPos++] = (byte)(literalCount | 0x80); // Literal marker
                    for (int i = 0; i < literalCount; i++)
                    {
                        output[outputPos++] = input[literalStart + i];
                    }
                }
            }
        }

        return outputPos;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CompressDictionary(byte[] input, byte[] output)
    {
        // Simple dictionary-based compression
        // Uses a hash table to find previous occurrences of 4-byte sequences

        var dictionary = new Dictionary<int, int>(capacity: 65536);
        int inputPos = 0;
        int outputPos = 0;
        int inputLength = input.Length;

        while (inputPos < inputLength - 4 && outputPos < output.Length - 5)
        {
            // Compute hash of next 4 bytes
            int hash = ComputeHash(input, inputPos);

            if (dictionary.TryGetValue(hash, out int matchPos) && matchPos < inputPos)
            {
                // Verify match and compute length
                int matchLength = 0;
                int maxMatch = Math.Min(255, inputLength - inputPos);

                while (matchLength < maxMatch &&
                       input[matchPos + matchLength] == input[inputPos + matchLength])
                {
                    matchLength++;
                }

                if (matchLength >= 4)
                {
                    // Output match reference
                    int offset = inputPos - matchPos;
                    output[outputPos++] = 0x00; // Match marker
                    output[outputPos++] = (byte)(offset & 0xFF);
                    output[outputPos++] = (byte)((offset >> 8) & 0xFF);
                    output[outputPos++] = (byte)matchLength;

                    // Update dictionary for positions we're skipping
                    for (int i = 1; i < matchLength && inputPos + i < inputLength - 4; i++)
                    {
                        int h = ComputeHash(input, inputPos + i);
                        dictionary[h] = inputPos + i;
                    }

                    inputPos += matchLength;
                    continue;
                }
            }

            // Output literal and update dictionary
            dictionary[hash] = inputPos;
            byte literal = input[inputPos++];

            if (literal == 0x00)
            {
                output[outputPos++] = 0x00;
                output[outputPos++] = 0xFF; // Escaped literal 0x00
            }
            else
            {
                output[outputPos++] = literal;
            }
        }

        // Output remaining bytes as literals
        while (inputPos < inputLength && outputPos < output.Length)
        {
            byte literal = input[inputPos++];
            if (literal == 0x00 && outputPos < output.Length - 1)
            {
                output[outputPos++] = 0x00;
                output[outputPos++] = 0xFF;
            }
            else
            {
                output[outputPos++] = literal;
            }
        }

        return outputPos;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeHash(byte[] data, int pos)
    {
        // Simple hash of 4 bytes
        return data[pos] |
               (data[pos + 1] << 8) |
               (data[pos + 2] << 16) |
               (data[pos + 3] << 24);
    }

    protected override string GetScoreUnit() => "MB/s";

    protected override double AggregateResults(double[] threadScores)
    {
        // Sum throughput across all threads
        return threadScores.Sum();
    }
}
