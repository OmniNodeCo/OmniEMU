using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Applet;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.Sdk.Audio.Detail
{
    partial class FinalOutputRecorderManager : IFinalOutputRecorderManager
    {
        [CmifCommand(0)]
        public Result OpenFinalOutputRecorder(
            out IFinalOutputRecorder recorder,
            FinalOutputRecorderParameter parameter,
            [CopyHandle] int processHandle,
            out FinalOutputRecorderParameterInternal outParameter,
            AppletResourceUserId appletResourceId)
        {
            recorder = new FinalOutputRecorder(processHandle);
            outParameter = new(parameter.SampleRate, 2, 0);

            return Result.Success;
        }
    }
}
