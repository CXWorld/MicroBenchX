[BITS 64]

%include "parameters.inc"

extern exit

global div_int64

section .text

div_int64:
    push rbp
	mov rbx, ITERATIONS_div_int64
    xor rax, rax
    xor rdx, rdx
    mov rcx, 2
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
    div rcx
    xor rdx, rdx
    xor rax, rax
    div rsi
    xor rdx, rdx
    xor rax, rax
    div rdi
    xor rdx, rdx
    xor rax, rax
    div r8
    xor rdx, rdx
    xor rax, rax
    div r9
    xor rdx, rdx
    xor rax, rax
    div r10
    xor rdx, rdx
    xor rax, rax
    div r11
    xor rdx, rdx
    xor rax, rax
    div r12
    xor rdx, rdx
    xor rax, rax
    div r13
    xor rdx, rdx
    xor rax, rax
    div r14
    xor rdx, rdx
    xor rax, rax
    div r15
    xor rdx, rdx
    xor rax, rax
	dec rbx
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_div_int64
    mov rsi, 13 ; 11 div + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0