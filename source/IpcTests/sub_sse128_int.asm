[BITS 64]

%include "parameters.inc"

extern exit

global sub_sse128_int

section .text

sub_sse128_int:
    push rbp
	mov rax, ITERATIONS_sub_sse128i
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
	psubd xmm0, xmm0
	psubd xmm1, xmm1
	psubd xmm2, xmm2
	psubd xmm3, xmm3
	psubd xmm4, xmm4
	psubd xmm5, xmm5
	psubd xmm6, xmm6
	psubd xmm7, xmm7
	psubd xmm8, xmm8
	psubd xmm9, xmm9
	psubd xmm10, xmm10
	psubd xmm11, xmm11
	psubd xmm12, xmm12
	psubd xmm13, xmm13
	psubd xmm14, xmm14
	psubd xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_sub_sse128i
    mov rsi, 18 ; 16 psubd + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4, 3, 2, 1