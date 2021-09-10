[BITS 64]

%include "parameters.inc"

extern exit

global sub_sse128_float

section .text

sub_sse128_float:
    push rbp
	mov rax, ITERATIONS_sub_sse128f
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
	subps xmm0, xmm0
	subps xmm1, xmm1
	subps xmm2, xmm2
	subps xmm3, xmm3
	subps xmm4, xmm4
	subps xmm5, xmm5
	subps xmm6, xmm6
	subps xmm7, xmm7
	subps xmm8, xmm8
	subps xmm9, xmm9
	subps xmm10, xmm10
	subps xmm11, xmm11
	subps xmm12, xmm12
	subps xmm13, xmm13
	subps xmm14, xmm14
	subps xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_sub_sse128f
    mov rsi, 18 ; 16 subps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4.0, 3.0, 2.0, 1.0