using ARMeilleure.Common;
using ARMeilleure.Memory;
using OmniEMU.Cpu.LightningJit.Arm32;
using OmniEMU.Cpu.LightningJit.Arm64;
using OmniEMU.Cpu.LightningJit.State;
using System;
using System.Runtime.InteropServices;

namespace OmniEMU.Cpu.LightningJit
{
    class AarchCompiler
    {
        public static CompiledFunction Compile(
            CpuPreset cpuPreset,
            IMemoryManager memoryManager,
            ulong address,
            AddressTable<ulong> funcTable,
            IntPtr dispatchStubPtr,
            ExecutionMode executionMode,
            Architecture targetArch)
        {
            if (executionMode == ExecutionMode.Aarch64)
            {
                return A64Compiler.Compile(cpuPreset, memoryManager, address, funcTable, dispatchStubPtr, targetArch);
            }
            else
            {
                return A32Compiler.Compile(cpuPreset, memoryManager, address, funcTable, dispatchStubPtr, executionMode == ExecutionMode.Aarch32Thumb, targetArch);
            }
        }
    }
}
