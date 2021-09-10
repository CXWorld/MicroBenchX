[BITS 64]

%include "parameters.inc"

extern exit

global add_avx512_float

section .text

add_avx512_float:
    push rbp
	mov rax, ITERATIONS_add_avx512f
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
	vaddps zmm0, zmm0, zmm0
	vaddps zmm1, zmm1, zmm1
	vaddps zmm2, zmm2, zmm2
	vaddps zmm3, zmm3, zmm3
	vaddps zmm4, zmm4, zmm4
	vaddps zmm5, zmm5, zmm5
	vaddps zmm6, zmm6, zmm6
	vaddps zmm7, zmm7, zmm7
	vaddps zmm8, zmm8, zmm8
	vaddps zmm9, zmm9, zmm9
	vaddps zmm10, zmm10, zmm10
	vaddps zmm11, zmm11, zmm11
	vaddps zmm12, zmm12, zmm12
	vaddps zmm13, zmm13, zmm13
	vaddps zmm14, zmm14, zmm14
	vaddps zmm15, zmm15, zmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_add_avx512f
    mov rsi, 18 ; 16 vaddps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 64
avx_iv: dd 16.0, 15.0, 14.0, 13.0, 12.0, 11.0, 10.0, 9.0, 8.0, 7.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0