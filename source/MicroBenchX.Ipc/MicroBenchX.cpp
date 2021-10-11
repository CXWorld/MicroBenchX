// MicroBenchX.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <stdio.h>
#include <string>
#include<sys\timeb.h>
#include<windows.h>
#include <conio.h>
#include "InstructionSet.cpp"
#include "Hwinfo.cpp"

// 36 tests
extern "C" uint64_t add_avx256_float();
extern "C" uint64_t add_avx256_int();
extern "C" uint64_t add_avx512_float();
extern "C" uint64_t add_avx512_int();
extern "C" uint64_t add_int64();
extern "C" uint64_t add_sse128_float();
extern "C" uint64_t add_sse128_int();
extern "C" uint64_t and_int64();
extern "C" uint64_t div_avx256_float();
extern "C" uint64_t div_avx512_float();
extern "C" uint64_t div_int64();
extern "C" uint64_t div_sse128_float();
extern "C" uint64_t int_fp_mix();
extern "C" uint64_t madd_avx256_float();
extern "C" uint64_t madd_sse128_float();
extern "C" uint64_t mov_int64();
extern "C" uint64_t movdep_int64();
extern "C" uint64_t movself_int64();
extern "C" uint64_t mul_avx256_float();
extern "C" uint64_t mul_avx256_int();
extern "C" uint64_t mul_avx512_float();
extern "C" uint64_t mul_avx512_int();
extern "C" uint64_t mul_int64();
extern "C" uint64_t mul_sse128_float();
extern "C" uint64_t mul_sse128_int();
extern "C" uint64_t or_int64();
extern "C" uint64_t sub_avx256_float();
extern "C" uint64_t sub_avx256_int();
extern "C" uint64_t sub_avx512_float();
extern "C" uint64_t sub_avx512_int();
extern "C" uint64_t sub_int64();
extern "C" uint64_t sub_sse128_float();
extern "C" uint64_t sub_sse128_int();
extern "C" uint64_t xor_int64();
extern "C" uint64_t zen_fpu_mix_21();
extern "C" uint64_t zen_fpu_mix_22();

const InstructionSet::InstructionSet_Internal InstructionSet::CPU_Rep;
uint64_t ClockSpeed_MHz;

class TestData
{
public:
	std::string testName;
	uint64_t time;
	uint64_t iterations;
	double_t sum_IPC_All = 0;
	double_t sum_IPC_Without_AVX512 = 0;
	uint32_t count_IPC_All = 0;
	uint32_t count_IPC_Without_AVX512 = 0;
	double_t sum_time_All = 0;
	double_t sum_time_Without_AVX512 = 0;
	std::string step;
	bool countAVX512;

	void SetStep(bool decCount)
	{
		if (decCount)
			count_IPC_All++;
		std::string count = std::to_string(count_IPC_All);
		step = count + "/" + std::to_string(36);
	}
};

void ManageResults(TestData* data)
{
	uint64_t cyles = 1000llu * ClockSpeed_MHz * data->time;
	double_t measured_Ipc = (double_t)data->iterations / cyles;

	data->sum_IPC_All += measured_Ipc;
	data->count_IPC_All++;
	data->sum_time_All += (double_t)data->time;

	if (!data->countAVX512)
	{
		data->sum_IPC_Without_AVX512 += measured_Ipc;
		data->count_IPC_Without_AVX512++;
		data->sum_time_Without_AVX512 += (double_t)data->time;
	}

	data->SetStep(false);
	printf("%-8s %-20s %-10.2f %-5.2f \n", data->step.c_str(), data->testName.c_str(), (double_t)data->time / 1000.0L, measured_Ipc);
}

void PrintNotSupportedTest(TestData* data)
{
	data->SetStep(true);
	printf("%-8s %-20s %-10s \n", data->step.c_str(), data->testName.c_str(), "not supported");
}

void PrintAverageResults(TestData* data)
{
	printf("\n%-29s %-10.2f %-5.2f \n", "Average all tests w/o AVX512",
		data->sum_time_Without_AVX512 / ((double_t)data->count_IPC_Without_AVX512 * 1000), data->sum_IPC_Without_AVX512 / data->count_IPC_Without_AVX512);

	if (InstructionSet::AVX512F()) {
		printf("%-29s %-10.2f %-5.2f \n", "Average all tests w/ AVX512",
			data->sum_time_All / ((double_t)data->count_IPC_Without_AVX512 * 1000), data->sum_IPC_All / data->count_IPC_All);
	}
}

int main()
{
	std::cout << "MicroBenchX v1.0 - CPU IPC micro benchmarks \n\n";
	std::cout << "[!] Set a fixed clock speed before running the test. [!]\n\n";
	std::cout << InstructionSet::Brand().c_str() << "\n";
	// clockSpeed_MHz = Hwinfo::Frequency() / 1000000;
	printf("Enter current frequency in MHz: ");
	std::cin >> ClockSpeed_MHz;

	printf("\n\n%-8s %-20s %-10s %-5s \n", "Step", "Test", "Time[s]", "IPC");

	TestData data;
	uint64_t time_diff_ms;
	struct timeb start, end;

	data.testName = "Add AVX256 Float";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = add_avx256_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);

	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Add AVX256 Integer";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = add_avx256_int();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Add AVX512 Float";
	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		data.iterations = add_avx512_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = true;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Add AVX512 Integer";
	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		data.iterations = add_avx512_int();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = true;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Add Integer 64";
	ftime(&start);
	data.iterations = add_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Add SSE128 Float";
	ftime(&start);
	data.iterations = add_sse128_float();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Add SSE128 Integer";
	ftime(&start);
	data.iterations = add_sse128_int();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "And Integer 64";
	ftime(&start);
	data.iterations = and_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Div AVX256 Float";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = div_avx256_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Div AVX512 Float";
	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		data.iterations = div_avx512_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = true;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Div Integer 64";
	ftime(&start);
	data.iterations = div_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Div SSE128 Float";
	ftime(&start);
	data.iterations = div_sse128_float();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Mix Integer Float";
	ftime(&start);
	data.iterations = int_fp_mix();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Madd AVX256 Float";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = madd_avx256_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Madd SSE128 Float";
	ftime(&start);
	data.iterations = madd_sse128_float();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Copy Integer 64";
	ftime(&start);
	data.iterations = mov_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Copy Dep Integer 64";
	ftime(&start);
	data.iterations = movdep_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Copy Self Integer 64";
	ftime(&start);
	data.iterations = movself_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Mul AVX256 Float";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = mul_avx256_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Mul AVX256 Integer";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = mul_avx256_int();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Mul AVX512 Float";
	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		data.iterations = mul_avx512_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = true;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Mul AVX512 Integer";
	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		data.iterations = mul_avx512_int();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = true;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Mul Integer 64";
	ftime(&start);
	data.iterations = mul_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Mul SSE128 Float";
	ftime(&start);
	data.iterations = mul_sse128_float();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Mul SSE128 Integer";
	ftime(&start);
	data.iterations = mul_sse128_int();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Or Integer 64";
	ftime(&start);
	data.iterations = or_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Sub AVX256 Float";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = sub_avx256_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Sub AVX256 Integer";
	if (InstructionSet::AVX())
	{
		ftime(&start);
		data.iterations = sub_avx256_int();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = false;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Sub AVX512 Float";
	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		data.iterations = sub_avx512_float();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = true;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Sub AVX512 Integer";
	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		data.iterations = sub_avx512_int();
		ftime(&end);
		data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		data.countAVX512 = true;
		ManageResults(&data);
	}
	else {
		PrintNotSupportedTest(&data);
	}

	data.testName = "Sub Integer 64";
	ftime(&start);
	data.iterations = sub_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Sub SSE128 Float";
	ftime(&start);
	data.iterations = sub_sse128_float();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Sub SSE128 Integer";
	ftime(&start);
	data.iterations = sub_sse128_int();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Xor Integer 64";
	ftime(&start);
	data.iterations = xor_int64();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Zen FPU Mix 21";
	ftime(&start);
	data.iterations = zen_fpu_mix_21();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	data.testName = "Zen FPU Mix 22";
	ftime(&start);
	data.iterations = zen_fpu_mix_22();
	ftime(&end);
	data.time = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	data.countAVX512 = false;
	ManageResults(&data);

	PrintAverageResults(&data);

	int ch;
	printf("\nPress any key to exit...\n");
	ch = _getch();
}
