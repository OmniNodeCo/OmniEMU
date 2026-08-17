using OmniEMU.Horizon.Sdk.Ncm;
using System.Runtime.InteropServices;

namespace OmniEMU.Horizon.Sdk.Friends
{
    [StructLayout(LayoutKind.Sequential, Size = 0x10, Pack = 0x8)]
    struct ApplicationInfo
    {
        public ApplicationId ApplicationId;
        public ulong PresenceGroupId;
    }
}
