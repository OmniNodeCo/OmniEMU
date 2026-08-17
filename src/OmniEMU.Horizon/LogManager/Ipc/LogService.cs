using OmniEMU.Horizon.Common;
using OmniEMU.Horizon.Sdk.Lm;
using OmniEMU.Horizon.Sdk.Sf;

namespace OmniEMU.Horizon.LogManager.Ipc
{
    partial class LogService : ILogService
    {
        public LogDestination LogDestination { get; set; } = LogDestination.TargetManager;

        [CmifCommand(0)]
        public Result OpenLogger(out LmLogger logger, [ClientProcessId] ulong pid)
        {
            // NOTE: Internal name is Logger, but we rename it to LmLogger to avoid name clash with OmniEMU.Common.Logging logger.
            logger = new LmLogger(this, pid);

            return Result.Success;
        }
    }
}
