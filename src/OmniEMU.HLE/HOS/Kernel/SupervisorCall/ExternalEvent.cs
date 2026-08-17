using OmniEMU.HLE.HOS.Kernel.Threading;
using OmniEMU.Horizon.Common;

namespace OmniEMU.HLE.HOS.Kernel.SupervisorCall
{
    readonly struct ExternalEvent : IExternalEvent
    {
        private readonly KWritableEvent _writableEvent;

        public ExternalEvent(KWritableEvent writableEvent)
        {
            _writableEvent = writableEvent;
        }

        public readonly void Signal()
        {
            _writableEvent.Signal();
        }

        public readonly void Clear()
        {
            _writableEvent.Clear();
        }
    }
}
