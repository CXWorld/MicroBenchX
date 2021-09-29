[BITS 64]

%include "parameters.inc"

extern exit

global mul_sse128_float

section .text

mul_sse128_float:
    push rbp
	mov rax, ITERATIONS_mul_sse128f
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
	mulps xmm0, xmm0
	mulps xmm1, xmm1
	mulps xmm2, xmm2
	mulps xmm3, xmm3
	mulps xmm4, xmm4
	mulps xmm5, xmm5
	mulps xmm6, xmm6
	mulps xmm7, xmm7
	mulps xmm8, xmm8
	mulps xmm9, xmm9
	mulps xmm10, xmm10
	mulps xmm11, xmm11
	mulps xmm12, xmm12
	mulps xmm13, xmm13
	mulps xmm14, xmm14
	mulps xmm15, xmm15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_mul_sse128f
    mov rsi, 18 ; 16 mulps + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4.0, 3.0, 2.0, 1.0