using OmniEMU.Horizon.Sdk.Ncm;

namespace OmniEMU.Horizon.Sdk.Arp
{
    public struct ApplicationLaunchProperty
    {
        public ApplicationId ApplicationId;
        public uint Version;
        public StorageId Storage;
        public StorageId PatchStorage;
        public ApplicationKind ApplicationKind;
        public byte Padding;
    }
}
