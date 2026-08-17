using OmniEMU.Horizon.Common;

namespace OmniEMU.Horizon.Sdk.OsTypes
{
    static class OsResult
    {
        private const int ModuleId = 3;

        public static Result OutOfResource => new(ModuleId, 9);
    }
}
