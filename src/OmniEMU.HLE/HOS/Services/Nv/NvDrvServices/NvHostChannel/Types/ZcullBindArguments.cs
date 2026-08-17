using System.Runtime.InteropServices;

namespace OmniEMU.HLE.HOS.Services.Nv.NvDrvServices.NvHostChannel.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct ZcullBindArguments
    {
        public ulong GpuVirtualAddress;
        public uint Mode;
        public uint Reserved;
    }
}
