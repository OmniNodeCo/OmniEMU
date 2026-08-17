using OmniEMU.Graphics.Device;
using OmniEMU.Graphics.Nvdec.Image;

namespace OmniEMU.Graphics.Nvdec
{
    readonly struct ResourceManager
    {
        public DeviceMemoryManager MemoryManager { get; }
        public SurfaceCache Cache { get; }

        public ResourceManager(DeviceMemoryManager mm, SurfaceCache cache)
        {
            MemoryManager = mm;
            Cache = cache;
        }
    }
}
