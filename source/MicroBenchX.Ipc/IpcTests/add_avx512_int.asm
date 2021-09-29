[BITS 64]

%include "parameters.inc"

extern exit

global add_avx512_int

section .text

add_avx512_int:
    push rbp
	mov rax, ITERATIONS_add_avx512i
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
	vpaddd zmm0, zmm0, zmm0
	vpaddd zmm1, zmm1, zmm1
	vpaddd zmm2, zmm2, zmm2
	vpaddd zmm3, zmm3, zmm3
	vpaddd zmm4, zmm4, zmm4
	vpaddd zmm5, zmm5, zmm5
	vpaddd zmm6, zmm6, zmm6
	vpaddd zmm7, zmm7, zmm7
	vpaddd zmm8, zmm8, zmm8
	vpaddd zmm9, zmm9, zmm9
	vpaddd zmm10, zmm10, zmm10
	vpaddd zmm11, zmm11, zmm11
	vpaddd zmm12, zmm12, zmm12
	vpaddd zmm13, zmm13, zmm13
	vpaddd zmm14, zmm14, zmm14
	vpaddd zmm15, zmm15, zmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_add_avx512i
    mov rsi, 18 ; 16 vpaddd + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 64
avx_iv: dd 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1