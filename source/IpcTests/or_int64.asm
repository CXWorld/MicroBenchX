[BITS 64]

%include "parameters.inc"

extern exit

global or_int64

section .text

or_int64:
    push rbp
	mov rax, ITERATIONS_or_int64
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
    or rbx, rbx
    or rcx, rcx
    or rdx, rdx
    or rsi, rsi
    or rdi, rdi
    or r8, r8
    or r9, r9
    or r10, r10
    or r11, r11
    or r12, r12
    or r13, r13
    or r14, r14
    or r15, r15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_or_int64
    mov rsi, 15 ; 13 or + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0