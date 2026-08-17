using OmniEMU.Audio.Common;
using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Applet;
using OmniEMU.Horizon.Sdk.Sf;
using System;

namespace OmniEMU.Horizon.Sdk.Audio.Detail
{
    interface IAudioOutManager : IServiceObject
    {
        Result ListAudioOuts(out int count, Span<DeviceName> names);
        Result OpenAudioOut(
            out AudioOutputConfiguration outputConfig,
            out IAudioOut audioOut,
            Span<DeviceName> outName,
            AudioInputConfiguration inputConfig,
            AppletResourceUserId appletResourceId,
            int processHandle,
            ReadOnlySpan<DeviceName> name,
            ulong pid);
        Result ListAudioOutsAuto(out int count, Span<DeviceName> names);
        Result OpenAudioOutAuto(
            out AudioOutputConfiguration outputConfig,
            out IAudioOut audioOut,
            Span<DeviceName> outName,
            AudioInputConfiguration inputConfig,
            AppletResourceUserId appletResourceId,
            int processHandle,
            ReadOnlySpan<DeviceName> name,
            ulong pid);
    }
}
