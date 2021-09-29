[BITS 64]

%include "parameters.inc"

extern exit

global sub_avx256_float

section .text

sub_avx256_float:
    push rbp
	mov rax, ITERATIONS_sub_avx256f
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
	vsubps ymm0, ymm0
	vsubps ymm1, ymm1
	vsubps ymm2, ymm2
	vsubps ymm3, ymm3
	vsubps ymm4, ymm4
	vsubps ymm5, ymm5
	vsubps ymm6, ymm6
	vsubps ymm7, ymm7
	vsubps ymm8, ymm8
	vsubps ymm9, ymm9
	vsubps ymm10, ymm10
	vsubps ymm11, ymm11
	vsubps ymm12, ymm12
	vsubps ymm13, ymm13
	vsubps ymm14, ymm14
	vsubps ymm15, ymm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_sub_avx256f
    mov rsi, 18 ; 16 vsubps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
avx_iv: dd 8.0, 7.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0