using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.Sdk.Audio.Detail
{
    interface IAudioSnoopManager : IServiceObject
    {
        Result EnableDspUsageMeasurement();
        Result DisableDspUsageMeasurement();
        Result GetDspUsage(out uint usage);
    }
}
