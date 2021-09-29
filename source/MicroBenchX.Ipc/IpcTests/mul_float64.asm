[BITS 64]

%include "parameters.inc"

extern exit

global mul_float64

section .text

mul_float64:
    push rbp
	mov rbx, ITERATIONS_mul_float64
    mov rax, __float64__(1.0)
    mov rcx, __float64__(2.0)
    mov rdx, __float64__(3.0)
    mov rsi, __float64__(4.0)
    mov rdi, __float64__(5.0)
    mov r8, __float64__(6.0)
    mov r9, __float64__(7.0)
    mov r10, __float64__(8.0)
    mov r11, __float64__(9.0)
    mov r12, __float64__(10.0)
    mov r13, __float64__(11.0)
    mov r14, __float64__(12.0)
    mov r15, __float64__(13.0)
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
	mov rax, ITERATIONS_mul_float64
    mov rsi, 15 ; 13 mul + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0