using System.Runtime.InteropServices;

namespace OmniEMU.HLE.HOS.Services.Nv.NvDrvServices.NvHostAsGpu.Types
{
    [StructLayout(LayoutKind.Sequential)]
    struct BindChannelArguments
    {
        public int Fd;
    }
}
