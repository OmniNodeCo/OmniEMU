using OmniEMU.Horizon.Sdk.Codec.Detail;
using OmniEMU.Horizon.Sdk.Sf.Hipc;
using OmniEMU.Horizon.Sdk.Sm;

namespace OmniEMU.Horizon.Audio
{
    class HwopusIpcServer
    {
        private const int MaxSessionsCount = 24;

        private const int PointerBufferSize = 0x1000;
        private const int MaxDomains = 8;
        private const int MaxDomainObjects = 256;
        private const int MaxPortsCount = 1;

        private static readonly ManagerOptions _options = new(PointerBufferSize, MaxDomains, MaxDomainObjects, false);

        private SmApi _sm;
        private ServerManager _serverManager;

        public void Initialize()
        {
            HeapAllocator allocator = new();

            _sm = new SmApi();
            _sm.Initialize().AbortOnFailure();

            _serverManager = new ServerManager(allocator, _sm, MaxPortsCount, _options, MaxSessionsCount);

            HardwareOpusDecoderManager hardwareOpusDecoderManager = new();

            _serverManager.RegisterObjectForServer(hardwareOpusDecoderManager, ServiceName.Encode("hwopus"), MaxSessionsCount);
        }

        public void ServiceRequests()
        {
            _serverManager.ServiceRequests();
        }

        public void Shutdown()
        {
            _serverManager.Dispose();
            _sm.Dispose();
        }
    }
}
