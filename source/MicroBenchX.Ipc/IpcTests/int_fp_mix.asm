[BITS 64]

%include "parameters.inc"

extern exit

global int_fp_mix

section .text

int_fp_mix:
    push rbp
	mov rax, ITERATIONS_int_fp_mix
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
    mov rbx, 1
    mov rcx, 2
    mov rdx, 3
    mov rsi, 4
    mov rdi, 5
    mov r8, 6
    mov r9, 7
    mov r10, 8
    mov r11, 9
    mov r12, 10
    mov r13, 11
    mov r14, 12
    mov r15, 13
.loop:
	vfmadd132ps xmm0, xmm0, xmm0
    add rbx, rbx
    xor rbx, rbx
    mov r9, rbx
    nop
	vfmadd132ps xmm1, xmm1, xmm1
    add rcx, rcx
    xor rcx, rcx
    mov r10, rcx
    nop
	vfmadd132ps xmm2, xmm2, xmm2
    add rdx, rdx
    xor rdx, rdx
    mov r11, rdx
    nop
	vfmadd132ps xmm3, xmm3, xmm3
    add rsi, rsi
    xor rsi, rsi
    mov r12, rsi
    nop
	vfmadd132ps xmm4, xmm4, xmm4
    add rdi, rdi
    xor rdi, rdi
    mov r13, rdi
    nop
	vfmadd132ps xmm5, xmm5, xmm5
    add r8, r8
    xor r8, r8
    mov r14, r8
    nop
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_int_fp_mix
    mov rsi, 26 ; 6 vfmadd132ps + 6 add + 6 mov + 6 nop 1 + dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0
align 32
sse_iv: dd 4.0, 3.0, 2.0, 1.0