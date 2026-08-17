using System.Runtime.InteropServices;

namespace OmniEMU.HLE.HOS.Services.Nv.NvDrvServices.NvHostCtrlGpu.Types
{

    [StructLayout(LayoutKind.Sequential)]
    struct GetTpcMasksArguments
    {
        public int MaskBufferSize;
        public int Reserved;
        public long MaskBufferAddress;
        public int TpcMask;
        public int Padding;
    }
}
