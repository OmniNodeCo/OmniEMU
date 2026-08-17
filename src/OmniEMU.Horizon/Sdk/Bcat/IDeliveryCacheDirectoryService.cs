using LibHac.Bcat;
using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf;
using System;

namespace OmniEMU.Horizon.Sdk.Bcat
{
    internal interface IDeliveryCacheDirectoryService : IServiceObject
    {
        Result GetCount(out int count);
        Result Open(DirectoryName directoryName);
        Result Read(out int entriesRead, Span<DeliveryCacheDirectoryEntry> entriesBuffer);
    }
}
