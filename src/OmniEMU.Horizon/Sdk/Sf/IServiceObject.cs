using System.Collections.Generic;

namespace OmniEMU.Horizon.Sdk.Sf
{
    interface IServiceObject
    {
        IReadOnlyDictionary<int, CommandHandler> GetCommandHandlers();
    }
}
