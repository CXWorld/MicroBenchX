[BITS 64]

%include "parameters.inc"

extern exit

global sub_avx512_float

section .text

sub_avx512_float:
    push rbp
	mov rax, ITERATIONS_sub_avx512f
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
	vsubps zmm0, zmm0, zmm0
	vsubps zmm1, zmm1, zmm1
	vsubps zmm2, zmm2, zmm2
	vsubps zmm3, zmm3, zmm3
	vsubps zmm4, zmm4, zmm4
	vsubps zmm5, zmm5, zmm5
	vsubps zmm6, zmm6, zmm6
	vsubps zmm7, zmm7, zmm7
	vsubps zmm8, zmm8, zmm8
	vsubps zmm9, zmm9, zmm9
	vsubps zmm10, zmm10, zmm10
	vsubps zmm11, zmm11, zmm11
	vsubps zmm12, zmm12, zmm12
	vsubps zmm13, zmm13, zmm13
	vsubps zmm14, zmm14, zmm14
	vsubps zmm15, zmm15, zmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_sub_avx512f
    mov rsi, 18 ; 16 vsubps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 64
avx_iv: dd 16.0, 15.0, 14.0, 13.0, 12.0, 11.0, 10.0, 9.0, 8.0, 7.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0