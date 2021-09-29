[BITS 64]

%include "parameters.inc"

extern exit

global mul_sse128_int

section .text

mul_sse128_int:
    push rbp
	mov rax, ITERATIONS_mul_sse128i
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
	pmulld xmm0, xmm0
	pmulld xmm1, xmm1
	pmulld xmm2, xmm2
	pmulld xmm3, xmm3
	pmulld xmm4, xmm4
	pmulld xmm5, xmm5
	pmulld xmm6, xmm6
	pmulld xmm7, xmm7
	pmulld xmm8, xmm8
	pmulld xmm9, xmm9
	pmulld xmm10, xmm10
	pmulld xmm11, xmm11
	pmulld xmm12, xmm12
	pmulld xmm13, xmm13
	pmulld xmm14, xmm14
	pmulld xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_mul_sse128i
    mov rsi, 18 ; 16 pmulld + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4, 3, 2, 1