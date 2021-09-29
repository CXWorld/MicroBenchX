[BITS 64]

%include "parameters.inc"

extern exit

global mul_avx256_int

section .text

mul_avx256_int:
    push rbp
	mov rax, ITERATIONS_mul_avx256i
    lea rbx, [rel avx_iv]
	vmovdqa ymm0, [rbx]
	vmovdqa ymm1, [rbx]
	vmovdqa ymm2, [rbx]
	vmovdqa ymm3, [rbx]
	vmovdqa ymm4, [rbx]
	vmovdqa ymm5, [rbx]
	vmovdqa ymm6, [rbx]
	vmovdqa ymm7, [rbx]
	vmovdqa ymm8, [rbx]
	vmovdqa ymm9, [rbx]
	vmovdqa ymm10, [rbx]
	vmovdqa ymm11, [rbx]
	vmovdqa ymm12, [rbx]
	vmovdqa ymm13, [rbx]
	vmovdqa ymm14, [rbx]
	vmovdqa ymm15, [rbx]
.loop:
	vpmulld ymm0, ymm0
	vpmulld ymm1, ymm1
	vpmulld ymm2, ymm2
	vpmulld ymm3, ymm3
	vpmulld ymm4, ymm4
	vpmulld ymm5, ymm5
	vpmulld ymm6, ymm6
	vpmulld ymm7, ymm7
	vpmulld ymm8, ymm8
	vpmulld ymm9, ymm9
	vpmulld ymm10, ymm10
	vpmulld ymm11, ymm11
	vpmulld ymm12, ymm12
	vpmulld ymm13, ymm13
	vpmulld ymm14, ymm14
	vpmulld ymm15, ymm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_mul_avx256i
    mov rsi, 18 ; 16 vpmulld + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
avx_iv: dd 8, 7, 6, 5, 4, 3, 2, 1