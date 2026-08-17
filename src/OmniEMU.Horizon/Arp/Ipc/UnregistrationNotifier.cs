using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Arp;
using OmniEMU.Horizon.Sdk.Arp.Detail;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.Arp.Ipc
{
    partial class UnregistrationNotifier : IUnregistrationNotifier, IServiceObject
    {
        private readonly ApplicationInstanceManager _applicationInstanceManager;

        public UnregistrationNotifier(ApplicationInstanceManager applicationInstanceManager)
        {
            _applicationInstanceManager = applicationInstanceManager;
        }

        [CmifCommand(0)]
        public Result GetReadableHandle([CopyHandle] out int readableHandle)
        {
            readableHandle = _applicationInstanceManager.EventHandle;

            return Result.Success;
        }
    }
}
