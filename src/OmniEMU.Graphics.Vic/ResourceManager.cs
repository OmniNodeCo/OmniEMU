using OmniEMU.Graphics.Device;
using OmniEMU.Graphics.Vic.Image;

namespace OmniEMU.Graphics.Vic
{
    readonly struct ResourceManager
    {
        public DeviceMemoryManager MemoryManager { get; }
        public BufferPool<Pixel> SurfacePool { get; }
        public BufferPool<byte> BufferPool { get; }

        public ResourceManager(DeviceMemoryManager mm, BufferPool<Pixel> surfacePool, BufferPool<byte> bufferPool)
        {
            MemoryManager = mm;
            SurfacePool = surfacePool;
            BufferPool = bufferPool;
        }
    }
}
