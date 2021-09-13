[BITS 64]

%include "parameters.inc"

extern exit

global add_float64

section .text

add_float64:
    push rbp
	mov rax, ITERATIONS_add_float64
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
    add rbx, rbx
    add rcx, rcx
    add rdx, rdx
    add rsi, rsi
    add rdi, rdi
    add r8, r8
    add r9, r9
    add r10, r10
    add r11, r11
    add r12, r12
    add r13, r13
    add r14, r14
    add r15, r15
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
	pop rbp
	xor rax, rax
	mov rax, ITERATIONS_add_float64
    mov rsi, 15 ; 13 add + 1 dec + 1 loop
	mul rsi
    ret

section .data
format: db "%lu", 10, 0