using System.Runtime.InteropServices;

namespace OmniEMU.HLE.HOS.Services.Nv.NvDrvServices.NvHostChannel.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct AllocObjCtxArguments
    {
        public uint ClassNumber;
        public uint Flags;
        public ulong ObjectId;
    }
}
