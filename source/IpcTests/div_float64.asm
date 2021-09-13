[BITS 64]

%include "parameters.inc"

extern exit

global div_float64

section .text

div_float64:
    push rbp
	mov rbx, ITERATIONS_div_float64
    xor rax, rax
    xor rdx, rdx
    mov rcx, __float64__(2.0)
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
	mov rax, ITERATIONS_div_float64
    mov rsi, 13 ; 11 div + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0