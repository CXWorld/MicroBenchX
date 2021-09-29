[BITS 64]

%include "parameters.inc"

extern exit

global add_sse128_int

section .text

add_sse128_int:
    push rbp
	mov rax, ITERATIONS_add_sse128i
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
	paddd xmm0, xmm0
	paddd xmm1, xmm1
	paddd xmm2, xmm2
	paddd xmm3, xmm3
	paddd xmm4, xmm4
	paddd xmm5, xmm5
	paddd xmm6, xmm6
	paddd xmm7, xmm7
	paddd xmm8, xmm8
	paddd xmm9, xmm9
	paddd xmm10, xmm10
	paddd xmm11, xmm11
	paddd xmm12, xmm12
	paddd xmm13, xmm13
	paddd xmm14, xmm14
	paddd xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_add_sse128i
    mov rsi, 18 ; 16 paddd + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4, 3, 2, 1