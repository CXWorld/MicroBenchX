[BITS 64]

%include "parameters.inc"

extern exit
extern printf

global mov_int64

section .text

mov_int64:
    push rbp
	mov rax, ITERATIONS_mov_int64
     mov rbx, __float64__(1.0)
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