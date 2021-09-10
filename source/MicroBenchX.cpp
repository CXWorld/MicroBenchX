// MicroBenchX.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <stdio.h>

// 36 tests
extern "C" unsigned __int64 add_avx256_float();
extern "C" unsigned __int64 add_avx256_int();
extern "C" unsigned __int64 add_avx512_float();
extern "C" unsigned __int64 add_avx512_int();
extern "C" unsigned __int64 add_int64();
extern "C" unsigned __int64 add_sse128_float();
extern "C" unsigned __int64 add_sse128_int();
extern "C" unsigned __int64 and_int64();
extern "C" unsigned __int64 div_avx256_float();
extern "C" unsigned __int64 div_avx512_float();
extern "C" unsigned __int64 div_int64();
extern "C" unsigned __int64 div_sse128_float();
extern "C" unsigned __int64 int_fp_mix();
extern "C" unsigned __int64 madd_avx256_float();
extern "C" unsigned __int64 madd_sse128_float();
extern "C" unsigned __int64 mov_int64();
extern "C" unsigned __int64 movdep_int64();
extern "C" unsigned __int64 movself_int64();
extern "C" unsigned __int64 mul_avx256_float();
extern "C" unsigned __int64 mul_avx256_int();
extern "C" unsigned __int64 mul_avx512_float();
extern "C" unsigned __int64 mul_avx512_int();
extern "C" unsigned __int64 mul_int64();
extern "C" unsigned __int64 mul_sse128_float();
extern "C" unsigned __int64 mul_sse128_int();
extern "C" unsigned __int64 or_int64();
extern "C" unsigned __int64 sub_avx256_float();
extern "C" unsigned __int64 sub_avx256_int();
extern "C" unsigned __int64 sub_avx512_float();
extern "C" unsigned __int64 sub_avx512_int();
extern "C" unsigned __int64 sub_int64();
extern "C" unsigned __int64 sub_sse128_float();
extern "C" unsigned __int64 sub_sse128_int();
extern "C" unsigned __int64 xor_int64();
extern "C" unsigned __int64 zen_fpu_mix_21();
extern "C" unsigned __int64 zen_fpu_mix_22();

int main()
{
    unsigned __int64 add_avx256_float_Iterations = add_avx256_float();
    unsigned __int64 add_avx256_int_Iterations = add_avx256_int();
    unsigned __int64 add_avx512_float_Iterations = add_avx512_float();
    unsigned __int64 add_avx512_int_Iterations = add_avx512_int();
    unsigned __int64 add_int64_Iterations = add_int64();
    unsigned __int64 add_sse128_float_Iterations = add_sse128_float();
    unsigned __int64 add_sse128_int_Iterations = add_sse128_int();
    unsigned __int64 and_int64_Iterations = and_int64();
    unsigned __int64 div_avx256_float_Iterations = div_avx256_float();
    unsigned __int64 div_avx512_float_Iterations = div_avx512_float();
    unsigned __int64 div_int64_Iterations = div_int64();
    unsigned __int64 div_sse128_float_Iterations = div_sse128_float();
    unsigned __int64 int_fp_mix_Iterations = int_fp_mix();
    unsigned __int64 madd_avx256_float_Iterations = madd_avx256_float();
    unsigned __int64 madd_sse128_float_Iterations = madd_sse128_float();
    unsigned __int64 mov_int64_Iterations = mov_int64();
    unsigned __int64 movdep_int64_Iterations = movdep_int64();
    unsigned __int64 movself_int64_Iterations = movself_int64();
    unsigned __int64 amul_avx256_float_Iterations = mul_avx256_float();
    unsigned __int64 mul_avx256_int_Iterations = mul_avx256_int();
    unsigned __int64 mul_avx512_float_Iterations = mul_avx512_float();
    unsigned __int64 amul_avx512_int_Iterations = mul_avx512_int();
    unsigned __int64 mul_int64_Iterations = mul_int64();
    unsigned __int64 mul_sse128_float_Iterations = mul_sse128_float();
    unsigned __int64 mul_sse128_int_Iterations = mul_sse128_int();
    unsigned __int64 or_int64_Iterations = or_int64();
    unsigned __int64 sub_avx256_float_Iterations = sub_avx256_float();
    unsigned __int64 sub_avx256_int_Iterations = sub_avx256_int();
    unsigned __int64 sub_avx512_float_Iterations = sub_avx512_float();
    unsigned __int64 sub_avx512_int_Iterations = sub_avx512_int();
    unsigned __int64 sub_int64_Iterations = sub_int64();
    unsigned __int64 sub_sse128_float_Iterations = sub_sse128_float();
    unsigned __int64 sub_sse128_int_Iterations = sub_sse128_int();
    unsigned __int64 xor_int64_Iterations = xor_int64();
    unsigned __int64 zen_fpu_mix_21_Iterations = zen_fpu_mix_21();
    unsigned __int64 zen_fpu_mix_22_Iterations = zen_fpu_mix_22();
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
