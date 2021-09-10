[BITS 64]

%include "parameters.inc"

extern exit

global add_sse128_float

section .text

add_sse128_float:
    push rbp
	mov rax, ITERATIONS_add_sse128f
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
	addps xmm0, xmm0
	addps xmm1, xmm1
	addps xmm2, xmm2
	addps xmm3, xmm3
	addps xmm4, xmm4
	addps xmm5, xmm5
	addps xmm6, xmm6
	addps xmm7, xmm7
	addps xmm8, xmm8
	addps xmm9, xmm9
	addps xmm10, xmm10
	addps xmm11, xmm11
	addps xmm12, xmm12
	addps xmm13, xmm13
	addps xmm14, xmm14
	addps xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_add_sse128f
    mov rsi, 18 ; addps addps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4.0, 3.0, 2.0, 1.0