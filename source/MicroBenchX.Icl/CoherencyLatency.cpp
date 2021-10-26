#include <stdio.h>
#include <stdint.h>
#include <sys\timeb.h>
#include <intrin.h>
#include <windows.h>
#include <iostream>
#include <conio.h>

#define ITERATIONS 1000000/*1000000*/;
#define RUNS 10;

float RunTest(unsigned int processor1, unsigned int processor2, uint64_t iter);
float RunOwnedTest(unsigned int processor1, unsigned int processor2, uint64_t iter);
DWORD WINAPI LatencyTestThread(LPVOID param);
DWORD WINAPI ReadLatencyTestThread(LPVOID param);

typedef struct LatencyThreadData {
	uint64_t start;       // initial value to write into target
	uint64_t iterations;  // number of iterations to run
	LONG64* target;       // value to bounce between threads, init with start - 1
	LONG64* readTarget;   // for read test, memory location to read from (owned by other core)
	DWORD affinityMask;   // thread affinity mask to set
} LatencyData;

int main(int argc, char* argv[]) {
	SYSTEM_INFO sysInfo;
	DWORD numProcs;
	float* latencies;
	float* latenciesFiltered;
	uint64_t iter = ITERATIONS;
	uint64_t runs = RUNS;
	float (*test)(unsigned int, unsigned int, uint64_t) = RunTest;

	GetSystemInfo(&sysInfo);
	numProcs = sysInfo.dwNumberOfProcessors;
	fprintf(stderr, "Number of CPUs: %u\n", numProcs);

	int ch;
	printf("\nPress any key to start the test:");
	ch = _getch();

	latencies = (float*)malloc(sizeof(float) * numProcs * numProcs * runs);
	latenciesFiltered = (float*)malloc(sizeof(float) * numProcs * numProcs);
	if (latencies == NULL) {
		fprintf(stderr, "\ncouldn't allocate result array\n");
		return 0;
	}

	if (argc > 1) {
		iter = atol(argv[1]);
		fprintf(stderr, "\n%lu iterations requested\n", iter);
	}
	else {
		fprintf(stderr, "\nUsage: MicroBenchX.C2CLatency.exe [iterations] [bounce/owned]\n");
	}

	if (argc > 2) {
		if (strncmp(argv[2], "owned", 5) == 0) {
			test = RunOwnedTest;
			fprintf(stderr, "\nUsing separate cache lines for each thread to write to\n");
		}
	}

	float progress = 0.0;
	int barWidth = 60;

	// Run all to all, skipping testing a core against itself ofc
	// technically can skip the other way around (start j = i + 1) but meh
	for (DWORD i = 0; i < numProcs; i++)
	{
		std::cout << "[";
		progress = (float)i / numProcs;
		int pos = barWidth * progress;
		for (int i = 0; i < barWidth; ++i) {
			if (i < pos) std::cout << "=";
			else if (i == pos) std::cout << ">";
			else std::cout << " ";
		}
		std::cout << "] " << int(progress * 100.0) << " %\r";
		std::cout.flush();

		for (DWORD j = 0; j < numProcs; j++)
		{
			for (DWORD k = 0; k < runs; k++)
			{
				latencies[j + i * numProcs + k * numProcs * numProcs] = i == j ? 0 : test(i, j, iter);
			}
		}
	}

	// Statical analysis
	for (DWORD i = 0; i < numProcs; i++)
	{
		for (DWORD j = 0; j < numProcs; j++)
		{
			float min = 1000000.0;
			for (DWORD k = 0; k < runs; k++)
			{
				if (latencies[j + i * numProcs + k * numProcs * numProcs] < min);
				min = latencies[j + i * numProcs + k * numProcs * numProcs];
			}

			latenciesFiltered[j + i * numProcs] = i == j ? 0 : min;
		}
	}

	// Symmetric
	for (DWORD i = 0; i < numProcs; i++)
	{
		for (DWORD j = i; j < numProcs; j++)
		{
			float avg = (latenciesFiltered[j + i * numProcs] + latenciesFiltered[i + j * numProcs]) / 2;
			latenciesFiltered[j + i * numProcs] = avg;
			latenciesFiltered[i + j * numProcs] = avg;
		}
	}

	// print thing to copy to excel
	for (DWORD i = 0; i < numProcs; i++) {
		for (DWORD j = 0; j < numProcs; j++) {
			if (j != 0) printf(",");
			if (j == i) printf("x");
			else printf("%f", latenciesFiltered[j + i * numProcs]);
		}
		printf("\n");
	}

	printf("\nPress any key to close the app");
	ch = _getch();

	free(latencies);
	return 0;
}

float TimeThreads(unsigned int processor1, unsigned int processor2, uint64_t iter, LatencyData lat1, LatencyData lat2, DWORD(*threadFunc)(LPVOID)) {
	struct timeb start, end;
	HANDLE testThreads[2];
	DWORD tid1, tid2;

	testThreads[0] = CreateThread(NULL, 0, threadFunc, &lat1, CREATE_SUSPENDED, &tid1);
	testThreads[1] = CreateThread(NULL, 0, threadFunc, &lat2, CREATE_SUSPENDED, &tid2);

	if (testThreads[0] == NULL || testThreads[1] == NULL) {
		fprintf(stderr, "Failed to create test threads\n");
		return -1;
	}

	SetThreadAffinityMask(testThreads[0], 1ULL << (uint64_t)processor1);
	SetThreadAffinityMask(testThreads[1], 1ULL << (uint64_t)processor2);

	ftime(&start);
	ResumeThread(testThreads[0]);
	ResumeThread(testThreads[1]);
	WaitForMultipleObjects(2, testThreads, TRUE, INFINITE);
	ftime(&end);

	int64_t time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	float latency = 1e6 * (float)time_diff_ms / (float)iter;

	// fprintf(stderr, "%d to %d: %f ns\n", processor1, processor2, latency);

	CloseHandle(testThreads[0]);
	CloseHandle(testThreads[1]);

	// each thread does interlocked compare and exchange iterations times. divide by 2 to get overall count of locked ops
	return latency / 2;
}

/// <summary>
/// Measures latency from one processor core to another
/// </summary>
/// <param name="processor1">processor number 1</param>
/// <param name="processor2">processor number 2</param>
/// <param name="iter">Number of iterations</param>
/// <param name="bouncy">aligned mem to bounce around</param>
/// <returns>latency per iteration in ns</returns>
float RunTest(unsigned int processor1, unsigned int processor2, uint64_t iter) {
	LatencyData lat1, lat2;
	LONG64* bouncy;
	float latency;

	bouncy = (LONG64*)_aligned_malloc(sizeof(LONG64), sizeof(LONG64));
	if (bouncy == NULL) {
		fprintf(stderr, "Could not allocate aligned mem\n");
	}

	*bouncy = 0;
	lat1.iterations = iter;
	lat1.start = 1;
	lat1.target = bouncy;
	lat2.iterations = iter;
	lat2.start = 2;
	lat2.target = bouncy;

	latency = TimeThreads(processor1, processor2, iter, lat1, lat2, LatencyTestThread);
	_aligned_free(bouncy);
	return latency;
}

float RunOwnedTest(unsigned int processor1, unsigned int processor2, uint64_t iter) {
	LatencyData lat1, lat2;
	LONG64* target1, * target2;
	float latency;

	// drop them on different cache lines
	target1 = (LONG64*)_aligned_malloc(128, 64);
	target2 = target1 + 8;
	if (target1 == NULL) {
		fprintf(stderr, "Could not allocate aligned mem\n");
	}

	*target1 = 1;
	*target2 = 0;
	lat1.iterations = iter;
	lat1.start = 3;
	lat1.target = target1;
	lat1.readTarget = target2;
	lat2.iterations = iter;
	lat2.start = 2;
	lat2.target = target2;
	lat2.readTarget = target1;

	latency = TimeThreads(processor1, processor2, iter, lat1, lat2, ReadLatencyTestThread);
	_aligned_free(target1);
	return latency;
}

/// <summary>
/// Runs one thread of the latency test. should be run in pairs
/// Always writes to target
/// </summary>
/// <param name="param">Latency test params</param>
/// <returns>next value that would have been written to shared memory</returns>
DWORD WINAPI LatencyTestThread(LPVOID param) {
	LatencyData* latencyData = (LatencyData*)param;
	uint64_t current = latencyData->start;
	while (current <= 2 * latencyData->iterations) {
		if (_InterlockedCompareExchange64(latencyData->target, current, current - 1) == current - 1) {
			current += 2;
		}
	}

	return current;
}

/// <summary>
/// Similar thing but tries to not bounce cache line ownership
/// Instead, threads write to different cache lines
/// </summary>
/// <param name="param">Latency test params</param>
/// <returns>next value that would have been written to owned mem</returns>
DWORD WINAPI ReadLatencyTestThread(LPVOID param) {
	LatencyData* latencyData = (LatencyData*)param;
	uint64_t current = latencyData->start;
	uint64_t startTsc = __rdtsc();
	while (current <= 2 * latencyData->iterations) {
		if (*(latencyData->readTarget) == current - 1) {
			*(latencyData->target) = current;
			current += 2;
			_mm_sfence();
		}
	}

	return current;
}