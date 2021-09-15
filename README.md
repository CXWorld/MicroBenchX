# MicroBenchX
Micro benchmarks CPU/GPU. 

### Basic IPC Tests
Assembler code is based on the great work by Nemes: https://git.nemez.net/nemes/ipc-tests. The code is not comparable because it was modified! 

These tests do not represent the real world performance of a CPU. In practice the performance depends on branch prediction, caching and other things like out-of-order executions.

It is very important to set a fixed clock speed for the IPC tests. Disable all turbos, sleep states and AVX offsets. Set "High performance" power plan.

### Frameworks and Packages
* Microsoft Visual C++ Redistributable for Visual Studio: https://bit.ly/3tIQqFh

### Install Steps Netwide Assembler (NASM)
* Download installer: https://www.nasm.us/
* Download VSNASM batch installer: https://github.com/ShiftMediaProject/VSNASM
* Create environment variable: Name=NASM, Variable Value=C:\Program Files\NASM

### To Do
Add other micro benchmarks. Create a WPF UI.