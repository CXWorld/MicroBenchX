[BITS 64]

%include "parameters.inc"

extern exit

global add_int64

section .text

add_int64:
    push rbp
	mov rax, ITERATIONS_add_int64
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
	mov rax, ITERATIONS_add_int64
    mov rsi, 15 ; 13 add + 1 dec + 1 loop
	mul rsi
    ret

section .data

format: db "%lu", 10, 0