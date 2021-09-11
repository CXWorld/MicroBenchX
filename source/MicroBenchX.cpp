// MicroBenchX.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <stdio.h>
#include <string>
#include<sys\timeb.h>
#include "InstructionSet.cpp"

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

void PrintIpcResults(const std::string testName, const uint64_t instructions,
	const uint64_t time, const uint64_t clockSpeedMHz)
{
	double_t ipc = (double)instructions / ((double)clockSpeedMHz * 1000 * time);
	printf("%-20s %-10.2f %-5.2f \n", testName.c_str(), (double)time / 1000, ipc);
}

int main()
{
	std::cout << "MircoBenchX v1.0 - CPU IPC micro benchmarks \n\n";
	std::cout << "[!] Set a fixed clock speed before running the test. [!]\n\n";
	std::cout << InstructionSet::Brand().c_str() << "\n";
	std::cout << "Please enter the CPU clock speed in MHz: ";
	uint64_t clockSpeed_MHz;
	std::cin >> clockSpeed_MHz;

	printf("\n\n%-20s %-10s %-5s \n", "Test", "Time[s]", "IPC");

	uint64_t time_diff_ms;
	struct timeb start, end;

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t add_avx256_float_Iterations = add_avx256_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Add AVX256 Float", add_avx256_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Add AVX256 Float", "not supported");
	}

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t add_avx256_int_Iterations = add_avx256_int();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Add AVX256 Integer", add_avx256_int_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Add AVX256 Integer", "not supported");
	}

	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		uint64_t add_avx512_float_Iterations = add_avx512_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Add AVX512 Float", add_avx512_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Add AVX512 Float", "not supported");
	}

	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		uint64_t add_avx512_int_Iterations = add_avx512_int();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Add AVX512 Integer", add_avx512_int_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Add AVX512 Integer", "not supported");
	}

	ftime(&start);
	uint64_t add_int64_Iterations = add_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Add Integer 64", add_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t add_sse128_float_Iterations = add_sse128_float();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Add SSE128 Float", add_sse128_float_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t add_sse128_int_Iterations = add_sse128_int();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Add SSE128 Integer", add_sse128_int_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t and_int64_Iterations = and_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("And Integer 64", and_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t div_avx256_float_Iterations = div_avx256_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Div AVX256 Float", div_avx256_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Div AVX256 Float", "not supported");
	}

	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		uint64_t div_avx512_float_Iterations = div_avx512_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Div AVX512 Float", div_avx512_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Div AVX512 Float", "not supported");
	}

	ftime(&start);
	uint64_t div_int64_Iterations = div_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Div Integer 64", div_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t div_sse128_float_Iterations = div_sse128_float();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Div SSE128 Float", div_sse128_float_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t int_fp_mix_Iterations = int_fp_mix();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Mix Integer Float", int_fp_mix_Iterations, time_diff_ms, clockSpeed_MHz);

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t madd_avx256_float_Iterations = madd_avx256_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Madd AVX256 Float", madd_avx256_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Madd AVX256 Float", "not supported");
	}

	ftime(&start);
	uint64_t madd_sse128_float_Iterations = madd_sse128_float();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Madd SSE128 Float", madd_sse128_float_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t mov_int64_Iterations = mov_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Copy Integer 64", mov_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t movdep_int64_Iterations = movdep_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Copy Deep Integer 64", movdep_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t movself_int64_Iterations = movself_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Copy Self Integer 64", movself_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t mul_avx256_float_Iterations = mul_avx256_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Mul AVX256 Float", mul_avx256_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Mul AVX256 Float", "not supported");
	}

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t mul_avx256_int_Iterations = mul_avx256_int();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Mul AVX256 Integer", mul_avx256_int_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Mul AVX256 Integer", "not supported");
	}

	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		uint64_t mul_avx512_float_Iterations = mul_avx512_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Mul AVX512 Float", mul_avx512_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Mul AVX512 Float", "not supported");
	}

	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		uint64_t mul_avx512_int_Iterations = mul_avx512_int();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Mul AVX512 Integer", mul_avx512_int_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Mul AVX512 Integer", "not supported");
	}

	ftime(&start);
	uint64_t mul_int64_Iterations = mul_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Mul Integer 64", mul_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t mul_sse128_float_Iterations = mul_sse128_float();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Mul SSE128 Float", mul_sse128_float_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t mul_sse128_int_Iterations = mul_sse128_int();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Mul SSE128 Integer", mul_sse128_int_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t or_int64_Iterations = or_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Or Integer 64", or_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t sub_avx256_float_Iterations = sub_avx256_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Sub AVX256 Float", sub_avx256_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Sub AVX256 Float", "not supported");
	}

	if (InstructionSet::AVX())
	{
		ftime(&start);
		uint64_t sub_avx256_int_Iterations = sub_avx256_int();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Sub AVX256 Integer", sub_avx256_int_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Sub AVX256 Integer", "not supported");
	}

	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		uint64_t sub_avx512_float_Iterations = sub_avx512_float();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Sub AVX512 Float", sub_avx512_float_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Sub AVX512 Float", "not supported");
	}

	if (InstructionSet::AVX512F())
	{
		ftime(&start);
		uint64_t sub_avx512_int_Iterations = sub_avx512_int();
		ftime(&end);
		time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
		PrintIpcResults("Sub AVX512 Integer", sub_avx512_int_Iterations, time_diff_ms, clockSpeed_MHz);
	}
	else {
		printf("%-20s %-10s \n", "Sub AVX512 Integer", "not supported");
	}

	ftime(&start);
	uint64_t sub_int64_Iterations = sub_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Sub Integer 64", sub_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t sub_sse128_float_Iterations = sub_sse128_float();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Sub SSE128 Float", sub_sse128_float_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t sub_sse128_int_Iterations = sub_sse128_int();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Sub SSE128 Integer", sub_sse128_int_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t xor_int64_Iterations = xor_int64();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Xor Integer 64", xor_int64_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t zen_fpu_mix_21_Iterations = zen_fpu_mix_21();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Zen FPU Mix 21", zen_fpu_mix_21_Iterations, time_diff_ms, clockSpeed_MHz);

	ftime(&start);
	uint64_t zen_fpu_mix_22_Iterations = zen_fpu_mix_22();
	ftime(&end);
	time_diff_ms = 1000 * (end.time - start.time) + (end.millitm - start.millitm);
	PrintIpcResults("Zen FPU Mix 22", zen_fpu_mix_22_Iterations, time_diff_ms, clockSpeed_MHz);
}
