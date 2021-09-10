[BITS 64]

%include "parameters.inc"

extern exit

global div_avx256_float

section .text

div_avx256_float:
    push rbp
	mov rax, ITERATIONS_div_avx256f
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
	vdivps ymm0, ymm0
	vdivps ymm1, ymm1
	vdivps ymm2, ymm2
	vdivps ymm3, ymm3
	vdivps ymm4, ymm4
	vdivps ymm5, ymm5
	vdivps ymm6, ymm6
	vdivps ymm7, ymm7
	vdivps ymm8, ymm8
	vdivps ymm9, ymm9
	vdivps ymm10, ymm10
	vdivps ymm11, ymm11
	vdivps ymm12, ymm12
	vdivps ymm13, ymm13
	vdivps ymm14, ymm14
	vdivps ymm15, ymm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_div_avx256f
    mov rsi, 18 ; 16 vdivps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
avx_iv: dd 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0