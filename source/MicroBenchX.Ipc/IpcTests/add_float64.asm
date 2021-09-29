; x87 playground, not for productive use  

[BITS 64]

%include "parameters.inc"

extern exit

global add_float64

section .bss
result resq 1; reserve memory for result double


section .text

add_float64:
    push rbp
	mov rax, 4000000000
    finit ;reset fpu registers to default
    fld qword [double_value_1]
    ; fld qword [double_value_2]
    ; fld dword [double_value3]
    ; fld dword [double_value4]
    ; fld dword [double_value5]
    ; fld dword [double_value6]
    ; fld dword [double_value7]
    ; fld dword [double_value8]
    ; fld dword [double_value8]
.loop:
    ; how to handle the stack in a smart w when s0 is 0 after fadd?
    mov rax, __float64__(1.0)
    push rax
    ; fld qword [double_value_1]
    fld qword [rsp]
    add rsp, 8   
    fadd st1,st0
    ; fstp qword [result] 
    ; fadd st3,st2
    ; fadd st5,st4
    ; fadd st7,st6
	dec rax
	jnz .loop
.exit:
    lea rdi, [rel format]
    finit ;reset fpu registers to default
	pop rbp
	xor rax, rax
	mov rax, 4000000000
    mov rsi, 4 ; 1 fadd  + 1 dec + 1 loop
	mul rsi
    ret

section .data
format: db "%lu", 10, 0
double_value_1 dq 1.0    ; double value
double_value_2 dq 1.0    ; double value