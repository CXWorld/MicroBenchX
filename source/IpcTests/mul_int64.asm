[BITS 64]

%include "parameters.inc"

extern exit

global mul_int64

section .text

mul_int64:
    push rbp
	mov rbx, ITERATIONS_mul_int64
    mov rax, 1
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
    mul rax
    xor rax, rax
    mul rcx
    xor rax, rax
    mul rdx
    xor rax, rax
    mul rsi
    xor rax, rax
    mul rdi
    xor rax, rax
    mul r8
    xor rax, rax
    mul r9
    xor rax, rax
    mul r10
    xor rax, rax
    mul r11
    xor rax, rax
    mul r12
    xor rax, rax
    mul r13
    xor rax, rax
    mul r14
    xor rax, rax
    mul r15
    xor rax, rax
	dec rbx
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_mul_int64
    mov rsi, 15 ; 13 mul + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0