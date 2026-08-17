using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf;
using System;

namespace OmniEMU.Horizon.Sdk.Lm
{
    interface ILmLogger : IServiceObject
    {
        Result Log(Span<byte> message);
        Result SetDestination(LogDestination destination);
    }
}
