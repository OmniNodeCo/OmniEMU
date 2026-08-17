using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.Sdk.Sm
{
    interface IUserService : IServiceObject
    {
        Result Initialize(ulong clientProcessId);
        Result GetService(out int handle, ServiceName name);
        Result RegisterService(out int handle, ServiceName name, int maxSessions, bool isLight);
        Result UnregisterService(ServiceName name);
    }
}
