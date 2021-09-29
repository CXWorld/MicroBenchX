[BITS 64]

%include "parameters.inc"

extern exit

global sub_int64

section .text

sub_int64:
    push rbp
	mov rax, ITERATIONS_sub_int64
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
    sub rbx, rbx
    sub rcx, rcx
    sub rdx, rdx
    sub rsi, rsi
    sub rdi, rdi
    sub r8, r8
    sub r9, r9
    sub r10, r10
    sub r11, r11
    sub r12, r12
    sub r13, r13
    sub r14, r14
    sub r15, r15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_sub_int64
    mov rsi, 15 ; 13 sub + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0