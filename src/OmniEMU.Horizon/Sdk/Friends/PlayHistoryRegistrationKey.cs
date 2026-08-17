using OmniEMU.Common.Memory;
using OmniEMU.Horizon.Sdk.Account;
using System.Runtime.InteropServices;

namespace OmniEMU.Horizon.Sdk.Friends
{
    [StructLayout(LayoutKind.Sequential, Size = 0x40)]
    struct PlayHistoryRegistrationKey
    {
        public ushort Type;
        public byte KeyIndex;
        public byte UserIdBool;
        public byte UnknownBool;
        public Array11<byte> Reserved;
        public Uid Uuid;
        public Array32<byte> HmacHash;
    }
}
