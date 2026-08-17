using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Applet;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.Sdk.Audio.Detail
{
    interface IFinalOutputRecorderManager : IServiceObject
    {
        Result OpenFinalOutputRecorder(
            out IFinalOutputRecorder recorder,
            FinalOutputRecorderParameter parameter,
            int processHandle,
            out FinalOutputRecorderParameterInternal outParameter,
            AppletResourceUserId appletResourceId);
    }
}
