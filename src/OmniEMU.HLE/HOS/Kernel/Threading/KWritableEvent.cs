using OmniEMU.HLE.HOS.Kernel.Common;
using OmniEMU.Horizon.Common;

namespace OmniEMU.HLE.HOS.Kernel.Threading
{
    class KWritableEvent : KAutoObject
    {
        private readonly KEvent _parent;

        public KWritableEvent(KernelContext context, KEvent parent) : base(context)
        {
            _parent = parent;
        }

        public void Signal()
        {
            _parent.ReadableEvent.Signal();
        }

        public Result Clear()
        {
            return _parent.ReadableEvent.Clear();
        }
    }
}
