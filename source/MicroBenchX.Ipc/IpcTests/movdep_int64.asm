[BITS 64]

%include "parameters.inc"

extern exit

global movdep_int64

section .text

movdep_int64:
    push rbp
	mov rax, ITERATIONS_movself_int64
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
    mov rcx, rbx
    mov rdx, rcx
    mov rsi, rdx
    mov rdi, rsi
    mov r8, rdi
    mov r9, r8
    mov r10, r9
    mov r11, r10
    mov r12, r11
    mov r13, r12
    mov r14, r13
    mov r15, r14
    mov rbx, r15
	dec rax
	jnz .loop
.exit:
   lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_movself_int64
    mov rsi, 15 ; 13 mov + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0