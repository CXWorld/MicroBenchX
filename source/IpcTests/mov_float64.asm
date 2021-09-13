[BITS 64]

%include "parameters.inc"

extern exit
extern printf

global mov_int64

section .text

mov_int64:
    push rbp
	mov rax, ITERATIONS_mov_int64
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
    mov rbx, rcx
    mov rcx, rdx
    mov rdx, rsi
    mov rsi, rdi
    mov rdi, r8
    mov r8, r9
    mov r9, r10
    mov r10, r11
    mov r11, r12
    mov r12, r13
    mov r13, r14
    mov r14, r15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_mov_int64
    mov rsi, 14 ; 12 mov + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0