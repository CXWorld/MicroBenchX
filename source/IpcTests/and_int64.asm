[BITS 64]

%include "parameters.inc"

extern exit

global and_int64

section .text

and_int64:
    push rbp
	mov rax, ITERATIONS_and_int64
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
    and rbx, rbx
    and rcx, rcx
    and rdx, rdx
    and rsi, rsi
    and rdi, rdi
    and r8, r8
    and r9, r9
    and r10, r10
    and r11, r11
    and r12, r12
    and r13, r13
    and r14, r14
    and r15, r15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_and_int64
    mov rsi, 15 ; 13 and + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0