using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Prepo.Ipc;
using OmniEMU.Horizon.Prepo.Types;
using OmniEMU.Horizon.Sdk.Arp;
using OmniEMU.Horizon.Sdk.Sf.Hipc;
using OmniEMU.Horizon.Sdk.Sm;
using System;

namespace OmniEMU.Horizon.Prepo
{
    class PrepoServerManager : ServerManager
    {
        private readonly ArpApi _arp;

        public PrepoServerManager(HeapAllocator allocator, SmApi sm, ArpApi arp, int maxPorts, ManagerOptions options, int maxSessions) : base(allocator, sm, maxPorts, options, maxSessions)
        {
            _arp = arp;
        }

        protected override Result OnNeedsToAccept(int portIndex, Server server)
        {
            return (PrepoPortIndex)portIndex switch
            {
#pragma warning disable IDE0055 // Disable formatting
                PrepoPortIndex.Admin   => AcceptImpl(server, new PrepoService(_arp, PrepoServicePermissionLevel.Admin)),
                PrepoPortIndex.Admin2  => AcceptImpl(server, new PrepoService(_arp, PrepoServicePermissionLevel.Admin)),
                PrepoPortIndex.Manager => AcceptImpl(server, new PrepoService(_arp, PrepoServicePermissionLevel.Manager)),
                PrepoPortIndex.User    => AcceptImpl(server, new PrepoService(_arp, PrepoServicePermissionLevel.User)),
                PrepoPortIndex.System  => AcceptImpl(server, new PrepoService(_arp, PrepoServicePermissionLevel.System)),
                PrepoPortIndex.Debug   => AcceptImpl(server, new PrepoService(_arp, PrepoServicePermissionLevel.Debug)),
                _                      => throw new ArgumentOutOfRangeException(nameof(portIndex)),
#pragma warning restore IDE0055
            };
        }
    }
}
