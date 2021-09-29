[BITS 64]

%include "parameters.inc"

extern exit

global mul_avx512_int

section .text

mul_avx512_int:
    push rbp
	mov rax, ITERATIONS_mul_avx512i
    lea rbx, [rel avx_iv]
	vmovdqa32 zmm0, [rbx]
	vmovdqa32 zmm1, [rbx]
	vmovdqa32 zmm2, [rbx]
	vmovdqa32 zmm3, [rbx]
	vmovdqa32 zmm4, [rbx]
	vmovdqa32 zmm5, [rbx]
	vmovdqa32 zmm6, [rbx]
	vmovdqa32 zmm7, [rbx]
	vmovdqa32 zmm8, [rbx]
	vmovdqa32 zmm9, [rbx]
	vmovdqa32 zmm10, [rbx]
	vmovdqa32 zmm11, [rbx]
	vmovdqa32 zmm12, [rbx]
	vmovdqa32 zmm13, [rbx]
	vmovdqa32 zmm14, [rbx]
	vmovdqa32 zmm15, [rbx]
.loop:
	vpmulld zmm0, zmm0, zmm0
	vpmulld zmm1, zmm1, zmm1
	vpmulld zmm2, zmm2, zmm2
	vpmulld zmm3, zmm3, zmm3
	vpmulld zmm4, zmm4, zmm4
	vpmulld zmm5, zmm5, zmm5
	vpmulld zmm6, zmm6, zmm6
	vpmulld zmm7, zmm7, zmm7
	vpmulld zmm8, zmm8, zmm8
	vpmulld zmm9, zmm9, zmm9
	vpmulld zmm10, zmm10, zmm10
	vpmulld zmm11, zmm11, zmm11
	vpmulld zmm12, zmm12, zmm12
	vpmulld zmm13, zmm13, zmm13
	vpmulld zmm14, zmm14, zmm14
	vpmulld zmm15, zmm15, zmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_mul_avx512i
    mov rsi, 18 ; 16 vpmulld + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 64
avx_iv: dd 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1