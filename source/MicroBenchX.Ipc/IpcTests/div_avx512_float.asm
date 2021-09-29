[BITS 64]

%include "parameters.inc"

extern exit

global div_avx512_float

section .text

div_avx512_float:
    push rbp
	mov rax, ITERATIONS_div_avx512f
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
	vdivps zmm0, zmm0, zmm0
	vdivps zmm1, zmm1, zmm1
	vdivps zmm2, zmm2, zmm2
	vdivps zmm3, zmm3, zmm3
	vdivps zmm4, zmm4, zmm4
	vdivps zmm5, zmm5, zmm5
	vdivps zmm6, zmm6, zmm6
	vdivps zmm7, zmm7, zmm7
	vdivps zmm8, zmm8, zmm8
	vdivps zmm9, zmm9, zmm9
	vdivps zmm10, zmm10, zmm10
	vdivps zmm11, zmm11, zmm11
	vdivps zmm12, zmm12, zmm12
	vdivps zmm13, zmm13, zmm13
	vdivps zmm14, zmm14, zmm14
	vdivps zmm15, zmm15, zmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_div_avx512f
    mov rsi, 18 ; 16 vdivps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 64
avx_iv: dd 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0