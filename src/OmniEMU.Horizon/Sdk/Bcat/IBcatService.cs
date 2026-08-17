using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.Sdk.Bcat
{
    internal interface IBcatService : IServiceObject
    {
        Result RequestSyncDeliveryCache(out IDeliveryCacheProgressService deliveryCacheProgressService);
    }
}
