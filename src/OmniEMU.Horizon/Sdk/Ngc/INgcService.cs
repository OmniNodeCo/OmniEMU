using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf;
using System;

namespace OmniEMU.Horizon.Sdk.Ngc
{
    interface INgcService : IServiceObject
    {
        Result GetContentVersion(out uint version);
        Result Check(out uint checkMask, ReadOnlySpan<byte> text, uint regionMask, ProfanityFilterOption option);
        Result Mask(out int maskedWordsCount, Span<byte> filteredText, ReadOnlySpan<byte> text, uint regionMask, ProfanityFilterOption option);
        Result Reload();
    }
}
