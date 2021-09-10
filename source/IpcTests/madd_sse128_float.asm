[BITS 64]

%include "parameters.inc"

extern exit

global madd_sse128_float

section .text

madd_sse128_float:
    push rbp
	mov rax, ITERATIONS_madd_sse128f
    lea rbx, [rel sse_iv]
	movdqa xmm0, [rbx]
	movdqa xmm1, [rbx]
	movdqa xmm2, [rbx]
	movdqa xmm3, [rbx]
	movdqa xmm4, [rbx]
	movdqa xmm5, [rbx]
	movdqa xmm6, [rbx]
	movdqa xmm7, [rbx]
	movdqa xmm8, [rbx]
	movdqa xmm9, [rbx]
	movdqa xmm10, [rbx]
	movdqa xmm11, [rbx]
	movdqa xmm12, [rbx]
	movdqa xmm13, [rbx]
	movdqa xmm14, [rbx]
	movdqa xmm15, [rbx]
.loop:
	vfmadd132ps xmm0, xmm0, xmm0
	vfmadd132ps xmm1, xmm1, xmm1
	vfmadd132ps xmm2, xmm2, xmm2
	vfmadd132ps xmm3, xmm3, xmm3
	vfmadd132ps xmm4, xmm4, xmm4
	vfmadd132ps xmm5, xmm5, xmm5
	vfmadd132ps xmm6, xmm6, xmm6
	vfmadd132ps xmm7, xmm7, xmm7
	vfmadd132ps xmm8, xmm8, xmm8
	vfmadd132ps xmm9, xmm9, xmm9
	vfmadd132ps xmm10, xmm10, xmm10
	vfmadd132ps xmm11, xmm11, xmm11
	vfmadd132ps xmm12, xmm12, xmm12
	vfmadd132ps xmm13, xmm13, xmm13
	vfmadd132ps xmm14, xmm14, xmm14
	vfmadd132ps xmm15, xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_madd_sse128f
    mov rsi, 18 ; 16 vfmadd132ps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4.0, 3.0, 2.0, 1.0