[BITS 64]

%include "parameters.inc"

extern exit

global div_sse128_float

section .text

div_sse128_float:
    push rbp
	mov rax, ITERATIONS_div_sse128f
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
	divps xmm0, xmm0
	divps xmm1, xmm1
	divps xmm2, xmm2
	divps xmm3, xmm3
	divps xmm4, xmm4
	divps xmm5, xmm5
	divps xmm6, xmm6
	divps xmm7, xmm7
	divps xmm8, xmm8
	divps xmm9, xmm9
	divps xmm10, xmm10
	divps xmm11, xmm11
	divps xmm12, xmm12
	divps xmm13, xmm13
	divps xmm14, xmm14
	divps xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_div_sse128f
    mov rsi, 18 ; 16 divps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 1.0, 1.0, 1.0, 1.0