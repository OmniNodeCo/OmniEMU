using LibHac.Bcat;
using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Sf;
using System;

namespace OmniEMU.Horizon.Sdk.Bcat
{
    internal interface IDeliveryCacheStorageService : IServiceObject
    {
        Result CreateDirectoryService(out IDeliveryCacheDirectoryService service);
        Result CreateFileService(out IDeliveryCacheFileService service);
        Result EnumerateDeliveryCacheDirectory(out int count, Span<DirectoryName> directoryNames);
    }
}
