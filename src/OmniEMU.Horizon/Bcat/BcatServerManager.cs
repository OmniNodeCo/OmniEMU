using OmniEMU.Horizon.Bcat.Ipc;
using OmniEMU.Horizon.Bcat.Types;
using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf.Hipc;
using OmniEMU.Horizon.Sdk.Sm;
using System;

namespace OmniEMU.Horizon.Bcat
{
    class BcatServerManager : ServerManager
    {
        public BcatServerManager(HeapAllocator allocator, SmApi sm, int maxPorts, ManagerOptions options, int maxSessions) : base(allocator, sm, maxPorts, options, maxSessions)
        {
        }

        protected override Result OnNeedsToAccept(int portIndex, Server server)
        {
            return (BcatPortIndex)portIndex switch
            {
                BcatPortIndex.Admin => AcceptImpl(server, new ServiceCreator("bcat:a", BcatServicePermissionLevel.Admin)),
                BcatPortIndex.Manager => AcceptImpl(server, new ServiceCreator("bcat:m", BcatServicePermissionLevel.Manager)),
                BcatPortIndex.User => AcceptImpl(server, new ServiceCreator("bcat:u", BcatServicePermissionLevel.User)),
                BcatPortIndex.System => AcceptImpl(server, new ServiceCreator("bcat:s", BcatServicePermissionLevel.System)),
                _ => throw new ArgumentOutOfRangeException(nameof(portIndex)),
            };
        }
    }
}
