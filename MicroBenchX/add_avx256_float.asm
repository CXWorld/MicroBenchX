[BITS 64]

%include "parameters.inc"

extern exit
; extern printf

global add_avx256f

section .text

add_avx256f:
    push rbp
	mov rax, ITERATIONS_add_avx256f
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
	vaddps ymm0, ymm0
	vaddps ymm1, ymm1
	vaddps ymm2, ymm2
	vaddps ymm3, ymm3
	vaddps ymm4, ymm4
	vaddps ymm5, ymm5
	vaddps ymm6, ymm6
	vaddps ymm7, ymm7
	vaddps ymm8, ymm8
	vaddps ymm9, ymm9
	vaddps ymm10, ymm10
	vaddps ymm11, ymm11
	vaddps ymm12, ymm12
	vaddps ymm13, ymm13
	vaddps ymm14, ymm14
	vaddps ymm15, ymm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
    mov rax, ITERATIONS_add_avx256f
    mov rsi, 16
    mul rsi
    mov rsi, rax
    xor rax, rax
	; call printf wrt ..plt
    pop rbp
	xor rax, rax
    ret

section .data

format: db "%lu", 10, 0
align 16
avx_iv: dd 8.0, 7.0, 6.0, 5.0, 4.0, 3.0, 2.0, 1.0
