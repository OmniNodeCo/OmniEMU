using OmniEMU.Horizon.Bcat.Types;
using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Bcat;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.Bcat.Ipc
{
    partial class BcatService : IBcatService
    {
        public BcatService(BcatServicePermissionLevel permissionLevel) { }

        [CmifCommand(10100)]
        public Result RequestSyncDeliveryCache(out IDeliveryCacheProgressService deliveryCacheProgressService)
        {
            deliveryCacheProgressService = new DeliveryCacheProgressService();

            return Result.Success;
        }
    }
}
