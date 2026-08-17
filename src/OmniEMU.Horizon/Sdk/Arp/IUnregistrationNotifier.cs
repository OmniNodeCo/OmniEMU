using OmniEMU.Horizon.Common;

namespace OmniEMU.Horizon.Sdk.Arp
{
    public interface IUnregistrationNotifier
    {
        public Result GetReadableHandle(out int readableHandle);
    }
}
